# Flash.SensitiveWords API

A .NET 8 REST API that detects and sanitizes SQL keywords from messages. Think of it as a filter that catches dangerous SQL words like SELECT, DROP, DELETE, etc.

## What it does

- Scans messages for SQL keywords
- Replaces found keywords with asterisks
- Manages a database of sensitive words
- Tracks operation statistics (CREATE, READ, UPDATE, DELETE, SANITIZE)
- Provides health monitoring and metrics

## Quick Start

The easiest way to get started:

```bash
# Run this from the project root
.\SpecialScripts\start-fullstack-windows.bat
```

This will:
- Check your database
- Build the API
- Start everything in separate windows
- Open your browser automatically

**URLs:**
- API: https://localhost:64725/swagger
- React UI: http://localhost:5173

## What you need

- .NET 8 SDK
- SQL Server (or SQL Express)
- That's it!

## Project Stats

We've got excellent test coverage:
- 133 tests total (78 unit + 55 integration)
- 100% pass rate
- 100% coverage on all controllers
- 100% coverage on the application layer

## How it's built

This uses Clean Architecture with:
- **Domain Layer** - The business logic (what is a sensitive word?)
- **Application Layer** - Use cases (sanitize this message, add that word)
- **Infrastructure Layer** - Database stuff (Dapper for speed)
- **API Layer** - REST endpoints you can call

It also uses CQRS, which just means we separate reading data from writing data. Makes things faster and cleaner.

## Helpful Scripts

We made a bunch of scripts to make your life easier. They're all in the `SpecialScripts` folder.

**To start everything:**
```bash
.\SpecialScripts\start-fullstack-windows.bat
```

**First time setup:**
```bash
.\SpecialScripts\setup-database.bat
```

**Run tests:**
```bash
.\SpecialScripts\run-tests.bat
```

**Check if everything's working:**
```bash
.\SpecialScripts\check-health.bat
```

**Before you commit:**
```bash
.\SpecialScripts\validate-project.bat
```

## API Examples

**Sanitize a message:**
```http
POST /api/sanitize
Content-Type: application/json

{
  "message": "SELECT * FROM users WHERE id=1"
}
```

**Get all sensitive words:**
```http
GET /api/sensitivewords
```

**Add a new word:**
```http
POST /api/sensitivewords
Content-Type: application/json

{
  "word": "TRUNCATE"
}
```

**Get operation statistics:**
```http
GET /api/statistics
```

**Get statistics by operation type:**
```http
GET /api/statistics/SANITIZE
```

**Reset statistics:**
```http
POST /api/statistics/reset
```

## Database Setup

The scripts handle this, but if you want to do it manually:

```bash
cd SensitiveWords.MicroService/Database/Scripts
sqlcmd -S localhost\SQLEXPRESS -i 01_CreateDatabase.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 02_CreateTables.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 03_SeedData.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 04_CreateStoredProcedures.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 05_CreateOperationStatsTable.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 06_CreateOperationStatsStoredProcs.sql
```

## Common Issues

**Can't connect to database?**
Run `.\SpecialScripts\setup-database.bat`

**Tests failing?**
Make sure your database is set up and running.

**Build errors?**
Try `.\SpecialScripts\clean-build.bat`

**API won't start?**
Check if something's already using ports 64725 or 64726.

## Architecture Deep Dive

Want to learn more about how everything fits together? Check out the [Architecture Documentation](../Documentation/UMLDiagrams.md).

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Dapper (not Entity Framework - we like speed)
- SQL Server
- xUnit for testing
- Serilog for logging

That's pretty much it. We keep it simple.
