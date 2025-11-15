-- =============================================
-- Script: 07_CreateOperationStatsStoredProcs.sql
-- Description: Creates stored procedures for operation statistics tracking
-- Author: System
-- Date: 2025-11-14
-- =============================================

USE [SensitiveWordsDB];
GO

-- =============================================
-- Stored Procedure: IncrementOperationCount
-- Description: Increments the count for a specific operation type
-- =============================================
IF OBJECT_ID('[dbo].[IncrementOperationCount]', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [dbo].[IncrementOperationCount];
END
GO

CREATE PROCEDURE [dbo].[IncrementOperationCount]
    @OperationType NVARCHAR(50),
    @ResourceType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Use MERGE for atomic upsert operation
    MERGE [dbo].[OperationStats] AS target
    USING (SELECT @OperationType AS OperationType, @ResourceType AS ResourceType) AS source
    ON (target.OperationType = source.OperationType AND target.ResourceType = source.ResourceType)
    WHEN MATCHED THEN
        UPDATE SET
            [Count] = target.[Count] + 1,
            [LastUpdated] = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT ([OperationType], [ResourceType], [Count], [LastUpdated])
        VALUES (source.OperationType, source.ResourceType, 1, GETUTCDATE());
END
GO

-- =============================================
-- Stored Procedure: GetAllOperationStats
-- Description: Retrieves all operation statistics
-- =============================================
IF OBJECT_ID('[dbo].[GetAllOperationStats]', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [dbo].[GetAllOperationStats];
END
GO

CREATE PROCEDURE [dbo].[GetAllOperationStats]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [OperationType],
        [ResourceType],
        [Count],
        [LastUpdated]
    FROM [dbo].[OperationStats]
    ORDER BY [OperationType], [ResourceType];
END
GO

-- =============================================
-- Stored Procedure: GetOperationStatsByType
-- Description: Retrieves statistics for a specific operation type
-- =============================================
IF OBJECT_ID('[dbo].[GetOperationStatsByType]', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [dbo].[GetOperationStatsByType];
END
GO

CREATE PROCEDURE [dbo].[GetOperationStatsByType]
    @OperationType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [OperationType],
        [ResourceType],
        [Count],
        [LastUpdated]
    FROM [dbo].[OperationStats]
    WHERE [OperationType] = @OperationType;
END
GO

-- =============================================
-- Stored Procedure: ResetOperationStats
-- Description: Resets all operation counts to zero (for testing/maintenance)
-- =============================================
IF OBJECT_ID('[dbo].[ResetOperationStats]', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [dbo].[ResetOperationStats];
END
GO

CREATE PROCEDURE [dbo].[ResetOperationStats]
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[OperationStats]
    SET
        [Count] = 0,
        [LastUpdated] = GETUTCDATE();

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

PRINT 'Operation statistics stored procedures created successfully';
GO
