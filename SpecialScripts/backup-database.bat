@echo off
REM ============================================================================
REM Database Backup Script
REM ============================================================================
REM This script creates a timestamped backup of the database
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Database Backup
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "SQL_SERVER=localhost\SQLEXPRESS"
set "DB_NAME=SensitiveWordsDb"
set "BACKUP_DIR=DatabaseBackups"

REM Create backup directory if it doesn't exist
if not exist "%BACKUP_DIR%" (
    mkdir "%BACKUP_DIR%"
    echo [INFO] Created backup directory: %BACKUP_DIR%
)

REM Get timestamp (format: YYYYMMDD_HHMMSS)
for /f "tokens=1-4 delims=/ " %%a in ('date /t') do (set mydate=%%c%%a%%b)
for /f "tokens=1-2 delims=: " %%a in ('time /t') do (set mytime=%%a%%b)
set TIMESTAMP=%mydate%_%mytime: =0%

REM Set backup filename
set "BACKUP_FILE=%BACKUP_DIR%\%DB_NAME%_%TIMESTAMP%.bak"
set "BACKUP_FILE_FULL=%CD%\%BACKUP_FILE%"

echo [INFO] Backup Configuration:
echo        Server: %SQL_SERVER%
echo        Database: %DB_NAME%
echo        Backup File: %BACKUP_FILE%
echo.

REM Check if sqlcmd is available
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] sqlcmd not found. Please install SQL Server Command Line Tools.
    exit /b 1
)

REM Check SQL Server connectivity
echo [INFO] Checking SQL Server connectivity...
sqlcmd -S %SQL_SERVER% -Q "SELECT @@VERSION" -b >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Cannot connect to SQL Server: %SQL_SERVER%
    exit /b 1
)
echo [SUCCESS] Connected to SQL Server.
echo.

REM Create backup
echo [INFO] Creating database backup...
echo        This may take a few moments...
echo.

sqlcmd -S %SQL_SERVER% -Q "BACKUP DATABASE [%DB_NAME%] TO DISK = '%BACKUP_FILE_FULL%' WITH FORMAT, MEDIANAME = 'SQLServerBackups', NAME = 'Full Backup of %DB_NAME%';" -b

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Backup failed.
    exit /b 1
)

echo [SUCCESS] Database backup completed successfully!
echo.

REM Get backup file size
for %%A in ("%BACKUP_FILE%") do set BACKUP_SIZE=%%~zA
set /a BACKUP_SIZE_MB=%BACKUP_SIZE% / 1048576

echo ============================================================================
echo  Backup Information
echo ============================================================================
echo  File: %BACKUP_FILE%
echo  Size: %BACKUP_SIZE_MB% MB
echo  Date: %DATE% %TIME%
echo ============================================================================
echo.

REM List recent backups
echo Recent backups:
echo.
dir /B /O-D "%BACKUP_DIR%\%DB_NAME%*.bak" 2>nul
echo.

REM Ask if user wants to open backup folder
set /p OPEN_FOLDER="Would you like to open the backup folder? (Y/N): "
if /i "%OPEN_FOLDER%"=="Y" (
    start explorer "%BACKUP_DIR%"
)

echo.
echo [INFO] To restore this backup, use:
echo        .\SpecialScripts\restore-database.bat %BACKUP_FILE%
echo.

exit /b 0
