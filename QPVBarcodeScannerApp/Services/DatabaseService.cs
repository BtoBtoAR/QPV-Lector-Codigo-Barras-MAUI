using SQLite;
using QPVBarcodeScannerApp.Models;

namespace QPVBarcodeScannerApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public DatabaseService()
    {
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "products.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Product>();
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        await InitAsync();
        return await _database!.Table<Product>().ToListAsync();
    }

    public async Task<Product?> GetProductByBarcodeAsync(string barcode)
    {
        await InitAsync();
        return await _database!.Table<Product>()
            .Where(p => p.Barcode == barcode)
            .FirstOrDefaultAsync();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        await InitAsync();
        return await _database!.FindAsync<Product>(id);
    }

    public async Task<int> SaveProductAsync(Product product)
    {
        await InitAsync();
        product.UpdatedAt = DateTime.Now;
        
        if (product.Id != 0)
        {
            return await _database!.UpdateAsync(product);
        }
        else
        {
            product.CreatedAt = DateTime.Now;
            return await _database!.InsertAsync(product);
        }
    }

    public async Task<int> DeleteProductAsync(Product product)
    {
        await InitAsync();
        return await _database!.DeleteAsync(product);
    }
}
