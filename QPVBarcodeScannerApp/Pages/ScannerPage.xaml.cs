using BarcodeScanning;
using QPVBarcodeScannerApp.Services;

namespace QPVBarcodeScannerApp;

public partial class ScannerPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private bool _isScanning = false;

    public ScannerPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        
        // Configure barcode scanning
        Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
        BarcodeScanner.OnDetected += OnBarcodeDetected;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isScanning = true;
        CameraView.CameraEnabled = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isScanning = false;
        CameraView.CameraEnabled = false;
    }

    private async void OnBarcodeDetected(object sender, OnDetectedEventArg e)
    {
        if (!_isScanning)
            return;

        _isScanning = false;

        var barcodeResults = e.BarcodeResults;
        if (barcodeResults != null && barcodeResults.Length > 0)
        {
            var barcode = barcodeResults[0].DisplayValue;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // Disable camera while processing
                CameraView.CameraEnabled = false;

                // Search for product in database
                var product = await _databaseService.GetProductByBarcodeAsync(barcode);

                if (product != null)
                {
                    // Product exists - show details
                    await Navigation.PushAsync(new ProductDetailPage(_databaseService, product));
                    await Navigation.RemovePage(this);
                }
                else
                {
                    // Product doesn't exist - ask if user wants to add it
                    bool addNew = await DisplayAlert(
                        "Producto no encontrado",
                        $"El código de barras '{barcode}' no existe en la base de datos. ¿Desea agregarlo?",
                        "Sí",
                        "No");

                    if (addNew)
                    {
                        await Navigation.PushAsync(new AddProductPage(_databaseService, barcode));
                        await Navigation.RemovePage(this);
                    }
                    else
                    {
                        // Go back to main page
                        await Navigation.PopAsync();
                    }
                }
            });
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
