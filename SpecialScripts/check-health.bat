@echo off
REM ============================================================================
REM System Health Check Script
REM ============================================================================
REM This script validates all system dependencies and connectivity
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - System Health Check
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

set "HEALTH_STATUS=HEALTHY"

REM Check 1: .NET SDK
echo [CHECK 1/7] .NET SDK...
dotnet --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found.
    set "HEALTH_STATUS=UNHEALTHY"
) else (
    for /f "tokens=*" %%a in ('dotnet --version') do set DOTNET_VERSION=%%a
    echo [SUCCESS] .NET SDK !DOTNET_VERSION! installed.
)
echo.

REM Check 2: SQL Server
echo [CHECK 2/7] SQL Server connectivity...
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] sqlcmd not found. SQL Server tools not installed.
    set "HEALTH_STATUS=UNHEALTHY"
) else (
    sqlcmd -S localhost\SQLEXPRESS -Q "SELECT @@VERSION" -b >nul 2>&1
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Cannot connect to SQL Server (localhost\SQLEXPRESS^).
        set "HEALTH_STATUS=UNHEALTHY"
    ) else (
        echo [SUCCESS] SQL Server is running and accessible.
    )
)
echo.

REM Check 3: Database
echo [CHECK 3/7] Database existence...
sqlcmd -S localhost\SQLEXPRESS -Q "SELECT name FROM sys.databases WHERE name = 'SensitiveWordsDb'" -h -1 -W -b 2>nul | findstr "SensitiveWordsDb" >nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Database 'SensitiveWordsDb' not found.
    echo [INFO] Run '.\SpecialScripts\setup-database.bat' to create it.
    set "HEALTH_STATUS=UNHEALTHY"
) else (
    echo [SUCCESS] Database 'SensitiveWordsDb' exists.

    REM Check table and data
    sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -Q "SELECT COUNT(*) AS Count FROM SensitiveWords" -h -1 -W -b 2>nul > temp_count.txt
    set /p WORD_COUNT=<temp_count.txt
    del temp_count.txt 2>nul
    echo [INFO] Sensitive words in database: !WORD_COUNT!
)
echo.

REM Check 4: Solution Build
echo [CHECK 4/7] Solution build status...
dotnet build --configuration Release --nologo --verbosity quiet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Solution has build errors.
    set "HEALTH_STATUS=UNHEALTHY"
) else (
    echo [SUCCESS] Solution builds successfully.
)
echo.

REM Check 5: Required Tools
echo [CHECK 5/7] Development tools...
dotnet tool list -g | findstr reportgenerator >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] reportgenerator tool not installed.
    echo [INFO] Run '.\SpecialScripts\install-tools.bat' to install it.
) else (
    echo [SUCCESS] reportgenerator tool installed.
)
echo.

REM Check 6: Project Structure
echo [CHECK 6/7] Project structure...
set "STRUCTURE_OK=1"
if not exist "SensitiveWords.MicroService\SensitiveWords.API" set "STRUCTURE_OK=0"
if not exist "SensitiveWords.MicroService\SensitiveWords.Domain" set "STRUCTURE_OK=0"
if not exist "SensitiveWords.MicroService\SensitiveWords.Application" set "STRUCTURE_OK=0"
if not exist "SensitiveWords.MicroService\SensitiveWords.Infrastructure" set "STRUCTURE_OK=0"
if not exist "SensitiveWords.MicroService\SensitiveWords.Tests.Unit" set "STRUCTURE_OK=0"
if not exist "SensitiveWords.MicroService\SensitiveWords.Tests.Integration" set "STRUCTURE_OK=0"

if "%STRUCTURE_OK%"=="0" (
    echo [ERROR] Project structure is incomplete.
    set "HEALTH_STATUS=UNHEALTHY"
) else (
    echo [SUCCESS] All project folders exist.
)
echo.

REM Check 7: API Endpoint (if running)
echo [CHECK 7/7] API health endpoint...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'https://localhost:64725/health' -SkipCertificateCheck -TimeoutSec 2 -ErrorAction Stop; exit 0 } catch { exit 1 }" >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [INFO] API is not currently running.
    echo [INFO] Start it with: .\SpecialScripts\run-api.bat
) else (
    echo [SUCCESS] API is running and healthy.
)
echo.

REM Summary
echo ============================================================================
if "%HEALTH_STATUS%"=="HEALTHY" (
    echo  SYSTEM STATUS: HEALTHY
    echo ============================================================================
    echo.
    echo  All critical checks passed. System is ready for development.
    echo.
    exit /b 0
) else (
    echo  SYSTEM STATUS: UNHEALTHY
    echo ============================================================================
    echo.
    echo  Some checks failed. Please fix the issues above.
    echo.
    echo  Quick fixes:
    echo   - Install .NET 8 SDK: https://dotnet.microsoft.com/download
    echo   - Install SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
    echo   - Setup database: .\SpecialScripts\setup-database.bat
    echo   - Install tools: .\SpecialScripts\install-tools.bat
    echo.
    exit /b 1
)
