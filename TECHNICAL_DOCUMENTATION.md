# Documentación Técnica - QPV Barcode Scanner

## Arquitectura de la Aplicación

### Patrón de Diseño

La aplicación utiliza una arquitectura basada en:
- **MVVM (Model-View-ViewModel)** implícito a través de code-behind
- **Repository Pattern** para acceso a datos (DatabaseService)
- **Dependency Injection** configurado en MauiProgram.cs

### Flujo de Navegación

```
MainPage
    │
    ├─→ ScannerPage (Escaneo de código de barras)
    │       │
    │       ├─→ ProductDetailPage (Si el producto existe)
    │       │
    │       └─→ AddProductPage (Si el producto no existe)
    │
    └─→ [Futuro: ProductListPage]
```

## Componentes Principales

### 1. Models/Product.cs

Modelo de datos que representa un producto en la base de datos.

**Propiedades:**
- `Id`: Identificador único autoincrementable
- `Barcode`: Código de barras del producto (indexado para búsquedas rápidas)
- `Description`: Descripción del producto (máx. 500 caracteres)
- `UnitOfMeasure`: Unidad de medida (máx. 50 caracteres)
- `Cost`: Costo del producto (decimal)
- `Price`: Precio de venta (decimal)
- `Quantity`: Cantidad en existencia (decimal)
- `PhotoPath`: Ruta del archivo de la foto
- `CreatedAt`: Fecha de creación
- `UpdatedAt`: Fecha de última actualización

### 2. Services/DatabaseService.cs

Servicio singleton para operaciones CRUD en SQLite.

**Métodos:**
- `InitAsync()`: Inicializa la base de datos y crea las tablas
- `GetProductsAsync()`: Obtiene todos los productos
- `GetProductByBarcodeAsync(string barcode)`: Busca un producto por código de barras
- `GetProductAsync(int id)`: Obtiene un producto por ID
- `SaveProductAsync(Product product)`: Guarda o actualiza un producto
- `DeleteProductAsync(Product product)`: Elimina un producto

**Inicialización Lazy:**
La base de datos se inicializa solo cuando se accede por primera vez, mejorando el rendimiento inicial.

### 3. Pages/ScannerPage

Página de escaneo de códigos de barras.

**Características:**
- Utiliza `Camera.MAUI` para acceso a la cámara
- `BarcodeScanning.Native.Maui` para detección de códigos
- Soporta todos los formatos de códigos de barras comunes
- Deshabilita la cámara durante el procesamiento para evitar múltiples detecciones
- Maneja permisos de cámara automáticamente

**Ciclo de Vida:**
- `OnAppearing()`: Activa la cámara
- `OnDisappearing()`: Desactiva la cámara para ahorrar batería
- `OnBarcodeDetected()`: Procesa el código detectado y navega según resultado

### 4. Pages/ProductDetailPage

Muestra la información completa de un producto existente.

**Características:**
- Visualización de foto del producto
- Muestra todos los datos del producto
- Permite actualizar solo la cantidad en existencia
- Validación de entrada numérica
- Botón para volver al inicio

### 5. Pages/AddProductPage

Formulario para agregar nuevos productos.

**Características:**
- Captura de foto usando `MediaPicker`
- Código de barras prellenado (solo lectura)
- Validación de todos los campos obligatorios
- Guardado en la base de datos
- Navegación automática al inicio después de guardar

**Validaciones:**
- Descripción: requerida
- Unidad de medida: requerida
- Costo: numérico válido
- Precio: numérico válido
- Cantidad: numérico válido
- Foto: opcional

### 6. MauiProgram.cs

Configuración de la aplicación y servicios.

**Servicios Registrados:**
- `DatabaseService`: Singleton para acceso a datos
- `MainPage`, `ScannerPage`, `ProductDetailPage`, `AddProductPage`: Transient

**Extensiones de MAUI:**
- `UseMauiCameraView()`: Habilita el control de cámara
- `UseBarcodeScanning()`: Habilita el escaneo de códigos de barras

## Gestión de Recursos

### Imágenes

- **Ubicación**: `Resources/Images/`
- **Formato**: SVG (escalable) y formatos de mapa de bits
- Las fotos de productos se guardan en `FileSystem.AppDataDirectory`

### Fuentes

- **OpenSans-Regular.ttf**: Fuente principal
- **OpenSans-Semibold.ttf**: Fuente para títulos

### Estilos

- **Colors.xaml**: Paleta de colores de la aplicación
- **Styles.xaml**: Estilos para todos los controles MAUI

## Permisos y Seguridad

### Permisos Necesarios (Android)

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_MEDIA_IMAGES" />
```

### Manejo de Permisos

Los permisos se solicitan automáticamente en tiempo de ejecución cuando:
- Se accede a `ScannerPage` (permisos de cámara)
- Se captura una foto en `AddProductPage` (permisos de cámara y almacenamiento)

## Base de Datos SQLite

### Esquema

```sql
CREATE TABLE products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Barcode TEXT NOT NULL,
    Description TEXT,
    UnitOfMeasure TEXT,
    Cost REAL,
    Price REAL,
    Quantity REAL,
    PhotoPath TEXT,
    CreatedAt TEXT,
    UpdatedAt TEXT
);

CREATE INDEX idx_barcode ON products(Barcode);
```

### Ubicación

- **Android**: `/data/data/com.companyname.qpvbarcodescannerapp/files/products.db3`

### Operaciones Asíncronas

Todas las operaciones de base de datos son asíncronas usando `async/await` para:
- No bloquear el UI thread
- Mejor rendimiento y responsividad
- Prevenir ANR (Application Not Responding) en Android

## Optimizaciones

### Rendimiento

1. **Lazy Loading**: La base de datos se inicializa solo cuando es necesaria
2. **Índices**: El campo `Barcode` está indexado para búsquedas rápidas
3. **Operaciones Asíncronas**: Todas las operaciones I/O son asíncronas
4. **Gestión de Cámara**: La cámara se activa/desactiva según la visibilidad de la página

### Memoria

1. **Imágenes**: Se almacenan como archivos, no en la base de datos
2. **Disposición de Recursos**: Los recursos de cámara se liberan correctamente
3. **Navegación**: Páginas anteriores se eliminan de la pila de navegación cuando corresponde

## Extensibilidad

### Agregar Nuevos Formatos de Código

Modificar en `ScannerPage.xaml.cs`:
```csharp
Methods.SetSupportBarcodeFormat(BarcodeFormats.Code128 | BarcodeFormats.QrCode);
```

### Agregar Campos al Producto

1. Agregar propiedad en `Models/Product.cs`
2. Agregar atributo `[MaxLength]` si es texto
3. Actualizar UI en `ProductDetailPage.xaml` y `AddProductPage.xaml`
4. SQLite detectará y agregará la columna automáticamente

### Exportar/Importar Datos

Agregar métodos en `DatabaseService`:
```csharp
public async Task<string> ExportToJson()
{
    var products = await GetProductsAsync();
    return JsonSerializer.Serialize(products);
}

public async Task ImportFromJson(string json)
{
    var products = JsonSerializer.Deserialize<List<Product>>(json);
    foreach (var product in products)
        await SaveProductAsync(product);
}
```

## Testing

### Pruebas Recomendadas

1. **Escaneo de Códigos**:
   - Códigos de barras de diferentes formatos (EAN-13, UPC, Code 128)
   - Códigos QR
   - Condiciones de iluminación variadas

2. **Base de Datos**:
   - Inserción de productos
   - Búsqueda de productos existentes
   - Actualización de cantidades
   - Productos con y sin foto

3. **Interfaz de Usuario**:
   - Rotación de pantalla
   - Navegación entre páginas
   - Validación de formularios
   - Manejo de permisos denegados

4. **Rendimiento**:
   - Base de datos con 1000+ productos
   - Búsquedas rápidas por código de barras
   - Carga de imágenes grandes

## Solución de Problemas Comunes

### Error: "Camera permission not granted"

**Solución**: Verificar que los permisos estén declarados en `AndroidManifest.xml` y que el usuario los haya aceptado.

### Error: "Database is locked"

**Solución**: Asegurarse de que solo hay una instancia de `DatabaseService` (singleton).

### Códigos de barras no se detectan

**Solución**: 
- Verificar formato de código soportado
- Mejorar iluminación
- Aumentar distancia entre cámara y código

### Imágenes no se muestran

**Solución**: 
- Verificar que `PhotoPath` tiene una ruta válida
- Verificar permisos de lectura de archivos
- Verificar que el archivo existe en la ubicación especificada

## Mejoras Futuras

1. **Búsqueda y Filtrado**: Lista de productos con búsqueda
2. **Sincronización**: Sincronizar con servidor remoto
3. **Exportación**: Exportar a CSV/Excel
4. **Estadísticas**: Reportes de inventario
5. **Categorías**: Organizar productos por categorías
6. **Código QR**: Generar códigos QR para compartir productos
7. **Multi-idioma**: Soporte para inglés y otros idiomas
8. **Tema Oscuro**: Modo oscuro/claro
9. **Backup**: Respaldo automático de base de datos
10. **Escaneo Múltiple**: Escanear varios productos consecutivamente

## Referencias

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [SQLite-net Documentation](https://github.com/praeclarum/sqlite-net)
- [Camera.MAUI](https://github.com/hjam40/Camera.MAUI)
- [BarcodeScanning.Native.Maui](https://github.com/afriscic/BarcodeScanning.Native.Maui)
