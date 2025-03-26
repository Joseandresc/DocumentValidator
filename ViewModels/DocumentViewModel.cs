using DocumentFormat.OpenXml.Spreadsheet;
using DocumentValidator.Core.DocumentProcessor;
using DocumentValidator.Core.FormatValidatorProcessor;
using DocumentValidator.Core.ResultGenerator;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentValidator.ViewModels
{
    public class DocumentViewModel : INotifyPropertyChanged

    {
        #region Properties
        //Observable collection stores log messages that we bind to the UI
        public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand ValidateCommand { get; private set; }
        public ICommand SelectDocumentCommand { get; private set; }
        private bool _isProcessing;

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        //generate constructor
        public DocumentViewModel()
        {
            ValidateCommand = new Command<Stream>(async (documentStream) => await ValidateDocumentLinksAsync(documentStream));
            SelectDocumentCommand = new Command(async () => await OnSelectDocumentAsync());
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

                    //await ValidateDocumentLinksAsync(stream);
                    await ValidateTableFormatAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
            }
        }

        public void AddLogMessage(string message)
        {
            LogMessages.Add(message);
        }

        private async Task ValidateDocumentLinksAsync(Stream documentStream)
        {
            // Indicate that processing has started.
            IsProcessing = true;
            // Call the method to validate hyperlinks
            var linkValidator = new LinkValidator(this);
            var results = await linkValidator.ValidateDocumentLinks(documentStream);
            // Generate the results file
            var resultsFileGenerator = new ResultsFileGenerator();
            await resultsFileGenerator.GenerateWorkbookAsync(results);
            IsProcessing = false;
        }

        private async Task ValidateTableFormatAsync(Stream documentStream)
        {
            // Indicate that processing has started.
            IsProcessing = true;
             TableFormatValidator tableValidator = new TableFormatValidator();
             await tableValidator.TableFormatValidation(documentStream);
            IsProcessing = false;

        }
    }
}
