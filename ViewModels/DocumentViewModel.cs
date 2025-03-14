using DocumentFormat.OpenXml.Spreadsheet;
using DocumentValidator.Core.DocumentProcessor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentValidator.ViewModels
{
    internal class DocumentViewModel 
    {
        
        private string _validationResult;
        public ICommand ValidateCommand { get; private set; }
        public ICommand SelectDocumentCommand { get; private set; }

        
        //generate constructor
        public DocumentViewModel()
        {
            ValidateCommand = new Command<Stream>(async (documentStream) => await ValidateDocumentLinksAsync(documentStream));
            SelectDocumentCommand = new Command(async () => await OnSelectDocumentAsync());
        }
        private async Task OnSelectDocumentAsync()
        {

            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".docx" } }
            });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select a file",
                    FileTypes = customFileType
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                   
                    await ValidateDocumentLinksAsync(stream);

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }



        private async Task ValidateDocumentLinksAsync(Stream documentStream)
        {
            // Call the method to validate hyperlinks
             var linkValidator = new LinkValidator();
            await linkValidator.ValidateDocumentLinks(documentStream);
        }
    }
}
