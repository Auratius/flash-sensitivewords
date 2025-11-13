@echo off
REM ============================================================================
REM Database Migration Generator
REM ============================================================================
REM This script creates a timestamped SQL migration file
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Generate Migration Script
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "SCRIPTS_DIR=SensitiveWords.MicroService\Database\Scripts"

REM Get timestamp (format: YYYYMMDD_HHMMSS)
for /f "tokens=1-4 delims=/ " %%a in ('date /t') do (set mydate=%%c%%a%%b)
for /f "tokens=1-2 delims=: " %%a in ('time /t') do (set mytime=%%a%%b)
set TIMESTAMP=%mydate%_%mytime: =0%

REM Get migration description
echo Please enter a description for this migration:
echo (e.g., "AddEmailColumnToUsers", "UpdateSensitiveWordsIndex")
echo.
set /p DESCRIPTION="Description: "

if "%DESCRIPTION%"=="" (
    echo [ERROR] Description cannot be empty.
    exit /b 1
)

REM Remove spaces from description
set DESCRIPTION=%DESCRIPTION: =%

REM Create migration filename
set FILENAME=%TIMESTAMP%_%DESCRIPTION%.sql

echo.
echo [INFO] Creating migration file: %FILENAME%
echo.

REM Create migration template
(
echo -- ============================================================================
echo -- Migration: %DESCRIPTION%
echo -- Created: %DATE% %TIME%
echo -- ============================================================================
echo.
echo -- Description:
echo -- [Describe what this migration does]
echo.
echo -- ============================================================================
echo.
echo USE [SensitiveWordsDb]
echo GO
echo.
echo -- ============================================================================
echo -- Begin Migration
echo -- ============================================================================
echo.
echo -- [Add your SQL statements here]
echo.
echo -- Example: Add a new column
echo -- ALTER TABLE SensitiveWords ADD [NewColumn] NVARCHAR(50) NULL;
echo.
echo -- Example: Create a new table
echo -- CREATE TABLE [NewTable] (
echo --     Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(^),
echo --     Name NVARCHAR(100^) NOT NULL,
echo --     CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(^)
echo -- ^);
echo.
echo -- Example: Create an index
echo -- CREATE INDEX IX_NewTable_Name ON [NewTable](Name^);
echo.
echo -- Example: Create/Alter a stored procedure
echo -- CREATE OR ALTER PROCEDURE sp_NewProcedure
echo --     @Param1 NVARCHAR(100^)
echo -- AS
echo -- BEGIN
echo --     SET NOCOUNT ON;
echo --     -- Procedure logic here
echo -- END;
echo -- GO
echo.
echo -- ============================================================================
echo -- End Migration
echo -- ============================================================================
echo.
echo -- Rollback Script (if applicable^):
echo -- [Add rollback statements to undo this migration]
echo.
echo -- Example rollback:
echo -- ALTER TABLE SensitiveWords DROP COLUMN [NewColumn];
echo -- DROP TABLE [NewTable];
echo -- DROP INDEX IX_NewTable_Name ON [NewTable];
echo.
echo PRINT 'Migration %FILENAME% completed successfully';
echo GO
) > "%SCRIPTS_DIR%\%FILENAME%"

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to create migration file.
    exit /b 1
)

echo [SUCCESS] Migration file created successfully!
echo.
echo ============================================================================
echo  File Location: %SCRIPTS_DIR%\%FILENAME%
echo ============================================================================
echo.
echo Next steps:
echo  1. Edit the migration file and add your SQL statements
echo  2. Test the migration: sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i "%SCRIPTS_DIR%\%FILENAME%"
echo  3. Add the migration to source control
echo.

REM Ask if user wants to open the file
set /p OPEN_FILE="Would you like to open the migration file now? (Y/N): "
if /i "%OPEN_FILE%"=="Y" (
    start notepad "%SCRIPTS_DIR%\%FILENAME%"
)

exit /b 0
