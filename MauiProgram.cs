using DocumentValidator.Core.DocumentProcessor;
using DocumentValidator.Core.SemanticValidatorProcessor;
using DocumentValidator.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace DocumentValidator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<DocumentViewModel>();
            builder.Services.AddTransient<LinkValidator>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<SemanticValidatorProcessor>();  // Register your processor as a transient service

            // Configure logging
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            builder.Logging.AddDebug();
          
            


            return builder.Build();
        }
    }
}
