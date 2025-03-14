using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocumentValidator.Core.DocumentProcessor
{

    public class LinkValidator
    {
        private readonly HttpClient _httpClient;


        public LinkValidator()
        {
            _httpClient = new HttpClient();
        }
        public async Task<List<LinkValidationResult>> ValidateDocumentLinks(Stream documentStream)
        {
            var results = new List<LinkValidationResult>();
            try
            {
                // Create a new MemoryStream with the entire content
                documentStream.Position = 0;
                byte[] docBytes = new byte[documentStream.Length];
                await documentStream.ReadAsync(docBytes, 0, docBytes.Length);
                

                // Create a MemoryStream copy to ensure we have a complete stream to work with
                using (var memoryStream = new MemoryStream(docBytes))
                {
                    
                    

                    using (DocX document = DocX.Load(memoryStream))
                    {
                        //  _logger.LogInformation("Document opened successfully");

                        // Get all hyperlinks in the document
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
                                        IsValid = isValid,
                                        StatusCode = (int)response.StatusCode,
                                        ErrorMessage = isValid ? null : $"HTTP Status: {response.StatusCode}"
                                    };

                                    results.Add(result);
                                   // _logger.LogInformation($"URL validation result: {url} - {(isValid ? "Valid" : "Invalid")} ({(int)response.StatusCode})");
                                }
                                catch (Exception ex)
                                {
                                    
                                    throw;
                                   
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Error opening document: {ex.Message}");
               // _logger.LogError($"Stack trace: {ex.StackTrace}");
                throw;
            }

            return results;
        }
    }
}
    

