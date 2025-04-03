using System;
using System.ClientModel;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using DocumentValidator.ViewModels;
using OpenAI.Chat;
using DocumentValidator.Utils;
using Xceed.Words.NET;
using System.Net.Http;
using Microsoft.Maui.Storage;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DocumentValidator.Core.SemanticValidatorProcessor
{
    public class SemanticValidatorProcessor
    {
        private readonly DocumentViewModel _documentViewModel;

        public SemanticValidatorProcessor(DocumentViewModel documentViewModel)
        {
            _documentViewModel = documentViewModel ?? throw new ArgumentNullException(nameof(documentViewModel));
        }

        public async Task EvaluateDocumentAsync(Stream documentStream)
        {
            IConfiguration configuration = CreateConfiguration();
            StreamToDocx converter = new StreamToDocx();
            DocX document = await converter.StreamToDocxConverter(documentStream);

            try
            {

                AzureOpenAIClient azureClient = new(
                    new Uri(configuration["OpenAI:Endpoint"]),
                    new AzureKeyCredential(configuration["OpenAI:ApiKey"]));
                ChatClient chatClient = azureClient.GetChatClient(configuration["OpenAI:DeploymentName"]);
                List<ChatMessage> messages = new List<ChatMessage>()
                {
                  new SystemChatMessage(@"1. Grammar, spelling, and syntax errors (identify specific examples)
2. Coherence and logical flow of ideas
3. Factual accuracy of all claims and data points
4. Customer consistency - verify the document references only ONE customer company throughout (flag if multiple companies like ""Google"" and ""Apple"" appear)
5. Quality and appropriateness of any recommendations
6. Formatting, structure, and readability issues
7. Any inconsistencies in terminology, tone, or perspective

For each issue identified, please:
- Quote the problematic text directly
- Explain specifically what is wrong
- Provide a suggested correction

Present your analysis in an HTML table with these columns:
- Category (Grammar, Coherence, Facts, Customer Consistency, Recommendations, Other)
- Issue (specific problem identified)
- Location (section/paragraph reference)
- Suggestion (proposed correction)

Do not include any text outside of the HTML table in your response."),

                 new UserChatMessage(document.Text),
                };
                var response = await chatClient.CompleteChatAsync(messages);
                _documentViewModel.AddLogMessage(response.Value.Content[0].Text);
                string filePath = GetDownloadLocation.GetDownloadsFolderPath() + "\\AIResults.html";
                StringBuilder htmlContent = new StringBuilder();
                htmlContent.AppendLine("<!DOCTYPE html>");
                htmlContent.AppendLine("<html lang='en'>");
                htmlContent.AppendLine("<head>");
                htmlContent.AppendLine("    <meta charset='UTF-8'>");
                htmlContent.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
                htmlContent.AppendLine("    <title>Validation Results</title>");
                htmlContent.AppendLine($"    <link rel='stylesheet' type='text/css' href='Styles.css'>");
                htmlContent.AppendLine("</head>");
                htmlContent.AppendLine("<body>");


                htmlContent.AppendLine("    <div class='container'>");
                htmlContent.AppendLine("    <div class='nav-buttons'>");
                htmlContent.AppendLine($"        <a href='LinkValidation.html' class='nav-button'>Link Results</a>");
                htmlContent.AppendLine("        <a href='AIResults.html' class='nav-button'>AI Results</a>");
                htmlContent.AppendLine("    </div>");
                htmlContent.Append(response.Value.Content[0].Text);
                htmlContent.AppendLine("    </div>");
                htmlContent.AppendLine("</body>");
                htmlContent.AppendLine("</html>");
                await File.WriteAllTextAsync(filePath, htmlContent.ToString());

            }
            catch (RequestFailedException ex)
            {
                _documentViewModel.AddLogMessage($"OpenAI API Error: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                _documentViewModel.AddLogMessage($"Request timed out: {ex.Message}");
            }
            catch (Exception ex)
            {
                _documentViewModel.AddLogMessage($"Unexpected Error: {ex.Message}");
            }
        }

        public static IConfiguration CreateConfiguration()
        {
            // Use Directory.GetCurrentDirectory() or an appropriate base path for your environment.
            var configuration = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build();


            return configuration;
        }
    }
}


