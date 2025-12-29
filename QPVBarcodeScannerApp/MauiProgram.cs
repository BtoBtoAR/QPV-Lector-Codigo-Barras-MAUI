using Microsoft.Extensions.Logging;
using Camera.MAUI;
using BarcodeScanning;

namespace QPVBarcodeScannerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCameraView()
            .UseBarcodeScanning()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<App>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<ProductDetailPage>();
        builder.Services.AddTransient<AddProductPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
