-- =============================================
-- Flash.SanitizeWords - Stored Procedures
-- Created: 2025-11-12
-- Description: All stored procedures for CRUD operations on SensitiveWords
-- =============================================

USE SensitiveWordsDb;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_GetById
-- Description: Get a sensitive word by ID
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_GetById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_GetById;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Word,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.SensitiveWords
    WHERE Id = @Id;
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_GetAll
-- Description: Get all sensitive words
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_GetAll;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Word,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.SensitiveWords
    ORDER BY Word;
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_GetActive
-- Description: Get all active sensitive words
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_GetActive', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_GetActive;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_GetActive
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Word,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.SensitiveWords
    WHERE IsActive = 1
    ORDER BY Word;
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_GetByWord
-- Description: Get a sensitive word by word (case-insensitive)
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_GetByWord', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_GetByWord;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_GetByWord
    @Word NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Word,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM dbo.SensitiveWords
    WHERE UPPER(Word) = UPPER(@Word);
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_Create
-- Description: Create a new sensitive word
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_Create', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_Create;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_Create
    @Id UNIQUEIDENTIFIER,
    @Word NVARCHAR(100),
    @IsActive BIT,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.SensitiveWords (Id, Word, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Id, @Word, @IsActive, @CreatedAt, @UpdatedAt);

        -- Return the created ID
        SELECT @Id AS Id;
    END TRY
    BEGIN CATCH
        -- Return error information
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_Update
-- Description: Update an existing sensitive word
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_Update', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_Update;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_Update
    @Id UNIQUEIDENTIFIER,
    @Word NVARCHAR(100),
    @IsActive BIT,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE dbo.SensitiveWords
        SET
            Word = @Word,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id;

        -- Return rows affected
        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        -- Return error information
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_Delete
-- Description: Delete a sensitive word by ID
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_Delete;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM dbo.SensitiveWords
        WHERE Id = @Id;

        -- Return rows affected
        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        -- Return error information
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_Exists
-- Description: Check if a word exists (case-insensitive)
-- =============================================
IF OBJECT_ID('dbo.sp_SensitiveWords_Exists', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_Exists;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_Exists
    @Word NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CASE
            WHEN EXISTS (SELECT 1 FROM dbo.SensitiveWords WHERE UPPER(Word) = UPPER(@Word))
            THEN 1
            ELSE 0
        END AS [Exists];
END;
GO

-- =============================================
-- Procedure: sp_SensitiveWords_BulkInsert
-- Description: Bulk insert sensitive words (Table-Valued Parameter)
-- =============================================

-- First create the table type for bulk operations
IF TYPE_ID('dbo.SensitiveWordTableType') IS NOT NULL
    DROP TYPE dbo.SensitiveWordTableType;
GO

CREATE TYPE dbo.SensitiveWordTableType AS TABLE
(
    Id UNIQUEIDENTIFIER,
    Word NVARCHAR(100),
    IsActive BIT,
    CreatedAt DATETIME2,
    UpdatedAt DATETIME2
);
GO

IF OBJECT_ID('dbo.sp_SensitiveWords_BulkInsert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SensitiveWords_BulkInsert;
GO

CREATE PROCEDURE dbo.sp_SensitiveWords_BulkInsert
    @Words dbo.SensitiveWordTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.SensitiveWords (Id, Word, IsActive, CreatedAt, UpdatedAt)
        SELECT Id, Word, IsActive, CreatedAt, UpdatedAt
        FROM @Words;

        -- Return rows affected
        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        -- Return error information
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Grant execute permissions (adjust as needed)
-- =============================================
GRANT EXECUTE ON dbo.sp_SensitiveWords_GetById TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_GetAll TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_GetActive TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_GetByWord TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_Create TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_Update TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_Delete TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_Exists TO PUBLIC;
GRANT EXECUTE ON dbo.sp_SensitiveWords_BulkInsert TO PUBLIC;
GO

PRINT 'All stored procedures created successfully!';
GO
