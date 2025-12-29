using SQLite;

namespace QPVBarcodeScannerApp.Models;

[Table("products")]
public class Product
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, MaxLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public decimal Price { get; set; }

    public decimal Quantity { get; set; }

    // Store image as base64 string or file path
    public string? PhotoPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
