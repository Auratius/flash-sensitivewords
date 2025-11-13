@echo off
REM ============================================================================
REM Clean Build Script
REM ============================================================================
REM This script performs a clean build with validation
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Clean Build
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Step 1: Clean previous builds
echo [STEP 1/4] Cleaning previous builds...
dotnet clean --configuration Release --nologo --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Clean failed.
    exit /b 1
)
echo [SUCCESS] Clean completed.
echo.

REM Step 2: Remove bin and obj folders
echo [STEP 2/4] Removing bin and obj folders...
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s /q "%%d" 2>nul
echo [SUCCESS] Folders removed.
echo.

REM Step 3: Restore NuGet packages
echo [STEP 3/4] Restoring NuGet packages...
dotnet restore --nologo --verbosity quiet
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Package restore failed.
    exit /b 1
)
echo [SUCCESS] Packages restored.
echo.

REM Step 4: Build solution
echo [STEP 4/4] Building solution in Release mode...
dotnet build --configuration Release --no-restore --nologo
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed. Please fix compilation errors.
    exit /b 1
)
echo [SUCCESS] Build completed successfully.
echo.

REM Optional: Run quick smoke test
set /p RUN_TESTS="Would you like to run quick tests? (Y/N): "
if /i "%RUN_TESTS%"=="Y" (
    echo.
    echo [INFO] Running unit tests...
    dotnet test SensitiveWords.MicroService\SensitiveWords.Tests.Unit --configuration Release --no-build --nologo --verbosity minimal
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Tests failed.
        exit /b 1
    )
    echo [SUCCESS] Tests passed.
)

echo.
echo ============================================================================
echo  Clean Build Completed Successfully!
echo ============================================================================
echo.

exit /b 0
