@echo off
REM ============================================================================
REM Project Validation Script
REM ============================================================================
REM This script performs comprehensive pre-commit validation
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Project Validation
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

set "VALIDATION_PASSED=1"

REM Step 1: Check Database Connectivity
echo [STEP 1/5] Checking database connectivity...
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -Q "SELECT COUNT(*) FROM SensitiveWords" -b >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Cannot connect to database.
    echo [INFO] Run '.\SpecialScripts\setup-database.bat' to set up the database.
    set "VALIDATION_PASSED=0"
) else (
    echo [SUCCESS] Database connection verified.
)
echo.

REM Step 2: Build Solution
echo [STEP 2/5] Building solution...
dotnet build --configuration Release --nologo --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed. Please fix compilation errors.
    set "VALIDATION_PASSED=0"
) else (
    echo [SUCCESS] Build completed successfully.
)
echo.

REM Step 3: Run All Tests
echo [STEP 3/5] Running all tests...
dotnet test --configuration Release --no-build --nologo --verbosity minimal
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Tests failed. Please fix failing tests.
    set "VALIDATION_PASSED=0"
) else (
    echo [SUCCESS] All tests passed.
)
echo.

REM Step 4: Check Code Coverage
echo [STEP 4/5] Checking code coverage...
if exist TestResults (
    rmdir /s /q TestResults 2>nul
)

dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory:./TestResults --nologo --verbosity quiet >nul 2>&1

REM Check for reportgenerator tool
dotnet tool list -g | findstr reportgenerator >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [INFO] Installing reportgenerator tool...
    dotnet tool install -g dotnet-reportgenerator-globaltool >nul 2>&1
)

reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"TextSummary" -verbosity:Error >nul 2>&1

if exist TestResults\CoverageReport\Summary.txt (
    REM Extract line coverage percentage
    for /f "tokens=3 delims=: " %%a in ('findstr "Line coverage" TestResults\CoverageReport\Summary.txt') do set COVERAGE=%%a
    set COVERAGE=!COVERAGE:%%=!

    REM Remove percentage and get numeric value
    for /f "tokens=1 delims=%%" %%a in ("!COVERAGE!") do set COVERAGE_NUM=%%a

    echo [INFO] Current code coverage: !COVERAGE!%%

    REM Check if coverage is above 85%
    if !COVERAGE_NUM! LSS 85 (
        echo [ERROR] Code coverage is below 85%% threshold.
        set "VALIDATION_PASSED=0"
    ) else (
        echo [SUCCESS] Code coverage meets 85%% threshold.
    )
) else (
    echo [WARNING] Could not generate coverage report.
)
echo.

REM Step 5: Validate API Health (Optional - if API is running)
echo [STEP 5/5] Validating project structure...
if not exist "SensitiveWords.MicroService\SensitiveWords.API" (
    echo [ERROR] API project not found.
    set "VALIDATION_PASSED=0"
) else if not exist "SensitiveWords.MicroService\SensitiveWords.Domain" (
    echo [ERROR] Domain project not found.
    set "VALIDATION_PASSED=0"
) else if not exist "SensitiveWords.MicroService\SensitiveWords.Application" (
    echo [ERROR] Application project not found.
    set "VALIDATION_PASSED=0"
) else if not exist "SensitiveWords.MicroService\SensitiveWords.Infrastructure" (
    echo [ERROR] Infrastructure project not found.
    set "VALIDATION_PASSED=0"
) else (
    echo [SUCCESS] Project structure validated.
)
echo.

REM Final Result
echo ============================================================================
if "%VALIDATION_PASSED%"=="1" (
    echo  PROJECT VALIDATION PASSED
    echo ============================================================================
    echo.
    echo  Your project is ready for commit!
    echo.
    exit /b 0
) else (
    echo  PROJECT VALIDATION FAILED
    echo ============================================================================
    echo.
    echo  Please fix the errors above before committing.
    echo.
    exit /b 1
)
