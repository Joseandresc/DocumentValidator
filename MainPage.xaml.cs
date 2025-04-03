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
            AnimateIconTransition();
            BindingContext = new DocumentViewModel();
        }
        private async void AnimateIconTransition()
        {
            while (true)
            {
                // Fade out first image, fade in second
                await IconImage1.FadeTo(0, 1000);
                await IconImage2.FadeTo(1, 1000);

                // Pause before switching back

                // Fade back to first image
                await IconImage2.FadeTo(0, 1000);
                await IconImage1.FadeTo(1, 1000);

            }
        }




    }

}
