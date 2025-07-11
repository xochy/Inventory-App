# Sistema de Gestión de Inventario

> **Prueba Técnica - Sistema de Gestión de Inventario WPF**

Este repositorio contiene el código fuente y la documentación para el "Sistema de Gestión de Inventario", desarrollado como parte de una prueba técnica utilizando tecnologías modernas de .NET.

## 🚀 Características Principales

Esta aplicación de escritorio WPF implementa un sistema completo de gestión de inventario con las siguientes funcionalidades:

### Módulos Principales
- **Gestión de Empleados (CRUD)**: Creación, lectura, actualización y eliminación de registros de empleados, incluyendo asignación a grupos
- **Gestión de Productos (CRUD)**: Control completo sobre productos del inventario con detalles como nombre, descripción y tipo de aplicabilidad (Administrativo, Sindicalizado, Todos)
- **Gestión de Tallas (CRUD)**: Administración de diferentes tallas y sus notaciones (Americana, Mexicana, Europea, Única)
- **Gestión de Stock de Inventario (CRUD)**: Control detallado del stock por cada combinación de producto y talla, incluyendo cantidad actual y stock mínimo
- **Registro de Entregas de Inventario**: Funcionalidad para registrar la salida de artículos del almacén a empleados, con validaciones de aplicabilidad y disponibilidad de stock
- **Historial de Movimientos**: Visualización completa de todos los movimientos de entrada y salida del inventario

## 🛠 Tecnologías Utilizadas

| Categoría | Tecnología |
|-----------|------------|
| **Lenguaje** | C# |
| **Framework** | .NET 8.0 |
| **Interfaz de Usuario** | WPF (Windows Presentation Foundation) |
| **Base de Datos** | SQL Server 2022 Express Edition |
| **Patrones de Diseño** | MVVM, Repositorio, Inyección de Dependencias |
| **Testing** | xUnit.net, Moq |

## 📋 Requisitos del Sistema

### Hardware Mínimo
- **CPU**: Procesador de doble núcleo o superior
- **RAM**: 4 GB de RAM o más
- **Almacenamiento**: 5 GB de espacio libre en disco

### Software Requerido
- **Sistema Operativo**: Windows 10 / 11 (64-bit)
- **Base de Datos**: SQL Server 2022 Express Edition
- **Herramienta de BD**: SQL Server Management Studio (SSMS) v19 o superior
- **IDE**: Visual Studio 2022 Community Edition (o superior)
- **Framework**: .NET 8.0 SDK

## 🚀 Guía de Instalación y Configuración

### Paso 1: Instalación de Herramientas Previas

Asegúrate de tener instaladas las siguientes herramientas:

1. **SQL Server 2022 Express Edition**
   - Descarga e instala la versión Express
   - Recuerda el nombre de la instancia (por defecto `SQLEXPRESS`)

2. **SQL Server Management Studio (SSMS)**
   - Herramienta para gestionar tu base de datos SQL Server

3. **Visual Studio 2022 Community Edition**
   - Durante la instalación, selecciona la carga de trabajo "Desarrollo de escritorio con .NET"

### Paso 2: Configuración de la Base de Datos

1. **Conectar a SQL Server**
   ```
   Servidor: .\SQLEXPRESS o (local)\SQLEXPRESS
   Autenticación: Windows Authentication
   ```

2. **Crear la Base de Datos**
   - En el "Object Explorer", clic derecho en "Databases"
   - Selecciona "New Database..."
   - Nombra la base de datos: `InventarioPruebaTecnicaDB`

3. **Ejecutar Scripts SQL**
   - Abre una "New Query" en SSMS
   - Ejecuta los scripts SQL de la carpeta `SQL_Scripts/`:
     - Scripts de creación de tablas
     - Stored procedure `sp_RegistrarEntregaInventario`
     - Vista `vw_MovimientosAlmacen`
   - Ejecuta los scripts `INSERT INTO` para datos iniciales

4. **Generar Diagrama E/R (Opcional)**
   - Expande `InventarioPruebaTecnicaDB` > "Database Diagrams"
   - Clic derecho y selecciona "New Database Diagram"

### Paso 3: Configuración de la Aplicación .NET

1. **Abrir el Proyecto**
   ```bash
   # Abrir la solución en Visual Studio
   InventoryWpfAppSolution.sln
   ```

2. **Restaurar Paquetes NuGet**
   - Los paquetes se restauran automáticamente
   - Si no, clic derecho en la solución > "Restore NuGet Packages"

3. **Actualizar Cadena de Conexión**
   - Archivo: `InventoryWpfApp/Repositories/Helpers/DbConnectionFactory.cs`
   - Actualizar `_connectionString`:
   ```csharp
   "Data Source=.\\SQLEXPRESS;Initial Catalog=InventarioPruebaTecnicaDB;Integrated Security=True;TrustServerCertificate=True;"
   ```

4. **Compilar la Solución**
   - En Visual Studio: `Build` > `Rebuild Solution`

## 🚀 Uso de la Aplicación

### Iniciar la Aplicación
```bash
# Ejecutar desde la carpeta de salida
InventoryWpfApp/bin/Debug/net8.0-windows/InventoryWpfApp.exe
```

### Navegación por Módulos

La aplicación se organiza en pestañas principales:

| Pestaña | Funcionalidad |
|---------|---------------|
| **Employees** | Gestión completa de empleados (CRUD) |
| **Products** | Gestión completa de productos (CRUD) |
| **Sizes** | Gestión completa de tallas (CRUD) |
| **Inventory Stock** | Gestión de stock por producto y talla (CRUD) |
| **Deliveries** | Registro de entregas a empleados |
| **Movement History** | Historial de movimientos de inventario |

## 🧪 Pruebas Unitarias

El proyecto incluye un conjunto completo de pruebas unitarias utilizando **xUnit.net** y **Moq**.

### Ejecutar Pruebas
1. En Visual Studio: `Test` > `Test Explorer`
2. Clic en `Run All Tests`

### Estructura de Pruebas
```
InventoryWpfApp.Tests/
├── ViewModels/
├── Repositories/
└── Services/
```

## 📁 Estructura del Proyecto

```
InventoryWpfAppSolution/
├── InventoryWpfApp/
│   ├── Models/
│   ├── ViewModels/
│   ├── Views/
│   ├── Repositories/
│   └── Services/
├── InventoryWpfApp.Tests/
├── SQL_Scripts/
└── README.md
```

## 📄 Historial de Versiones

### v1.0.0 (Beta) - 11 de Julio de 2025
- ✅ Entrega inicial de la aplicación de gestión de inventario
- ✅ Funcionalidades CRUD completas para todos los catálogos
- ✅ Implementación de módulo de entregas y historial de movimientos
- ✅ Arquitectura refactorizada a MVVM con repositorios e inyección de dependencias
- ✅ Suite de pruebas unitarias con xUnit.net y Moq

## 📜 Licencia

Este proyecto se entrega bajo los términos de una licencia propietaria para la prueba técnica.

### Dependencias con Licencia
- **.NET 8.0 SDK** (Licencia MIT)
- **SQL Server 2022 Express Edition** (Microsoft EULA)
- **xUnit.net** (Licencia Apache 2.0)
- **Moq** (Licencia MIT)

## 👨‍💻 Autor

**José Luis Chávez Ayala**
- Desarrollado como parte de una prueba técnica
- Fecha: Julio 2025

---

### 📞 Soporte

Si tienes alguna pregunta o problema con la instalación, por favor contacta al desarrollador o revisa la documentación técnica incluida en el proyecto.