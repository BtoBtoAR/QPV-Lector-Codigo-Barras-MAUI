# Guía de Construcción y Despliegue

## Requisitos Previos

### Software Necesario

1. **.NET 9.0 SDK**
   ```bash
   # Verificar instalación
   dotnet --version
   # Debe mostrar 9.0.x o superior
   ```

2. **Workload de MAUI**
   ```bash
   # Instalar workload
   dotnet workload install maui-android
   
   # Verificar instalación
   dotnet workload list
   ```

3. **Android SDK** (se instala con Visual Studio o Android Studio)
   - Android SDK 21 o superior
   - Android SDK Build-Tools
   - Android SDK Platform-Tools

4. **Visual Studio 2022** (Opcional pero recomendado)
   - Workload: "Desarrollo para dispositivos móviles con .NET"

## Configuración del Entorno

### Windows

1. Instalar Visual Studio 2022 con el workload de MAUI
2. Configurar variables de entorno:
   ```
   ANDROID_HOME=C:\Users\[Usuario]\AppData\Local\Android\Sdk
   JAVA_HOME=C:\Program Files\Java\jdk-17
   ```

### macOS

1. Instalar Xcode y command line tools (aunque esta app es solo Android)
2. Instalar .NET 9.0 SDK
3. Configurar ANDROID_HOME:
   ```bash
   export ANDROID_HOME=$HOME/Library/Android/sdk
   export PATH=$PATH:$ANDROID_HOME/platform-tools
   ```

### Linux

1. Instalar .NET 9.0 SDK
2. Instalar Android SDK manualmente o vía Android Studio
3. Configurar variables:
   ```bash
   export ANDROID_HOME=$HOME/Android/Sdk
   export PATH=$PATH:$ANDROID_HOME/platform-tools
   ```

## Construcción del Proyecto

### Restaurar Dependencias

```bash
cd QPVBarcodeScannerApp
dotnet restore
```

### Compilar en Modo Debug

```bash
dotnet build -f net9.0-android -c Debug
```

### Compilar en Modo Release

```bash
dotnet build -f net9.0-android -c Release
```

## Despliegue en Dispositivo

### Conectar Dispositivo Android

1. Habilitar "Opciones de desarrollador" en el dispositivo
2. Activar "Depuración USB"
3. Conectar vía USB
4. Verificar conexión:
   ```bash
   adb devices
   ```

### Instalar y Ejecutar

```bash
# Instalar en dispositivo conectado
dotnet build -f net9.0-android -t:Run

# O especificar configuración
dotnet build -f net9.0-android -c Debug -t:Run
```

### Instalar APK Manualmente

```bash
# Ubicar el APK generado
cd bin/Debug/net9.0-android/

# Instalar con adb
adb install com.companyname.qpvbarcodescannerapp-Signed.apk
```

## Generación de APK para Distribución

### APK Debug (para pruebas)

```bash
dotnet build -f net9.0-android -c Debug
```

APK ubicado en: `bin/Debug/net9.0-android/`

### APK Release (para producción)

#### 1. Crear Keystore (primera vez)

```bash
keytool -genkey -v -keystore qpvscanner.keystore -alias qpvscanner -keyalg RSA -keysize 2048 -validity 10000
```

Responder las preguntas:
- Contraseña del keystore
- Nombre y apellido
- Unidad organizativa
- Organización
- Ciudad
- Estado
- Código de país

#### 2. Configurar Firma en el Proyecto

Agregar al archivo `.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidKeyStore>true</AndroidKeyStore>
  <AndroidSigningKeyStore>qpvscanner.keystore</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>qpvscanner</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>TU_CONTRASEÑA</AndroidSigningKeyPass>
  <AndroidSigningStorePass>TU_CONTRASEÑA</AndroidSigningStorePass>
</PropertyGroup>
```

**IMPORTANTE**: No incluir contraseñas en el repositorio. Usar variables de entorno:

```xml
<AndroidSigningKeyPass>$(AndroidSigningKeyPass)</AndroidSigningKeyPass>
<AndroidSigningStorePass>$(AndroidSigningStorePass)</AndroidSigningStorePass>
```

```bash
# Configurar variables
export AndroidSigningKeyPass="tu_contraseña"
export AndroidSigningStorePass="tu_contraseña"
```

#### 3. Compilar Release

```bash
dotnet publish -f net9.0-android -c Release
```

APK firmado ubicado en: `bin/Release/net9.0-android/publish/`

## Generación de AAB (Android App Bundle)

Para Google Play Store, se requiere AAB en lugar de APK:

```bash
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=aab
```

AAB ubicado en: `bin/Release/net9.0-android/publish/`

## Emuladores

### Crear Emulador con Android Studio

1. Abrir Android Studio
2. Tools → Device Manager
3. Create Device
4. Seleccionar un dispositivo (ej: Pixel 5)
5. Seleccionar imagen del sistema (API 33 recomendado)
6. Finalizar creación

### Usar Emulador desde CLI

```bash
# Listar emuladores disponibles
emulator -list-avds

# Iniciar emulador
emulator -avd Pixel_5_API_33

# En otra terminal, instalar app
dotnet build -f net9.0-android -t:Run
```

## Optimizaciones de Compilación

### Reducir Tamaño del APK

Agregar al `.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidLinkMode>Full</AndroidLinkMode>
  <AndroidLinkSkip></AndroidLinkSkip>
  <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
  <AndroidUseAapt2>true</AndroidUseAapt2>
  <EnableProguard>true</EnableProguard>
</PropertyGroup>
```

### Mejorar Rendimiento

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidPackageFormat>aab</AndroidPackageFormat>
  <AndroidUseAapt2>true</AndroidUseAapt2>
  <AndroidCreatePackagePerAbi>true</AndroidCreatePackagePerAbi>
</PropertyGroup>
```

## Troubleshooting

### Error: "SDK not found"

**Solución**:
```bash
# Configurar ANDROID_HOME
export ANDROID_HOME=/path/to/android/sdk
export PATH=$PATH:$ANDROID_HOME/platform-tools
```

### Error: "Workload not installed"

**Solución**:
```bash
dotnet workload install maui-android
```

### Error: "Unable to connect to device"

**Solución**:
```bash
# Reiniciar servidor adb
adb kill-server
adb start-server
adb devices
```

### Error: "Build failed with code signing"

**Solución**:
- Verificar que el keystore existe
- Verificar contraseñas correctas
- Verificar alias correcto

### APK muy grande

**Solución**:
- Habilitar linking
- Crear APKs por ABI
- Usar AAB en lugar de APK universal

## CI/CD

### GitHub Actions (ejemplo)

```yaml
name: Build Android

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
    
    - name: Install MAUI
      run: dotnet workload install maui-android
    
    - name: Restore dependencies
      run: dotnet restore QPVBarcodeScannerApp/QPVBarcodeScannerApp.csproj
    
    - name: Build
      run: dotnet build QPVBarcodeScannerApp/QPVBarcodeScannerApp.csproj -f net9.0-android -c Release
    
    - name: Upload APK
      uses: actions/upload-artifact@v3
      with:
        name: android-apk
        path: QPVBarcodeScannerApp/bin/Release/net9.0-android/**/*.apk
```

## Publicación en Google Play Store

### 1. Preparar App

- Incrementar `ApplicationDisplayVersion` y `ApplicationVersion` en `.csproj`
- Compilar AAB en modo Release
- Probar exhaustivamente

### 2. Crear Cuenta de Desarrollador

- Registrarse en Google Play Console ($25 USD único pago)
- Completar perfil de desarrollador

### 3. Crear Nueva App

- Nombre de la aplicación
- Categoría
- Descripción
- Screenshots (mínimo 2)
- Icono (512x512 PNG)
- Feature graphic (1024x500 PNG)

### 4. Subir AAB

- Production → Create new release
- Upload AAB
- Completar release notes
- Submit for review

### 5. Esperar Aprobación

- Revisión puede tomar 1-7 días
- Revisar feedback si es rechazada

## Actualizaciones

### Versión Nueva

1. Incrementar versión en `.csproj`:
   ```xml
   <ApplicationDisplayVersion>1.1</ApplicationDisplayVersion>
   <ApplicationVersion>2</ApplicationVersion>
   ```

2. Compilar nuevo AAB/APK
3. Subir a Play Store (o distribuir APK)
4. Los usuarios recibirán actualización automática (Play Store)

## Recursos Adicionales

- [MAUI Android Deployment](https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/)
- [Android Developer Guide](https://developer.android.com/guide)
- [Google Play Console Help](https://support.google.com/googleplay/android-developer)
