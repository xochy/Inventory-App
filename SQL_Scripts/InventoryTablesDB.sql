USE InventoryTechnicalTestDB;
GO

-- 1. EmployeeTypes Table (Employee Type Catalog)
CREATE TABLE EmployeeTypes (
    EmployeeTypeId INT IDENTITY(1,1) PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL UNIQUE -- We use brackets around 'Type' because it is a reserved word in some SQL contexts
);

-- 2. Groups Table (Groups Catalog)
CREATE TABLE Groups (
    GroupId INT IDENTITY(1,1) PRIMARY KEY,
    [Group] NVARCHAR(100) NOT NULL UNIQUE, -- We use brackets for 'Group'
    EmployeeTypeId INT NOT NULL,
    FOREIGN KEY (EmployeeTypeId) REFERENCES EmployeeTypes(EmployeeTypeId)
);

-- 3. Employees Table (Employee/User Catalog)
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    GroupId INT NOT NULL,
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId)
);

-- 4. Products Table (Item Catalog)
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500),
    ApplicabilityType NVARCHAR(50) NOT NULL -- 'Administrativo', 'Sindicalizado', 'Todos'
);

-- 5. Sizes Table (Size Catalog)
CREATE TABLE Sizes (
    SizeId INT IDENTITY(1,1) PRIMARY KEY,
    SizeValue NVARCHAR(50) NOT NULL UNIQUE, -- Ej: 'M', 'L', 'XL', '32', 'Unitalla'
    NotationType NVARCHAR(50) -- Ej: 'Mexicana', 'Americana', 'Europea', 'Unica'
);

-- 6. InventoryStock Table (Inventory Stock by Product and Size)
CREATE TABLE InventoryStock (
    InventoryStockId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    SizeId INT NOT NULL,
    CurrentQuantity INT NOT NULL DEFAULT 0 CHECK (CurrentQuantity >= 0),
    MinStockLimit INT NOT NULL DEFAULT 0 CHECK (MinStockLimit >= 0),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (SizeId) REFERENCES Sizes(SizeId),
    UNIQUE (ProductId, SizeId) -- Ensures that there is only one entry per product and size
);

-- 7. Movements Table (Inventory Movement Record)
CREATE TABLE Movements (
    MovementId INT IDENTITY(1,1) PRIMARY KEY,
    MovementDate DATETIME NOT NULL DEFAULT GETDATE(),
    MovementType NVARCHAR(100) NOT NULL, -- 'Entrada por Compra', 'Ajuste', 'Devolucion', 'Salida por Entrega'
    Quantity INT NOT NULL CHECK (Quantity > 0), -- Cantidad movida
    InventoryStockId INT NOT NULL, -- Quantity moved
    EmployeeId INT, -- Optional, for deliveries or returns from/to an employee
    Notes NVARCHAR(500), -- Additional notes if necessary
    FOREIGN KEY (InventoryStockId) REFERENCES InventoryStock(InventoryStockId),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);
GO