using DocumentValidator.Core.DocumentProcessor;
using DocumentValidator.ViewModels;
using Microsoft.Extensions.Logging;

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
            
            // Configure logging
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            builder.Logging.AddDebug();
          
            


            return builder.Build();
        }
    }
}
