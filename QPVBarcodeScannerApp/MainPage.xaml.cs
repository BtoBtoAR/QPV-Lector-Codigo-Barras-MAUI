using QPVBarcodeScannerApp.Services;

namespace QPVBarcodeScannerApp;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MainPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    private async void OnScanBarcodeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ScannerPage(_databaseService));
    }

    private async void OnViewProductsClicked(object sender, EventArgs e)
    {
        var products = await _databaseService.GetProductsAsync();
        await DisplayAlert("Productos", $"Total de productos: {products.Count}", "OK");
    }
}
