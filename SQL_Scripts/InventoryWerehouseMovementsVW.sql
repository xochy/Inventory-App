USE InventoryTechnicalTestDB;
GO

CREATE VIEW vw_InventoryWerehouseMovements
AS
SELECT
    M.MovementId,
    M.MovementDate,
    M.MovementType,
    M.Quantity AS QuantityMoved,
    P.Name AS ProductName,
    P.Description AS ProductDescription,
    S.SizeValue AS ProductSize,
    S.NotationType AS SizeNotation,
    InvS.CurrentQuantity AS RemainingStock,
    InvS.MinStockLimit,   
    E.Name AS EmployeeName,
    G.[Group] AS EmployeeGroup,
    ET.[Type] AS EmployeeType,
    M.Notes
FROM
    Movements M
JOIN
    InventoryStock InvS ON M.InventoryStockId = InvS.InventoryStockId
JOIN
    Products P ON InvS.ProductId = P.ProductId
JOIN
    Sizes S ON InvS.SizeId = S.SizeId
LEFT JOIN
    Employees E ON M.EmployeeId = E.EmployeeId
LEFT JOIN
    Groups G ON E.GroupId = G.GroupId
LEFT JOIN
    EmployeeTypes ET ON G.EmployeeTypeId = ET.EmployeeTypeId;
GO

-- For testing:
-- SELECT * FROM vw_MovimientosAlmacen;