using DocumentValidator.Core.DocumentProcessor;
using DocumentValidator.Core.FormatValidatorProcessor;
using DocumentValidator.Core.ResultGenerator;
using DocumentValidator.Core.SemanticValidatorProcessor;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DocumentValidator.ViewModels
{
    public class DocumentViewModel : INotifyPropertyChanged

    {
        #region Properties
        //Observable collection stores log messages that we bind to the UI
        public ObservableCollection<string> LogMessages { get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IConfiguration _configuration;
        public ICommand ValidateCommand { get; private set; }
        public ICommand SelectDocumentCommand { get; private set; }
        private bool _isProcessing;
        private readonly SemanticValidatorProcessor _semanticValidatorProcessor;
        private bool _isImageVisible = true;
        private List<LinkValidationResult> _linkValidationResult;
        public bool IsImageVisible
        {
            get => _isImageVisible;
            set
            {
                _isImageVisible = value;
                OnPropertyChanged(nameof(IsImageVisible));
            }
        }

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
        private bool _isAIValidationEnabled;
        public bool IsAIValidationEnabled
        {
            get => _isAIValidationEnabled;
            set
            {
                if (_isAIValidationEnabled != value)
                {
                    _isAIValidationEnabled = value;
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
    
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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

                    await ValidateDocumentLinksAsync(stream);
                    if(IsAIValidationEnabled)
                    {
                        
                        //await ValidateSemanticDocumentAsync(stream);
                    }
                 await HTMLResultsGenerator.GenerateHtmlReportAsync(_linkValidationResult);
                    //await ValidateTableFormatAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                LogMessages.Add($"Error: {ex.Message}");
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
            IsImageVisible = false;  // Hide when validation starts

            // Call the method to validate hyperlinks
            var linkValidator = new LinkValidator(this);
            _linkValidationResult = await linkValidator.ValidateDocumentLinks(documentStream);
            // Generate the results file
            var resultsFileGenerator = new ResultsFileGenerator();
            //await resultsFileGenerator.GenerateWorkbookAsync(results);
            
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

        private async Task ValidateSemanticDocumentAsync(Stream documentStream)
        {
            IsProcessing = true;
            SemanticValidatorProcessor semanticValidator = new SemanticValidatorProcessor(this);
            await semanticValidator.EvaluateDocumentAsync(documentStream);
            IsProcessing = false;
        }
    }
}
