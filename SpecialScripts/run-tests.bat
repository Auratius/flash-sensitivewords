@echo off
REM ============================================================================
REM Test Runner Script
REM ============================================================================
REM This script provides various test execution options
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Test Runner
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

echo Select test execution mode:
echo.
echo  [1] Run Unit Tests Only (Fast)
echo  [2] Run Integration Tests Only (Requires Database)
echo  [3] Run All Tests
echo  [4] Run All Tests with Detailed Output
echo  [5] Run All Tests with Code Coverage
echo  [0] Cancel
echo.
set /p CHOICE="Enter your choice (0-5): "

if "%CHOICE%"=="0" (
    echo [INFO] Test execution cancelled.
    exit /b 0
)

echo.
echo ============================================================================

if "%CHOICE%"=="1" (
    echo  Running Unit Tests Only...
    echo ============================================================================
    echo.
    dotnet test SensitiveWords.MicroService\SensitiveWords.Tests.Unit --configuration Release --nologo --verbosity minimal
    set TEST_RESULT=%ERRORLEVEL%
)

if "%CHOICE%"=="2" (
    echo  Running Integration Tests Only...
    echo ============================================================================
    echo.
    echo [INFO] Verifying database connectivity...
    sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -Q "SELECT COUNT(*) FROM SensitiveWords" -b >nul 2>&1
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Cannot connect to database. Run '.\SpecialScripts\setup-database.bat' first.
        exit /b 1
    )
    echo [SUCCESS] Database connection verified.
    echo.
    dotnet test SensitiveWords.MicroService\SensitiveWords.Tests.Integration --configuration Release --nologo --verbosity minimal
    set TEST_RESULT=%ERRORLEVEL%
)

if "%CHOICE%"=="3" (
    echo  Running All Tests...
    echo ============================================================================
    echo.
    dotnet test --configuration Release --nologo --verbosity minimal
    set TEST_RESULT=%ERRORLEVEL%
)

if "%CHOICE%"=="4" (
    echo  Running All Tests with Detailed Output...
    echo ============================================================================
    echo.
    dotnet test --configuration Release --nologo --verbosity normal
    set TEST_RESULT=%ERRORLEVEL%
)

if "%CHOICE%"=="5" (
    echo  Running All Tests with Code Coverage...
    echo ============================================================================
    echo.
    call "%~dp0check-coverage.bat"
    set TEST_RESULT=%ERRORLEVEL%
)

if NOT DEFINED TEST_RESULT (
    echo [ERROR] Invalid choice.
    exit /b 1
)

echo.
if %TEST_RESULT% EQU 0 (
    echo ============================================================================
    echo  All Tests Passed!
    echo ============================================================================
    echo.
) else (
    echo ============================================================================
    echo  Tests Failed!
    echo ============================================================================
    echo.
    exit /b 1
)

exit /b 0
