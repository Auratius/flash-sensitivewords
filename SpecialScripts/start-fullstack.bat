@echo off
REM ============================================================================
REM Full Stack Startup Script
REM ============================================================================
REM This script starts both the API and React frontend concurrently
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Full Stack Startup
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "API_PROJECT=SensitiveWords.MicroService\SensitiveWords.API"
set "REACT_PROJECT=SensitiveWords.React"
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

REM Display startup information
echo ============================================================================
echo  Starting Full Stack Application...
echo ============================================================================
echo.
echo  API Endpoints:
echo    - Swagger UI:    %HTTPS_URL%/swagger
echo    - Health Check:  %HTTPS_URL%/health
echo    - HTTPS:         %HTTPS_URL%
echo    - HTTP:          %HTTP_URL%
echo.
echo  React Frontend:
echo    - Local:         http://localhost:5173
echo    - Network:       Check console output below
echo.
echo  Press Ctrl+C to stop both services
echo.
echo ============================================================================
echo.

REM Create a temporary VBS script to open browser after delay
echo Set WshShell = CreateObject("WScript.Shell") > "%TEMP%\open-browser.vbs"
echo WScript.Sleep 8000 >> "%TEMP%\open-browser.vbs"
echo WshShell.Run "http://localhost:5173", 1 >> "%TEMP%\open-browser.vbs"

REM Start browser opener in background
start /min wscript "%TEMP%\open-browser.vbs"

REM Start both services using PowerShell to run them concurrently
powershell -NoProfile -Command "$apiJob = Start-Job -ScriptBlock { Set-Location '%CD%\%API_PROJECT%'; dotnet run --no-build --configuration Release }; $reactJob = Start-Job -ScriptBlock { Set-Location '%CD%\%REACT_PROJECT%'; npm run dev }; Write-Host '[API] Starting .NET API...' -ForegroundColor Green; Write-Host '[REACT] Starting React Dev Server...' -ForegroundColor Cyan; Write-Host ''; try { while ($true) { Receive-Job -Job $apiJob; Receive-Job -Job $reactJob; if ($apiJob.State -eq 'Failed' -or $reactJob.State -eq 'Failed') { throw 'One or more services failed'; } Start-Sleep -Milliseconds 100; } } catch { Write-Host 'Stopping services...' -ForegroundColor Yellow; } finally { Stop-Job -Job $apiJob -ErrorAction SilentlyContinue; Stop-Job -Job $reactJob -ErrorAction SilentlyContinue; Remove-Job -Job $apiJob -ErrorAction SilentlyContinue; Remove-Job -Job $reactJob -ErrorAction SilentlyContinue; }"

REM Cleanup
del "%TEMP%\open-browser.vbs" 2>nul

echo.
echo ============================================================================
echo  Services Stopped
echo ============================================================================
echo.

exit /b 0
