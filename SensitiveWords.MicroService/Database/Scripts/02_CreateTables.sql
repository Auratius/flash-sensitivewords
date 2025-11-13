USE SensitiveWordsDb;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SensitiveWords]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SensitiveWords] (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [Word] NVARCHAR(100) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE UNIQUE INDEX IX_SensitiveWords_Word ON [dbo].[SensitiveWords]([Word]);
    CREATE INDEX IX_SensitiveWords_IsActive ON [dbo].[SensitiveWords]([IsActive]);
END
GO
