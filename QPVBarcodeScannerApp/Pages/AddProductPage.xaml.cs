using QPVBarcodeScannerApp.Models;
using QPVBarcodeScannerApp.Services;

namespace QPVBarcodeScannerApp;

public partial class AddProductPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private string _barcode;
    private string? _photoPath;

    public AddProductPage(DatabaseService databaseService, string barcode)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _barcode = barcode;
        BarcodeEntry.Text = barcode;
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                
                if (photo != null)
                {
                    // Save the file to local storage
                    var localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    
                    using var sourceStream = await photo.OpenReadAsync();
                    using var localFileStream = File.OpenWrite(localFilePath);
                    await sourceStream.CopyToAsync(localFileStream);

                    _photoPath = localFilePath;
                    ProductPhoto.Source = ImageSource.FromFile(localFilePath);
                    ProductPhoto.IsVisible = true;
                    PhotoStatusLabel.Text = "Foto capturada correctamente";
                    PhotoStatusLabel.TextColor = Colors.Green;
                }
            }
            else
            {
                await DisplayAlert("Error", "La cámara no está disponible en este dispositivo", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al capturar foto: {ex.Message}", "OK");
        }
    }

    private async void OnSaveProductClicked(object sender, EventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
        {
            await DisplayAlert("Error", "Por favor ingrese una descripción", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(UnitEntry.Text))
        {
            await DisplayAlert("Error", "Por favor ingrese una unidad de medida", "OK");
            return;
        }

        if (!decimal.TryParse(CostEntry.Text, out decimal cost))
        {
            await DisplayAlert("Error", "Por favor ingrese un costo válido", "OK");
            return;
        }

        if (!decimal.TryParse(PriceEntry.Text, out decimal price))
        {
            await DisplayAlert("Error", "Por favor ingrese un precio válido", "OK");
            return;
        }

        if (!decimal.TryParse(QuantityEntry.Text, out decimal quantity))
        {
            await DisplayAlert("Error", "Por favor ingrese una cantidad válida", "OK");
            return;
        }

        // Create new product
        var product = new Product
        {
            Barcode = BarcodeEntry.Text,
            Description = DescriptionEntry.Text,
            UnitOfMeasure = UnitEntry.Text,
            Cost = cost,
            Price = price,
            Quantity = quantity,
            PhotoPath = _photoPath
        };

        // Save to database
        await _databaseService.SaveProductAsync(product);

        await DisplayAlert("Éxito", "Producto guardado correctamente", "OK");
        await Navigation.PopToRootAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
