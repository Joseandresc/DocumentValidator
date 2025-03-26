using DocumentValidator.Core.DocumentProcessor;
using DocumentValidator.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentValidator


{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage( )
        {
            InitializeComponent();
            BindingContext = new DocumentViewModel();
        }



        
       
    }

}
