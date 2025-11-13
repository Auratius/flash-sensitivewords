IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SensitiveWordsDb')
BEGIN
    CREATE DATABASE SensitiveWordsDb;
END
GO

USE SensitiveWordsDb;
GO
