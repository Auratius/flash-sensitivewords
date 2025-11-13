@echo off
REM ============================================================================
REM Log Viewer Script
REM ============================================================================
REM This script helps view and analyze Serilog log files
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Log Viewer
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set log directory
set "LOG_DIR=SensitiveWords.MicroService\SensitiveWords.API\Logs"

REM Check if log directory exists
if not exist "%LOG_DIR%" (
    echo [INFO] Log directory does not exist: %LOG_DIR%
    echo [INFO] Logs will be created when the API runs.
    exit /b 0
)

REM Count log files
set LOG_COUNT=0
for %%f in ("%LOG_DIR%\log-*.txt") do set /a LOG_COUNT+=1

if %LOG_COUNT% EQU 0 (
    echo [INFO] No log files found.
    echo [INFO] Logs will be created when the API runs.
    exit /b 0
)

echo [INFO] Found %LOG_COUNT% log file(s) in %LOG_DIR%
echo.

REM Display menu
echo Select an option:
echo.
echo  [1] View latest log file
echo  [2] View all logs (concatenated)
echo  [3] Search logs for keyword
echo  [4] View errors only
echo  [5] Tail latest log (live view)
echo  [6] Open log folder in Explorer
echo  [7] Clear old logs
echo  [0] Exit
echo.
set /p CHOICE="Enter your choice (0-7): "

if "%CHOICE%"=="0" exit /b 0

echo.
echo ============================================================================

if "%CHOICE%"=="1" (
    echo  Latest Log File
    echo ============================================================================
    echo.
    REM Find most recent log file
    for /f "delims=" %%f in ('dir /b /o-d "%LOG_DIR%\log-*.txt" 2^>nul') do (
        set LATEST_LOG=%%f
        goto :found_latest
    )
    :found_latest
    if defined LATEST_LOG (
        echo [INFO] Viewing: %LOG_DIR%\!LATEST_LOG!
        echo.
        type "%LOG_DIR%\!LATEST_LOG!"
    ) else (
        echo [ERROR] No log files found.
    )
)

if "%CHOICE%"=="2" (
    echo  All Logs (Concatenated)
    echo ============================================================================
    echo.
    for %%f in ("%LOG_DIR%\log-*.txt") do (
        echo [FILE: %%~nxf]
        type "%%f"
        echo.
        echo ------------------------------------------------------------------------
        echo.
    )
)

if "%CHOICE%"=="3" (
    echo  Search Logs
    echo ============================================================================
    echo.
    set /p KEYWORD="Enter search keyword: "
    if defined KEYWORD (
        echo.
        echo [INFO] Searching for: !KEYWORD!
        echo.
        findstr /i /c:"!KEYWORD!" "%LOG_DIR%\log-*.txt"
        if %ERRORLEVEL% NEQ 0 (
            echo [INFO] No matches found for "!KEYWORD!".
        )
    )
)

if "%CHOICE%"=="4" (
    echo  Errors Only
    echo ============================================================================
    echo.
    findstr /i /c:"[ERR]" /c:"[FTL]" /c:"Error" /c:"Exception" "%LOG_DIR%\log-*.txt"
    if %ERRORLEVEL% NEQ 0 (
        echo [INFO] No errors found in logs.
    )
)

if "%CHOICE%"=="5" (
    echo  Tail Latest Log (Live View)
    echo ============================================================================
    echo.
    echo [INFO] Press Ctrl+C to stop watching...
    echo.
    REM Find most recent log file
    for /f "delims=" %%f in ('dir /b /o-d "%LOG_DIR%\log-*.txt" 2^>nul') do (
        set LATEST_LOG=%%f
        goto :found_tail
    )
    :found_tail
    if defined LATEST_LOG (
        powershell -Command "Get-Content '%LOG_DIR%\!LATEST_LOG!' -Wait -Tail 20"
    ) else (
        echo [ERROR] No log files found.
    )
)

if "%CHOICE%"=="6" (
    echo  Opening Log Folder
    echo ============================================================================
    echo.
    start explorer "%LOG_DIR%"
    echo [INFO] Log folder opened in Explorer.
)

if "%CHOICE%"=="7" (
    echo  Clear Old Logs
    echo ============================================================================
    echo.
    echo [WARNING] This will delete log files older than 7 days.
    echo.
    set /p CONFIRM="Are you sure? (Y/N): "
    if /i "!CONFIRM!"=="Y" (
        forfiles /p "%LOG_DIR%" /s /m log-*.txt /d -7 /c "cmd /c del @path" 2>nul
        if %ERRORLEVEL% EQU 0 (
            echo [SUCCESS] Old logs deleted.
        ) else (
            echo [INFO] No old logs to delete.
        )
    ) else (
        echo [INFO] Operation cancelled.
    )
)

echo.
echo ============================================================================
echo.

exit /b 0
