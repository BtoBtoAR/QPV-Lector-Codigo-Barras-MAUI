using QPVBarcodeScannerApp.Models;
using QPVBarcodeScannerApp.Services;

namespace QPVBarcodeScannerApp;

public partial class ProductDetailPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private Product _product;

    public ProductDetailPage(DatabaseService databaseService, Product product)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _product = product;
        LoadProductData();
    }

    private void LoadProductData()
    {
        // Display product photo if exists
        if (!string.IsNullOrEmpty(_product.PhotoPath) && File.Exists(_product.PhotoPath))
        {
            ProductPhoto.Source = ImageSource.FromFile(_product.PhotoPath);
            ProductPhoto.IsVisible = true;
        }
        else
        {
            ProductPhoto.IsVisible = false;
        }

        BarcodeLabel.Text = _product.Barcode;
        DescriptionLabel.Text = _product.Description;
        UnitLabel.Text = _product.UnitOfMeasure;
        CostLabel.Text = $"${_product.Cost:F2}";
        PriceLabel.Text = $"${_product.Price:F2}";
        QuantityEntry.Text = _product.Quantity.ToString();
    }

    private async void OnUpdateQuantityClicked(object sender, EventArgs e)
    {
        if (decimal.TryParse(QuantityEntry.Text, out decimal newQuantity))
        {
            _product.Quantity = newQuantity;
            await _databaseService.SaveProductAsync(_product);
            await DisplayAlert("Éxito", "Cantidad actualizada correctamente", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Por favor ingrese una cantidad válida", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
