-- =============================================
-- Script: 06_CreateOperationStatsTable.sql
-- Description: Creates the OperationStats table for tracking API operation counts
-- Author: System
-- Date: 2025-11-14
-- =============================================

USE [SensitiveWordsDB];
GO

-- Drop table if exists (for development/testing)
IF OBJECT_ID('[dbo].[OperationStats]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[OperationStats];
    PRINT 'Dropped existing OperationStats table';
END
GO

-- Create OperationStats table
CREATE TABLE [dbo].[OperationStats]
(
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [OperationType] NVARCHAR(50) NOT NULL,
    [ResourceType] NVARCHAR(50) NOT NULL,
    [Count] BIGINT NOT NULL DEFAULT 0,
    [LastUpdated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [UQ_OperationStats_Type] UNIQUE ([OperationType], [ResourceType])
);
GO

-- Create index for faster lookups
CREATE INDEX [IX_OperationStats_OperationType] ON [dbo].[OperationStats]([OperationType]);
GO

-- Insert initial records for all operation types
INSERT INTO [dbo].[OperationStats] ([OperationType], [ResourceType], [Count], [LastUpdated])
VALUES
    ('CREATE', 'SensitiveWord', 0, GETUTCDATE()),
    ('READ', 'SensitiveWord', 0, GETUTCDATE()),
    ('UPDATE', 'SensitiveWord', 0, GETUTCDATE()),
    ('DELETE', 'SensitiveWord', 0, GETUTCDATE()),
    ('SANITIZE', 'Message', 0, GETUTCDATE());
GO

PRINT 'OperationStats table created successfully with initial records';
GO
