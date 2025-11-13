@echo off
REM ============================================================================
REM Developer Tools Installation Script
REM ============================================================================
REM This script installs all required development tools
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Install Development Tools
echo ============================================================================
echo.

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found. Please install .NET 8.0 SDK first.
    echo [INFO] Download from: https://dotnet.microsoft.com/download/dotnet/8.0
    exit /b 1
)

echo [INFO] .NET SDK detected. Proceeding with tool installation...
echo.

REM Tool 1: ReportGenerator (Code Coverage)
echo [TOOL 1/3] Installing ReportGenerator (Code Coverage Tool)...
dotnet tool list -g | findstr reportgenerator >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [INFO] ReportGenerator is already installed.
    set /p UPDATE_RG="Would you like to update it? (Y/N): "
    if /i "!UPDATE_RG!"=="Y" (
        dotnet tool update -g dotnet-reportgenerator-globaltool
        if %ERRORLEVEL% EQU 0 (
            echo [SUCCESS] ReportGenerator updated successfully.
        )
    )
) else (
    dotnet tool install -g dotnet-reportgenerator-globaltool
    if %ERRORLEVEL% EQU 0 (
        echo [SUCCESS] ReportGenerator installed successfully.
    ) else (
        echo [ERROR] Failed to install ReportGenerator.
    )
)
echo.

REM Tool 2: dotnet-format (Code Formatting)
echo [TOOL 2/3] Installing dotnet-format (Code Formatting Tool)...
dotnet tool list -g | findstr dotnet-format >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [INFO] dotnet-format is already installed.
    set /p UPDATE_FMT="Would you like to update it? (Y/N): "
    if /i "!UPDATE_FMT!"=="Y" (
        dotnet tool update -g dotnet-format
        if %ERRORLEVEL% EQU 0 (
            echo [SUCCESS] dotnet-format updated successfully.
        )
    )
) else (
    dotnet tool install -g dotnet-format
    if %ERRORLEVEL% EQU 0 (
        echo [SUCCESS] dotnet-format installed successfully.
    ) else (
        echo [WARNING] Failed to install dotnet-format (may not be needed for .NET 8).
        echo [INFO] Code formatting is built into 'dotnet format' command.
    )
)
echo.

REM Tool 3: dotnet-outdated (Dependency Updates)
echo [TOOL 3/3] Installing dotnet-outdated (Dependency Update Checker)...
dotnet tool list -g | findstr dotnet-outdated >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [INFO] dotnet-outdated is already installed.
    set /p UPDATE_OUT="Would you like to update it? (Y/N): "
    if /i "!UPDATE_OUT!"=="Y" (
        dotnet tool update -g dotnet-outdated-tool
        if %ERRORLEVEL% EQU 0 (
            echo [SUCCESS] dotnet-outdated updated successfully.
        )
    )
) else (
    dotnet tool install -g dotnet-outdated-tool
    if %ERRORLEVEL% EQU 0 (
        echo [SUCCESS] dotnet-outdated installed successfully.
    ) else (
        echo [WARNING] Failed to install dotnet-outdated.
    )
)
echo.

REM Check SQL Server Command Line Tools
echo [CHECK] SQL Server Command Line Tools...
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] sqlcmd not found.
    echo [INFO] Please install SQL Server Command Line Tools manually.
    echo [INFO] Download from: https://aka.ms/sqlcmd
    echo.
) else (
    echo [SUCCESS] sqlcmd is installed.
    echo.
)

REM Summary
echo ============================================================================
echo  Tool Installation Summary
echo ============================================================================
echo.
echo Installed tools:
dotnet tool list -g
echo.

echo ============================================================================
echo  Installation Complete!
echo ============================================================================
echo.
echo Installed tools can be used:
echo  - reportgenerator: Generate coverage reports
echo  - dotnet format: Format code automatically
echo  - dotnet outdated: Check for outdated packages
echo.
echo For SQL Server tools, visit: https://aka.ms/sqlcmd
echo.

exit /b 0
