
using DocumentValidator.Utils;
using DocumentValidator.ViewModels;
using Microsoft.Extensions.Logging;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocumentValidator.Core.DocumentProcessor
{

    public class LinkValidator
    {
        StreamToDocx converter = new StreamToDocx();
        //Injecting DocumentViewModel to update the UI
        private readonly DocumentViewModel _documentViewModel;

        private readonly HttpClient _httpClient;

        public LinkValidator(DocumentViewModel documentViewModel)
        {
            _httpClient = new HttpClient();
            _documentViewModel = documentViewModel;
        }
        public async Task<List<LinkValidationResult>> ValidateDocumentLinks(Stream documentStream)
        {
            DocX document = await converter.StreamToDocxConverter(documentStream);

            var results = new List<LinkValidationResult>();
            try
            {
                
                        var hyperlinks = document.Hyperlinks;
                        if (hyperlinks != null && hyperlinks.Count > 0)
                        {
                         

                            foreach (var hyperlink in hyperlinks)
                            {
                                var url = hyperlink.Uri;


                                // _logger.LogInformation($"Validating URL: {url}");
                                if (url != null)
                                    try
                                {
                                    var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                                        

                                        bool isValid = response.IsSuccessStatusCode;

                                    var result = new LinkValidationResult
                                    {
                                        Url = url.AbsoluteUri,
                                        LinkText = hyperlink.Text,
                                        IsValid = isValid,
                                        StatusCode = (int)response.StatusCode,
                                        ErrorMessage = isValid ? null : $"HTTP Status: {response.StatusCode}"
                                    };
                                    if(!isValid)
                                        {
                                            //For UI to be updated, the call should happen in the main thread.
                                            MainThread.BeginInvokeOnMainThread(() =>
                                            {
                                                _documentViewModel.AddLogMessage($"Validating URL: {url} - status code is: {response.StatusCode}");
                                            });
                                        }
                                    results.Add(result);
                                   // _logger.LogInformation($"URL validation result: {url} - {(isValid ? "Valid" : "Invalid")} ({(int)response.StatusCode})");
                                }
                                    catch (NotSupportedException ex) when (ex.Message.Contains("rhttps"))
                                    {

                                        var result = new LinkValidationResult
                                        {
                                            Url = url.ToString(),
                                            LinkText = hyperlink.Text,
                                            IsValid = false,
                                            StatusCode = 0, // Indicating no HTTP response was received
                                            ErrorMessage = $"Invalid URI: {ex.Message}"
                                        };
                                        //For UI to be updated, the call should happen in the main thread.
                                        MainThread.BeginInvokeOnMainThread(() =>
                                        {
                                            _documentViewModel.AddLogMessage($"Validating URL: {url} - status code is: {ex.Message}");
                                        });
                                        results.Add(result);

                                    }
                            }

                        }
                    
                
            }
            catch (Exception ex)
            {
               Console.WriteLine($"An error occurred while validating links: {ex.Message}");
                // Log the exception or handle it as needed
            }

            return results;
        }
    }
}
    

