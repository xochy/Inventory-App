USE InventoryTechnicalTestDB;
GO

ALTER PROCEDURE sp_InventoryRegisterDelivery
    @InventoryStockId INT,
    @EmployeeId INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON; -- Prevents row counts from being returned

    DECLARE @CurrentStock INT;
    DECLARE @ProductName NVARCHAR(255);
    DECLARE @SizeValue NVARCHAR(50);
    DECLARE @ApplicabilityType NVARCHAR(50);
    DECLARE @EmployeeGroupType NVARCHAR(50);

    -- 1. Check if the InventoryStockId is valid
    SELECT @CurrentStock = InvS.CurrentQuantity,
           @ProductName = P.Name,
           @SizeValue = S.SizeValue,
           @ApplicabilityType = P.ApplicabilityType
    FROM InventoryStock InvS
    JOIN Products P ON InvS.ProductId = P.ProductId
    JOIN Sizes S ON InvS.SizeId = S.SizeId
    WHERE InvS.InventoryStockId = @InventoryStockId;

    IF @CurrentStock IS NULL
    BEGIN
        RAISERROR('Error: The inventory stock ID is not valid.', 16, 1);
        RETURN;
    END;

    -- 2. Check stock availability
    IF @CurrentStock < @Quantity
    BEGIN
        RAISERROR('Error: Not enough stock available for the requested quantity.', 16, 1);
        RETURN;
    END;

    -- 3. Check the applicability of the article to the type of employee
    SELECT @EmployeeGroupType = ET.[Type]
    FROM Employees E
    JOIN Groups G ON E.GroupId = G.GroupId
    JOIN EmployeeTypes ET ON G.EmployeeTypeId = ET.EmployeeTypeId
    WHERE E.EmployeeId = @EmployeeId;

    IF @EmployeeGroupType IS NULL
    BEGIN
        RAISERROR('Error: The employee ID is not valid.', 16, 1);
        RETURN;
    END;

    IF (@ApplicabilityType = 'Administrativo' AND @EmployeeGroupType <> 'Administrativo')
        OR (@ApplicabilityType = 'Sindicalizado' AND @EmployeeGroupType <> 'Sindicalizado')
    BEGIN
        -- Exception for reflective vests or others that apply to 'Todos'
        IF NOT (@ProductName = 'Chaleco Reflejante' AND @ApplicabilityType = 'Todos')
        BEGIN
            RAISERROR('Error: This article does not apply to this type of worker.', 16, 1);
            RETURN;
        END
    END;


    BEGIN TRY
        BEGIN TRANSACTION;

        -- Update the quantity in InventoryStock
        UPDATE InventoryStock
        SET CurrentQuantity = CurrentQuantity - @Quantity
        WHERE InventoryStockId = @InventoryStockId;

        -- Record the outgoing movement
        INSERT INTO Movements (MovementDate, MovementType, Quantity, InventoryStockId, EmployeeId, Notes)
        VALUES (GETDATE(), 'Salida por Entrega', @Quantity, @InventoryStockId, @EmployeeId, 'Entrega a trabajador');

        COMMIT TRANSACTION;
        PRINT 'Entrega registrada exitosamente.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO

-- Example of how to execute the Stored Procedure:
-- DECLARE @stockId INT = (SELECT TOP 1 InventoryStockId FROM InventoryStock WHERE ProductId = (SELECT ProductId FROM Products WHERE Name = 'Camisa Polo') AND SizeId = (SELECT SizeId FROM Sizes WHERE SizeValue = 'M'));
-- DECLARE @empId INT = (SELECT TOP 1 EmployeeId FROM Employees WHERE Name = 'Juan Perez');
-- EXEC sp_InventoryRegisterDelivery @InventoryStockId = @stockId, @EmployeeId = @empId, @Quantity = 1;

-- Another example (if you want to try a unionized vest delivery, which should work)
-- DECLARE @stockId_chaleco INT = (SELECT TOP 1 InventoryStockId FROM InventoryStock WHERE ProductId = (SELECT ProductId FROM Products WHERE Name = 'Chaleco Reflejante') AND SizeId = (SELECT SizeId FROM Sizes WHERE SizeValue = 'L'));
-- DECLARE @empId_sindicalizado INT = (SELECT TOP 1 EmployeeId FROM Employees WHERE Name = 'Pedro Garcia');
-- EXEC sp_InventoryRegisterDelivery @InventoryStockId = @stockId_chaleco, @EmployeeId = @empId_sindicalizado, @Quantity = 1;

-- Example of an error due to insufficient stock
-- EXEC sp_InventoryRegisterDelivery @InventoryStockId = @stockId, @EmployeeId = @empId, @Quantity = 100;
-- Ejemplo de error por tipo de artículo incorrecto
-- DECLARE @stockId_casco INT = (SELECT TOP 1 InventoryStockId FROM InventoryStock WHERE ProductId = (SELECT ProductId FROM Products WHERE Name = 'Casco Seguridad') AND SizeId = (SELECT SizeId FROM Sizes WHERE SizeValue = 'Unitalla'));
-- EXEC sp_InventoryRegisterDelivery @InventoryStockId = @stockId_casco, @EmployeeId = @empId, @Quantity = 1; -- Intentar entregar casco a Juan Perez (Administrativo)