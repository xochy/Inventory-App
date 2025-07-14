USE InventoryTechnicalTestDB;
GO

DELETE FROM Movements;
DELETE FROM InventoryStock;
DELETE FROM Sizes;
DELETE FROM Products;
DELETE FROM Employees;
DELETE FROM Groups;
DELETE FROM EmployeeTypes;

-- Restart IDENTITY
DBCC CHECKIDENT ('Movements', RESEED, 0);
DBCC CHECKIDENT ('InventoryStock', RESEED, 0);
DBCC CHECKIDENT ('Sizes', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);
DBCC CHECKIDENT ('Groups', RESEED, 0);
DBCC CHECKIDENT ('EmployeeTypes', RESEED, 0);

-- Data for EmployeeTypes
INSERT INTO EmployeeTypes ([Type]) VALUES
('Administrativo'),
('Sindicalizado'),
('Visitante'); -- Can add other types if needed.
GO

-- Data for Groups
INSERT INTO Groups ([Group], EmployeeTypeId) VALUES
('Contabilidad', (SELECT EmployeeTypeId FROM EmployeeTypes WHERE [Type] = 'Administrativo')),
('Recursos Humanos', (SELECT EmployeeTypeId FROM EmployeeTypes WHERE [Type] = 'Administrativo')),
('Produccion A', (SELECT EmployeeTypeId FROM EmployeeTypes WHERE [Type] = 'Sindicalizado')),
('Mantenimiento', (SELECT EmployeeTypeId FROM EmployeeTypes WHERE [Type] = 'Sindicalizado'));
GO

-- Data for Employees
INSERT INTO Employees (Name, GroupId) VALUES
('Juan Perez', (SELECT GroupId FROM Groups WHERE [Group] = 'Contabilidad')),
('Maria Lopez', (SELECT GroupId FROM Groups WHERE [Group] = 'Recursos Humanos')),
('Pedro Garcia', (SELECT GroupId FROM Groups WHERE [Group] = 'Produccion A')),
('Ana Rodriguez', (SELECT GroupId FROM Groups WHERE [Group] = 'Mantenimiento'));
GO

-- Data for Products
INSERT INTO Products (Name, Description, ApplicabilityType) VALUES
('Camisa Polo', 'Camisa con logo de la compañía para personal administrativo', 'Administrativo'),
('Chamarra Invierno', 'Chamarra corporativa', 'Administrativo'),
('Casco Seguridad', 'Casco de protección industrial', 'Sindicalizado'),
('Botas Casquillo', 'Botas de seguridad con casquillo de acero', 'Sindicalizado'),
('Chaleco Reflejante', 'Chaleco de alta visibilidad', 'Todos');
GO

-- Data for Sizes
INSERT INTO Sizes (SizeValue, NotationType) VALUES
('S', 'Americana'),
('M', 'Americana'),
('L', 'Americana'),
('XL', 'Americana'),
('38', 'Mexicana'),
('40', 'Mexicana'),
('Unitalla', 'Unica');
GO

-- Initial data for InventoryStock (examples)
-- 'Camisa Polo' Talla 'M'
INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit) VALUES
((SELECT ProductId FROM Products WHERE Name = 'Camisa Polo'), (SELECT SizeId FROM Sizes WHERE SizeValue = 'M'), 50, 10);

-- 'Camisa Polo' Talla 'L'
INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit) VALUES
((SELECT ProductId FROM Products WHERE Name = 'Camisa Polo'), (SELECT SizeId FROM Sizes WHERE SizeValue = 'L'), 60, 15);

-- 'Casco Seguridad' Unitalla
INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit) VALUES
((SELECT ProductId FROM Products WHERE Name = 'Casco Seguridad'), (SELECT SizeId FROM Sizes WHERE SizeValue = 'Unitalla'), 100, 20);

-- 'Botas Casquillo' Talla '40'
INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit) VALUES
((SELECT ProductId FROM Products WHERE Name = 'Botas Casquillo'), (SELECT SizeId FROM Sizes WHERE SizeValue = '40'), 30, 5);

-- 'Chaleco Reflejante' Talla 'L'
INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit) VALUES
((SELECT ProductId FROM Products WHERE Name = 'Chaleco Reflejante'), (SELECT SizeId FROM Sizes WHERE SizeValue = 'L'), 80, 15);
GO