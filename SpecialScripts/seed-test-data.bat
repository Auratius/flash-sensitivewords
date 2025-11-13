@echo off
REM ============================================================================
REM Seed Test Data Script
REM ============================================================================
REM This script adds sample test data to the database
REM ============================================================================

echo.
echo ============================================================================
echo  Flash.SensitiveWords - Seed Test Data
echo ============================================================================
echo.

REM Navigate to solution root
cd /d "%~dp0.."

REM Set variables
set "SQL_SERVER=localhost\SQLEXPRESS"
set "DB_NAME=SensitiveWordsDb"

echo [INFO] This will add additional test data to the database.
echo        Server: %SQL_SERVER%
echo        Database: %DB_NAME%
echo.

REM Check if sqlcmd is available
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] sqlcmd not found. Please install SQL Server Command Line Tools.
    exit /b 1
)

REM Check SQL Server connectivity
echo [INFO] Checking database connectivity...
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -Q "SELECT COUNT(*) FROM SensitiveWords" -b >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Cannot connect to database.
    echo [INFO] Run '.\SpecialScripts\setup-database.bat' to set up the database.
    exit /b 1
)
echo [SUCCESS] Database connection verified.
echo.

echo Select test data set to seed:
echo.
echo  [1] Additional SQL Keywords (10 words)
echo  [2] Extended SQL Keywords (20 words)
echo  [3] Edge Case Test Data (special characters, lengths)
echo  [4] Performance Test Data (100 words)
echo  [5] All of the above
echo  [0] Cancel
echo.
set /p CHOICE="Enter your choice (0-5): "

if "%CHOICE%"=="0" (
    echo [INFO] Seeding cancelled.
    exit /b 0
)

echo.
echo ============================================================================
echo  Seeding Test Data...
echo ============================================================================
echo.

REM Create temporary SQL file
set "TEMP_SQL=%TEMP%\seed_test_data_%RANDOM%.sql"

(
echo USE [SensitiveWordsDb]
echo GO
echo.
) > "%TEMP_SQL%"

if "%CHOICE%"=="1" (
    echo [INFO] Seeding additional SQL keywords...
    (
    echo -- Additional SQL Keywords
    echo INSERT INTO SensitiveWords (Word^) VALUES
    echo ('TRUNCATE'^),('GRANT'^),('REVOKE'^),('ROLLBACK'^),('COMMIT'^),
    echo ('BEGIN'^),('TRANSACTION'^),('EXEC'^),('EXECUTE'^),('PROCEDURE'^);
    ) >> "%TEMP_SQL%"
)

if "%CHOICE%"=="2" (
    echo [INFO] Seeding extended SQL keywords...
    (
    echo -- Extended SQL Keywords
    echo INSERT INTO SensitiveWords (Word^) VALUES
    echo ('TRUNCATE'^),('GRANT'^),('REVOKE'^),('ROLLBACK'^),('COMMIT'^),
    echo ('BEGIN'^),('TRANSACTION'^),('EXEC'^),('EXECUTE'^),('PROCEDURE'^),
    echo ('TRIGGER'^),('VIEW'^),('INDEX'^),('SCHEMA'^),('DATABASE'^),
    echo ('BACKUP'^),('RESTORE'^),('MERGE'^),('PIVOT'^),('UNPIVOT'^);
    ) >> "%TEMP_SQL%"
)

if "%CHOICE%"=="3" (
    echo [INFO] Seeding edge case test data...
    (
    echo -- Edge Case Test Data
    echo INSERT INTO SensitiveWords (Word^) VALUES
    echo ('X'^),('AB'^),('VERYLONGKEYWORDTHATEXCEEDSNORMALLENGTH'^),
    echo ('WORD-WITH-DASH'^),('WORD_UNDERSCORE'^),('WORD.DOT'^),
    echo ('123NUMERIC'^),('SPECIAL!@#'^),('MixedCASE'^),('lowercase'^);
    ) >> "%TEMP_SQL%"
)

if "%CHOICE%"=="4" (
    echo [INFO] Seeding performance test data (100 words^)...
    (
    echo -- Performance Test Data (100 words^)
    echo DECLARE @Counter INT = 1;
    echo WHILE @Counter ^<= 100
    echo BEGIN
    echo     INSERT INTO SensitiveWords (Word^)
    echo     VALUES ('PERFTEST' + RIGHT('000' + CAST(@Counter AS VARCHAR(3^)^), 3^)^);
    echo     SET @Counter = @Counter + 1;
    echo END;
    ) >> "%TEMP_SQL%"
)

if "%CHOICE%"=="5" (
    echo [INFO] Seeding all test data sets...
    (
    echo -- Additional SQL Keywords
    echo INSERT INTO SensitiveWords (Word^) VALUES
    echo ('TRUNCATE'^),('GRANT'^),('REVOKE'^),('ROLLBACK'^),('COMMIT'^),
    echo ('BEGIN'^),('TRANSACTION'^),('EXEC'^),('EXECUTE'^),('PROCEDURE'^),
    echo ('TRIGGER'^),('VIEW'^),('INDEX'^),('SCHEMA'^),('DATABASE'^),
    echo ('BACKUP'^),('RESTORE'^),('MERGE'^),('PIVOT'^),('UNPIVOT'^);
    echo.
    echo -- Edge Case Test Data
    echo INSERT INTO SensitiveWords (Word^) VALUES
    echo ('X'^),('AB'^),('VERYLONGKEYWORDTHATEXCEEDSNORMALLENGTH'^),
    echo ('WORD-WITH-DASH'^),('WORD_UNDERSCORE'^),('WORD.DOT'^),
    echo ('123NUMERIC'^),('SPECIAL!@#'^),('MixedCASE'^),('lowercase'^);
    echo.
    echo -- Performance Test Data (100 words^)
    echo DECLARE @Counter INT = 1;
    echo WHILE @Counter ^<= 100
    echo BEGIN
    echo     BEGIN TRY
    echo         INSERT INTO SensitiveWords (Word^)
    echo         VALUES ('PERFTEST' + RIGHT('000' + CAST(@Counter AS VARCHAR(3^)^), 3^)^);
    echo     END TRY
    echo     BEGIN CATCH
    echo         -- Ignore duplicates
    echo     END CATCH;
    echo     SET @Counter = @Counter + 1;
    echo END;
    ) >> "%TEMP_SQL%"
)

REM Execute the SQL
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -i "%TEMP_SQL%" -b
set SQL_RESULT=%ERRORLEVEL%

REM Clean up temp file
del "%TEMP_SQL%" 2>nul

if %SQL_RESULT% NEQ 0 (
    echo [WARNING] Some inserts may have failed (possibly due to duplicates).
) else (
    echo [SUCCESS] Test data seeded successfully.
)
echo.

REM Show current count
echo [INFO] Checking current word count...
sqlcmd -S %SQL_SERVER% -d %DB_NAME% -Q "SELECT COUNT(*) AS TotalWords FROM SensitiveWords" -b
echo.

echo ============================================================================
echo  Test Data Seeding Complete!
echo ============================================================================
echo.
echo You can now test the API with a variety of sensitive words.
echo.

exit /b 0
