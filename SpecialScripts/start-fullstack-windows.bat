@echo off
REM ============================================================================
REM Full Stack Startup Script (Separate Windows)
REM ============================================================================
REM This script starts the API and React frontend in separate windows
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Full Stack Startup (Separate Windows)
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "API_PROJECT=SensitiveWords.MicroService\SensitiveWords.API"
set "REACT_PROJECT=SensitiveWords.React"

REM Check if database exists
echo [INFO] Checking database connectivity...
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -Q "SELECT COUNT(*) FROM SensitiveWords" -b >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Cannot connect to database.
    echo [INFO] Run '.\SpecialScripts\setup-database.bat' to set up the database.
    echo.
    set /p CONTINUE="Do you want to continue without database? (Y/N): "
    if /i NOT "!CONTINUE!"=="Y" (
        echo [INFO] Startup cancelled.
        exit /b 1
    )
) else (
    echo [SUCCESS] Database connection verified.
)
echo.

REM Build the solution
echo [INFO] Building .NET solution...
dotnet build --configuration Release --nologo --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed. Please fix compilation errors.
    exit /b 1
)
echo [SUCCESS] .NET build completed successfully.
echo.

REM Check if React dependencies are installed
echo [INFO] Checking React dependencies...
if not exist "%REACT_PROJECT%\node_modules" (
    echo [INFO] Installing React dependencies ^(this may take a few minutes^)...
    cd %REACT_PROJECT%
    call npm install
    if errorlevel 1 (
        echo [ERROR] Failed to install React dependencies.
        cd ..
        exit /b 1
    )
    cd ..
    echo [SUCCESS] React dependencies installed.
) else (
    echo [SUCCESS] React dependencies already installed.
)
echo.

echo ============================================================================
echo  Starting Services...
echo ============================================================================
echo.

REM Start API in new window
echo [INFO] Starting .NET API in new window...
start "Flash.SensitiveWords - API" cmd /k "cd /d "%CD%\%API_PROJECT%" && echo ============================================================================ && echo  .NET API Server && echo ============================================================================ && echo. && echo API will be available at: && echo   - https://localhost:64725 && echo   - http://localhost:64726 && echo   - Swagger: https://localhost:64725/swagger && echo. && echo Press Ctrl+C to stop && echo. && dotnet run --no-build --configuration Release"

REM Wait a bit for API to start
echo [INFO] Waiting for API to initialize (8 seconds)...
timeout /t 8 /nobreak >nul

REM Start React in new window
echo [INFO] Starting React frontend in new window...
start "Flash.SensitiveWords - React" cmd /k "cd /d "%CD%\%REACT_PROJECT%" && echo ============================================================================ && echo  React Frontend Dev Server && echo ============================================================================ && echo. && echo Frontend will be available at: && echo   - http://localhost:5173 && echo. && echo Press Ctrl+C to stop && echo. && npm run dev"

echo.
echo ============================================================================
echo  Services Started Successfully!
echo ============================================================================
echo.
echo Two windows have been opened:
echo   1. .NET API Server (https://localhost:64725)
echo   2. React Frontend (http://localhost:5173)
echo.
echo The React site will automatically open in your browser once ready.
echo.
echo To stop the services:
echo   - Close each window, or
echo   - Press Ctrl+C in each window
echo.

REM Wait a bit more then open browser
timeout /t 5 /nobreak >nul
start http://localhost:5173

exit /b 0
