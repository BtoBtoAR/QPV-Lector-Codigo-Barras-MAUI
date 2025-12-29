# QPV Lector Código de Barras - MAUI

Aplicación Android desarrollada con .NET MAUI y C# que permite escanear códigos de barras de productos, buscarlos en una base de datos local SQLite, y gestionar su información.

## Características

- ✅ **Escaneo de Códigos de Barras**: Utiliza la cámara del dispositivo para leer códigos de barras
- ✅ **Base de Datos Local**: Almacena productos en SQLite con toda su información
- ✅ **Captura de Fotografías**: Permite tomar fotos de productos nuevos
- ✅ **Gestión Completa de Productos**: 
  - Código de barras
  - Fotografía del producto
  - Descripción
  - Unidad de medida
  - Costo
  - Precio
  - Cantidad en existencia

## Requisitos del Sistema

- Visual Studio 2022 (17.8 o superior) o Visual Studio Code
- .NET 9.0 SDK
- Workload de .NET MAUI para Android
- Dispositivo o emulador Android (API nivel 21 o superior)

## Instalación

### 1. Instalar .NET 9.0 SDK

Descarga e instala desde: https://dotnet.microsoft.com/download

### 2. Instalar el Workload de MAUI

```bash
dotnet workload install maui-android
```

### 3. Clonar el Repositorio

```bash
git clone https://github.com/BtoBtoAR/QPV-Lector-Codigo-Barras-MAUI.git
cd QPV-Lector-Codigo-Barras-MAUI
```

### 4. Restaurar Dependencias

```bash
cd QPVBarcodeScannerApp
dotnet restore
```

## Compilación y Ejecución

### Desde Visual Studio 2022

1. Abre el archivo `QPVBarcodeScannerApp.csproj` en Visual Studio
2. Selecciona un dispositivo Android o emulador en el menú desplegable
3. Presiona F5 o haz clic en el botón "Ejecutar"

### Desde la Línea de Comandos

```bash
cd QPVBarcodeScannerApp
dotnet build -f net9.0-android
```

Para instalar en un dispositivo conectado:

```bash
dotnet build -f net9.0-android -t:Run
```

## Estructura del Proyecto

```
QPVBarcodeScannerApp/
├── Models/
│   └── Product.cs                    # Modelo de datos del producto
├── Services/
│   └── DatabaseService.cs            # Servicio de acceso a SQLite
├── Pages/
│   ├── ScannerPage.xaml             # Página de escaneo de códigos de barras
│   ├── ProductDetailPage.xaml       # Página de visualización de producto
│   └── AddProductPage.xaml          # Página de agregar nuevo producto
├── Platforms/Android/
│   ├── AndroidManifest.xml          # Permisos y configuración de Android
│   ├── MainActivity.cs              # Actividad principal
│   └── MainApplication.cs           # Aplicación Android
├── Resources/
│   ├── Styles/                      # Estilos y colores
│   ├── Fonts/                       # Fuentes
│   ├── Images/                      # Imágenes
│   ├── AppIcon/                     # Icono de la aplicación
│   └── Splash/                      # Pantalla de inicio
├── App.xaml                         # Aplicación MAUI
├── MainPage.xaml                    # Página principal
└── MauiProgram.cs                   # Configuración de la aplicación
```

## Uso de la Aplicación

### 1. Pantalla Principal

Al abrir la aplicación, verás dos opciones:
- **Escanear Código de Barras**: Inicia el proceso de escaneo
- **Ver Productos**: Muestra el total de productos en la base de datos

### 2. Escanear Código de Barras

1. Toca "Escanear Código de Barras"
2. Otorga permisos de cámara si es necesario
3. Apunta la cámara al código de barras del producto
4. La app detectará automáticamente el código

### 3. Producto Encontrado

Si el código de barras existe en la base de datos:
- Se muestra la fotografía del producto
- Código de barras
- Descripción completa
- Unidad de medida
- Costo y precio
- Campo editable para actualizar la cantidad en existencia

### 4. Producto No Encontrado

Si el código de barras no existe:
1. Se pregunta si deseas agregar el producto
2. Si aceptas, puedes:
   - Tomar una fotografía del producto
   - Ingresar descripción
   - Definir unidad de medida
   - Establecer costo y precio
   - Ingresar cantidad inicial
3. Guardar el producto en la base de datos

## Dependencias

El proyecto utiliza los siguientes paquetes NuGet:

- **Microsoft.Maui.Controls** (9.0.0) - Framework principal de MAUI
- **sqlite-net-pcl** (1.9.172) - ORM para SQLite
- **SQLitePCLRaw.bundle_green** (2.1.10) - Provider de SQLite
- **Camera.MAUI** (1.5.1) - Control de cámara para MAUI
- **BarcodeScanning.Native.Maui** (1.5.8) - Librería de escaneo de códigos de barras

## Permisos de Android

La aplicación requiere los siguientes permisos (configurados en AndroidManifest.xml):

- `CAMERA` - Para escanear códigos de barras y tomar fotos
- `WRITE_EXTERNAL_STORAGE` - Para guardar fotos
- `READ_EXTERNAL_STORAGE` - Para leer fotos guardadas
- `READ_MEDIA_IMAGES` - Para acceder a imágenes (Android 13+)

## Base de Datos

Los datos se almacenan localmente en SQLite en:
```
{AppDataDirectory}/products.db3
```

Estructura de la tabla `products`:
- `Id` (INTEGER, PRIMARY KEY, AUTOINCREMENT)
- `Barcode` (TEXT, INDEXED)
- `Description` (TEXT)
- `UnitOfMeasure` (TEXT)
- `Cost` (REAL)
- `Price` (REAL)
- `Quantity` (REAL)
- `PhotoPath` (TEXT)
- `CreatedAt` (TEXT)
- `UpdatedAt` (TEXT)

## Solución de Problemas

### La cámara no funciona

1. Verifica que el dispositivo tenga cámara
2. Confirma que los permisos fueron otorgados
3. Reinicia la aplicación

### No se detectan códigos de barras

1. Asegúrate de tener buena iluminación
2. Mantén el código de barras dentro del marco de la cámara
3. Limpia la lente de la cámara

### Error al restaurar paquetes

```bash
dotnet workload restore
dotnet restore --force
```

### Error al compilar

Limpia y reconstruye:
```bash
dotnet clean
dotnet build -f net9.0-android
```

## Contribuir

Las contribuciones son bienvenidas. Por favor:

1. Haz un fork del repositorio
2. Crea una rama para tu característica (`git checkout -b feature/nueva-caracteristica`)
3. Commit tus cambios (`git commit -am 'Agregar nueva característica'`)
4. Push a la rama (`git push origin feature/nueva-caracteristica`)
5. Crea un Pull Request

## Licencia

Este proyecto está disponible para uso según los términos que defina el propietario del repositorio.

## Contacto

Para reportar problemas o sugerencias, por favor abre un issue en el repositorio de GitHub.
