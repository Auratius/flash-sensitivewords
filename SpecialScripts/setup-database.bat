@echo off
REM ============================================================================
REM Database Setup Script
REM ============================================================================
REM This script automates the complete database setup process
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Database Setup
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "DB_SCRIPTS=SensitiveWords.MicroService\Database\Scripts"
set "SQL_SERVER=localhost\SQLEXPRESS"
set "DB_NAME=SensitiveWordsDb"

echo [INFO] Database Setup Configuration:
echo        SQL Server: %SQL_SERVER%
echo        Database: %DB_NAME%
echo        Scripts Location: %DB_SCRIPTS%
echo.

REM Check if sqlcmd is available
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] sqlcmd not found. Please install SQL Server Command Line Tools.
    echo [INFO] Download from: https://aka.ms/sqlcmd
    exit /b 1
)

REM Check if SQL Server is running
echo [INFO] Checking SQL Server connectivity...
sqlcmd -S %SQL_SERVER% -Q "SELECT @@VERSION" -b >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Cannot connect to SQL Server: %SQL_SERVER%
    echo [INFO] Please ensure SQL Server is running and the server name is correct.
    exit /b 1
)
echo [SUCCESS] Connected to SQL Server successfully.
echo.

REM Step 1: Create Database
echo [STEP 1/4] Creating database '%DB_NAME%'...
sqlcmd -S %SQL_SERVER% -i "%DB_SCRIPTS%\01_CreateDatabase.sql" -b
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to create database.
    exit /b 1
)
echo [SUCCESS] Database created successfully.
echo.

REM Step 2: Create Tables
echo [STEP 2/4] Creating tables and indexes...
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -i "%DB_SCRIPTS%\02_CreateTables.sql" -b
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to create tables.
    exit /b 1
)
echo [SUCCESS] Tables created successfully.
echo.

REM Step 3: Create Stored Procedures
echo [STEP 3/4] Creating stored procedures...
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -i "%DB_SCRIPTS%\04_CreateStoredProcedures.sql" -b
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to create stored procedures.
    exit /b 1
)
echo [SUCCESS] Stored procedures created successfully.
echo.

REM Step 4: Seed Data
echo [STEP 4/4] Seeding initial data...
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -i "%DB_SCRIPTS%\03_SeedData.sql" -b
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to seed data.
    exit /b 1
)
echo [SUCCESS] Data seeded successfully.
echo.

REM Validate setup
echo [INFO] Validating database setup...
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -Q "SELECT COUNT(*) AS WordCount FROM SensitiveWords" -b
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Could not validate data, but setup completed.
) else (
    echo [SUCCESS] Database validated successfully.
)

echo.
echo ============================================================================
echo  Database Setup Completed Successfully!
echo ============================================================================
echo.
echo You can now run the API using: .\SpecialScripts\run-api.bat
echo.

exit /b 0
