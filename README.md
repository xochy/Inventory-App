# Sistema de Gestión de Inventario

## 📋 Información General

**Versión:** v1.0.0 (Beta)  
**Fecha de Entrega:** 14 de Julio de 2025  
**Tecnología:** WPF .NET 8.0 + SQL Server 2022

## 🎯 Descripción del Proyecto

Sistema de gestión de inventario desarrollado como prueba técnica, que permite el registro y control de artículos, tallas, stock y movimientos de entrada y salida, con un sistema básico de registro de usuarios (empleados, grupos y tipos de empleado).

### ✨ Características Principales

- **Gestión CRUD de Empleados**: Creación, lectura, actualización y eliminación de registros de empleados con asignación a grupos
- **Gestión CRUD de Productos**: Manejo completo de productos con tipos de aplicabilidad (Administrativo, Sindicalizado, Todos)
- **Gestión CRUD de Tallas**: Control de tallas con diferentes notaciones (Americana, Mexicana, Europea, Única)
- **Gestión CRUD de Stock**: Administración de inventario por combinación producto-talla con stock mínimo
- **Registro de Entregas**: Funcionalidad para registrar salidas de inventario con validaciones automáticas
- **Historial de Movimientos**: Visualización completa de entradas y salidas del almacén

## 🛠️ Requisitos Técnicos

### Hardware Mínimo

- **CPU**: Procesador de doble núcleo o superior
- **RAM**: 4 GB (8 GB recomendados)
- **Almacenamiento**: 5 GB de espacio libre (SSD recomendado)

### Software Requerido

- **Sistema Operativo**: Windows 10/11 (64-bit)
- **Base de Datos**: SQL Server 2022 Express Edition
- **Framework**: .NET 8.0 SDK
- **IDE**: Visual Studio 2022 Community Edition
- **Herramientas**: SQL Server Management Studio (SSMS) v19+

### Dependencias NuGet

```xml
<PackageReference Include="Microsoft.Data.SqlClient" />
<PackageReference Include="Moq" /> <!-- Para testing -->
<PackageReference Include="xunit" /> <!-- Para testing -->
<PackageReference Include="xunit.runner.visualstudio" /> <!-- Para testing -->
```

## 🚀 Instalación y Configuración

### 1. Instalación de Prerrequisitos

1. **Instalar SQL Server 2022 Express Edition**

   - Descargar desde el sitio oficial de Microsoft
   - Seleccionar instalación "Básica" o "Personalizada"
   - Recordar el nombre de la instancia (por defecto: `SQLEXPRESS`)

2. **Instalar SQL Server Management Studio (SSMS)**

   - Descargar desde el sitio oficial de Microsoft
   - Ejecutar instalador y seguir instrucciones

3. **Instalar Visual Studio 2022 Community**
   - Descargar desde el sitio oficial de Microsoft
   - Seleccionar carga de trabajo "Desarrollo de escritorio con .NET"

### 2. Configuración de Base de Datos

1. **Conectar a SQL Server**

   ```
   Server name: . o (local)\SQLEXPRESS
   Authentication: Windows Authentication
   ```

2. **Crear Base de Datos**

   - Nombre: `InventoryTechnicalTestDB`

3. **Ejecutar Scripts SQL**
   - Ejecutar scripts en el siguiente orden:
     - `InventoryTablesDB.sql`
     - `InventoryRegisterDeliverySP.sql`
     - `InventoryWerehouseMovementsVW.sql`
     - `InventoryDummyDataDB.sql`

### 3. Configuración de la Aplicación

1. **Abrir Proyecto**

   ```bash
   # Abrir en Visual Studio 2022
   InventoryWpfAppSolution.sln
   ```

2. **Restaurar Paquetes NuGet**

   - Click derecho en la solución → "Restore NuGet Packages"

3. **Actualizar Cadena de Conexión**

   ```csharp
   // En Repositories/Helpers/DbConnectionFactory.cs
   private readonly string _connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=InventarioPruebaTecnicaDB;Integrated Security=True;TrustServerCertificate=True";
   ```

4. **Verificar Estilos**

   - Asegurar que `Styles.xaml` esté en `Themes/`
   - Verificar referencia en `App.xaml`:

   ```xml
   <Application.Resources>
       <ResourceDictionary>
           <ResourceDictionary.MergedDictionaries>
               <ResourceDictionary Source="Themes/Styles.xaml"/>
           </ResourceDictionary.MergedDictionaries>
       </ResourceDictionary>
   </Application.Resources>
   ```

5. **Compilar Solución**
   - Build → Rebuild Solution

## 📁 Estructura del Proyecto

```
InventoryWpfAppSolution/
├── InventoryWpfApp/                    # Proyecto WPF principal
│   ├── Models/                         # Entidades de datos
│   ├── Repositories/
│   │   ├── Contracts/                  # Interfaces
│   │   ├── Implementations/            # Implementaciones
│   │   └── Helpers/                    # Fábrica de conexión
│   ├── ViewModels/
│   │   ├── Base/                       # BaseViewModel, MessageType
│   │   ├── Commands/                   # RelayCommand
│   │   └── Implementations/            # ViewModels específicos
│   ├── Views/
│   │   ├── Shared/                     # MainWindow.xaml
│   │   └── ...                         # UserControls por módulo
│   ├── Converters/                     # Convertidores XAML
│   ├── Themes/                         # Estilos globales
│   │   └── Styles.xaml
│   └── App.xaml / App.xaml.cs
├── InventoryWpfApp.Tests/              # Proyecto de pruebas
│   ├── ViewModels/                     # Tests de ViewModels
│   └── Repositories/                   # Tests de Repositorios
├── SQL_Scripts/                        # Scripts de base de datos
├── DatabaseBackup/                     # Respaldo de BD
└── InventoryWpfAppSolution.sln
```

## 💻 Uso de la Aplicación

### Inicio

1. Ejecutar `InventoryWpfApp.exe` desde `bin/Debug/net8.0-windows/`
2. La aplicación se abrirá con una interfaz de pestañas

### Módulos Disponibles

- **Employees**: Gestión de empleados con grupos y tipos
- **Products**: Administración de productos de inventario
- **Sizes**: Configuración de tallas y notaciones
- **Inventory Stock**: Control de stock por producto-talla
- **Deliveries**: Registro de entregas a empleados
- **Movement History**: Historial completo de movimientos

### Características de UI/UX

- **Diseño**: Minimalista basado en Microsoft Fluent Design
- **Colores**: Verde claro (#A8BCB2) y verde oliva (#84978E)
- **Tipografía**: Segoe UI con jerarquía clara
- **Interacciones**: Efectos sutiles de hover y focus
- **Iconografía**: Segoe MDL2 Assets integrada

## 🧪 Pruebas

### Tipos de Pruebas Realizadas

- **Pruebas Unitarias**: xUnit.net + Moq para ViewModels y Repositorios
- **Pruebas de Integración**: Verificación manual de interacción con BD
- **Pruebas Funcionales**: Validación completa de flujos de trabajo
- **Pruebas de UI/UX**: Verificación de diseño y usabilidad

### Ejecutar Pruebas

```bash
# Desde Visual Studio
Test → Run All Tests

# Desde línea de comandos
dotnet test
```

## 🔧 Arquitectura

- **Patrón**: MVVM (Model-View-ViewModel)
- **Repositorios**: Abstracción de acceso a datos
- **Inyección de Dependencias**: Para mejor testabilidad
- **Separación de Responsabilidades**: Capas bien definidas

## 📝 Notas de la Versión v1.0.0 (Beta)

### Nuevas Características

- Implementación completa CRUD para todos los módulos
- Sistema de entregas con validaciones automáticas
- Historial de movimientos en tiempo real
- Diseño UI/UX moderno y responsivo

### Mejoras Técnicas

- Arquitectura MVVM refactorizada
- Repositorios con interfaces bien definidas
- Cobertura de pruebas unitarias
- Manejo robusto de errores

### Problemas Conocidos

- Ninguno crítico identificado en esta versión
- Se recomienda testing exhaustivo en ambiente de producción

## 🆘 Soporte

Para consultas, problemas o solicitudes de soporte:

- **Contacto**: chavez.ayala.dev@gmail.com
- **Documentación**: Este README y comentarios en código
- **Actualizaciones**: Se comunicarán a través de canales habituales

## 📄 Licencia

Este software se entrega bajo los términos de licencia MIT.

### Dependencias con Licencia

- **.NET 8.0 SDK**: Licencia MIT
- **SQL Server 2022 Express**: Microsoft EULA
- **xUnit.net**: Licencia Apache 2.0
- **Moq**: Licencia MIT

## 🔄 Historial de Versiones

### v1.0.0 (Beta) - 14 de Julio de 2025

- Entrega inicial con funcionalidades CRUD completas
- Implementación de módulos de entregas e historial
- Arquitectura MVVM con repositorios
- Diseño UI/UX minimalista implementado
- Preparado para pruebas unitarias

---

**Desarrollado como Prueba Técnica - Sistema de Gestión de Inventario**
