@echo off
REM ============================================================================
REM Database Reset Script
REM ============================================================================
REM This script drops and recreates the database for clean testing
REM WARNING: This will DELETE all data in the database!
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Database Reset
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "SQL_SERVER=localhost\SQLEXPRESS"
set "DB_NAME=SensitiveWordsDb"

echo [WARNING] This will DELETE the '%DB_NAME%' database and all its data!
echo.
set /p CONFIRM="Are you sure you want to continue? (Y/N): "
if /i NOT "%CONFIRM%"=="Y" (
    echo [INFO] Database reset cancelled.
    exit /b 0
)

echo.
echo [INFO] Resetting database...

REM Check if sqlcmd is available
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] sqlcmd not found. Please install SQL Server Command Line Tools.
    exit /b 1
)

REM Drop database if exists
echo [INFO] Dropping existing database...
sqlcmd -S %SQL_SERVER% -Q "IF EXISTS (SELECT name FROM sys.databases WHERE name = '%DB_NAME%') BEGIN ALTER DATABASE [%DB_NAME%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%DB_NAME%]; END" -b
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Could not drop database (it may not exist).
) else (
    echo [SUCCESS] Database dropped successfully.
)
echo.

REM Run setup script
echo [INFO] Running database setup...
call "%~dp0setup-database.bat"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ============================================================================
    echo  Database Reset Completed Successfully!
    echo ============================================================================
    echo.
) else (
    echo [ERROR] Database reset failed.
    exit /b 1
)

exit /b 0
