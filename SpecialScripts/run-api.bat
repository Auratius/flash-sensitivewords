@echo off
REM ============================================================================
REM API Startup Script
REM ============================================================================
REM This script builds and runs the SensitiveWords API
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Starting API
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "API_PROJECT=SensitiveWords.MicroService\SensitiveWords.API"
set "HTTPS_URL=https://localhost:64725"
set "HTTP_URL=http://localhost:64726"

REM Check if database exists
echo [INFO] Checking database connectivity...
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -Q "SELECT COUNT(*) FROM SensitiveWords" -b >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Cannot connect to database.
    echo [INFO] Run '.\SpecialScripts\setup-database.bat' to set up the database.
    echo.
    set /p CONTINUE="Do you want to continue without database? (Y/N): "
    if /i NOT "!CONTINUE!"=="Y" (
        echo [INFO] API startup cancelled.
        exit /b 1
    )
) else (
    echo [SUCCESS] Database connection verified.
)
echo.

REM Build the solution
echo [INFO] Building solution...
dotnet build --configuration Release --nologo --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed. Please fix compilation errors.
    exit /b 1
)
echo [SUCCESS] Build completed successfully.
echo.

REM Display startup information
echo ============================================================================
echo  API Starting...
echo ============================================================================
echo.
echo  Swagger UI:    %HTTPS_URL%/swagger
echo  Health Check:  %HTTPS_URL%/health
echo  HTTPS:         %HTTPS_URL%
echo  HTTP:          %HTTP_URL%
echo.
echo  Press Ctrl+C to stop the API
echo.
echo ============================================================================
echo.

REM Start the API
cd %API_PROJECT%
dotnet run --no-build --configuration Release

exit /b 0
