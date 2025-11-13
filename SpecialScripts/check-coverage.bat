@echo off
REM ============================================================================
REM Code Coverage Check Script
REM ============================================================================
REM This script runs tests with code coverage collection and generates reports
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Code Coverage Analysis
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Clean previous test results
echo [INFO] Cleaning previous test results...
if exist TestResults (
    rmdir /s /q TestResults
)

REM Run tests with coverage
echo [INFO] Running tests with code coverage collection...
echo.
dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory:./TestResults --nologo --verbosity minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Tests failed. Please fix failing tests first.
    exit /b 1
)

REM Check for reportgenerator tool
echo.
echo [INFO] Checking for reportgenerator tool...
dotnet tool list -g | findstr reportgenerator >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [INFO] Installing dotnet-reportgenerator-globaltool...
    dotnet tool install -g dotnet-reportgenerator-globaltool
)

REM Generate merged coverage report
echo [INFO] Generating merged coverage report...
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary;Badges" -verbosity:Warning

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to generate coverage report.
    exit /b 1
)

echo.
echo ============================================================================
echo  Coverage Report Generated Successfully
echo ============================================================================
echo.

REM Display the text summary
if exist TestResults\CoverageReport\Summary.txt (
    type TestResults\CoverageReport\Summary.txt
    echo.
)

echo ============================================================================
echo  Report Location: TestResults\CoverageReport\index.html
echo ============================================================================
echo.

REM Ask user if they want to open the HTML report
set /p OPEN_REPORT="Would you like to open the HTML coverage report? (Y/N): "
if /i "%OPEN_REPORT%"=="Y" (
    start "" "TestResults\CoverageReport\index.html"
)

echo.
echo [SUCCESS] Code coverage analysis completed successfully!
echo.

exit /b 0
