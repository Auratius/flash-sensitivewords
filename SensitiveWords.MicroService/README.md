# Flash.SensitiveWords Microservice

## Overview

This is the main microservice implementation for the Flash.SensitiveWords SQL keyword sanitization system. It provides a REST API for sanitizing sensitive SQL keywords from messages and managing the sensitive words database.

**Current Status:**
- **Test Coverage: 85.6%** (495 of 578 coverable lines)
- **Total Tests: 64** (53 unit tests + 11 integration tests)
- **Application Layer: 100%** coverage
- **Domain Layer: 94.5%** coverage
- **Infrastructure Layer: 77.7%** coverage
- **API Layer: 80%** coverage

## Architecture

This microservice is built using:
- **Domain-Driven Design (DDD)** - Clean separation of domain logic
- **CQRS (Command Query Responsibility Segregation)** - Separate read and write operations
- **Microservice Architecture** - Independent, deployable service
- **Dapper** - Lightweight ORM for high-performance data access (No Entity Framework)

**For detailed architectural documentation, see:**
- [Architecture & UML Diagrams](../Documentation/UMLDiagrams.md) - Comprehensive architectural explanations, layer descriptions, and system flows

## Project Structure

```
flash-sensitivewords/
├── SensitiveWords.MicroService/
│   ├── SensitiveWords.API/              # API Layer (REST endpoints, Swagger, Middleware)
│   ├── SensitiveWords.Application/      # Application Layer (CQRS handlers, DTOs, Commands, Queries)
│   ├── SensitiveWords.Domain/           # Domain Layer (Entities, Value Objects, Services)
│   ├── SensitiveWords.Infrastructure/   # Infrastructure Layer (Dapper repositories, DapperContext)
│   ├── SensitiveWords.Tests.Unit/       # Unit Tests (53 tests, mocked dependencies)
│   ├── SensitiveWords.Tests.Integration/# Integration Tests (11 tests, WebApplicationFactory)
│   └── Database/Scripts/                # SQL migration scripts and stored procedures
├── SpecialScripts/                       # Developer helper scripts
│   ├── setup-database.bat               # Automated database setup
│   ├── reset-database.bat               # Drop and recreate database
│   ├── run-api.bat                      # Build and run API
│   ├── clean-build.bat                  # Clean build with validation
│   ├── run-tests.bat                    # Interactive test runner
│   ├── validate-project.bat             # Pre-commit validation
│   ├── check-coverage.bat               # Code coverage analysis
│   ├── generate-migration.bat           # Create migration scripts
│   ├── backup-database.bat              # Database backup utility
│   ├── check-health.bat                 # System health check
│   ├── install-tools.bat                # Install dev tools
│   ├── view-logs.bat                    # Log viewer utility
│   └── seed-test-data.bat               # Add test data
└── TestResults/                         # Generated coverage reports
    └── CoverageReport/                  # HTML coverage report
```

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server 2019+ or SQL Server Express
- Visual Studio 2022 / VS Code / Rider (optional)
- ReportGenerator tool (for coverage reports): `dotnet tool install -g dotnet-reportgenerator-globaltool`

### Local Development (Using Helper Scripts - Recommended)

```bash
# 1. Install development tools
.\SpecialScripts\install-tools.bat

# 2. Check system health
.\SpecialScripts\check-health.bat

# 3. Setup database (automated)
.\SpecialScripts\setup-database.bat

# 4. Run the API
.\SpecialScripts\run-api.bat
```

**Access:**
- Main API: https://localhost:64725 (or http://localhost:64726)
- Swagger UI: https://localhost:64725/swagger
- Health Check: https://localhost:64725/health

### Manual Setup (Alternative)

```bash
# 1. Setup database manually
cd SensitiveWords.MicroService/Database/Scripts
sqlcmd -S localhost\SQLEXPRESS -i 01_CreateDatabase.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 02_CreateTables.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 04_CreateStoredProcedures.sql
sqlcmd -S localhost\SQLEXPRESS -d SensitiveWordsDb -i 03_SeedData.sql

# 2. Build and run manually
cd ../SensitiveWords.API
dotnet restore
dotnet build
dotnet run
```

## Developer Helper Scripts

The `SpecialScripts/` folder contains automated scripts to streamline development workflows:

### Database Management

**setup-database.bat** - Automated database setup
- Creates database, tables, stored procedures, and seeds initial data
- Validates connectivity before starting
- Runs all SQL scripts in correct order
```bash
.\SpecialScripts\setup-database.bat
```

**reset-database.bat** - Complete database reset
- Drops existing database and recreates it from scratch
- Useful for clean testing or resolving database issues
- **Warning:** Deletes all data
```bash
.\SpecialScripts\reset-database.bat
```

**backup-database.bat** - Create timestamped backups
- Backs up database to `DatabaseBackups/` folder
- Includes timestamp in filename
- Shows backup size and location
```bash
.\SpecialScripts\backup-database.bat
```

**generate-migration.bat** - Create migration scripts
- Generates timestamped SQL migration file with template
- Includes rollback script section
- Opens file in notepad for editing
```bash
.\SpecialScripts\generate-migration.bat
```

**seed-test-data.bat** - Add test data
- Multiple test data sets (SQL keywords, edge cases, performance data)
- Interactive menu to choose data sets
- Safe to run multiple times (handles duplicates)
```bash
.\SpecialScripts\seed-test-data.bat
```

### Build and Testing

**clean-build.bat** - Clean build with validation
- Removes bin/obj folders
- Restores NuGet packages
- Builds in Release mode
- Optional quick test run
```bash
.\SpecialScripts\clean-build.bat
```

**run-tests.bat** - Interactive test runner
- Unit tests only (fast)
- Integration tests only
- All tests with various verbosity levels
- All tests with code coverage
```bash
.\SpecialScripts\run-tests.bat
```

**check-coverage.bat** - Code coverage analysis
- Runs all tests with coverage collection
- Generates HTML report with detailed statistics
- Shows summary in console
- Opens report in browser
```bash
.\SpecialScripts\check-coverage.bat
```

**validate-project.bat** - Pre-commit validation
- Checks database connectivity
- Builds solution
- Runs all tests
- Validates 85%+ code coverage
- Checks project structure
- Returns pass/fail status
```bash
.\SpecialScripts\validate-project.bat
```

### Runtime and Monitoring

**run-api.bat** - Start the API
- Checks database connectivity
- Builds solution
- Runs API with formatted output
- Shows all access URLs
```bash
.\SpecialScripts\run-api.bat
```

**check-health.bat** - System health check
- Validates .NET SDK installation
- Checks SQL Server connectivity
- Verifies database existence
- Tests solution build
- Checks required tools
- Validates project structure
- Tests API endpoints (if running)
```bash
.\SpecialScripts\check-health.bat
```

**view-logs.bat** - Log viewer utility
- View latest log file
- View all logs concatenated
- Search logs by keyword
- View errors only
- Tail logs in real-time
- Clear old logs
```bash
.\SpecialScripts\view-logs.bat
```

### Development Tools

**install-tools.bat** - Install development tools
- Installs reportgenerator (code coverage)
- Installs dotnet-format (code formatting)
- Installs dotnet-outdated (dependency updates)
- Updates existing tools
```bash
.\SpecialScripts\install-tools.bat
```

### Recommended Workflow

**First-time setup:**
```bash
.\SpecialScripts\install-tools.bat
.\SpecialScripts\check-health.bat
.\SpecialScripts\setup-database.bat
.\SpecialScripts\run-api.bat
```

**Daily development:**
```bash
.\SpecialScripts\check-health.bat
.\SpecialScripts\run-tests.bat
```

**Before committing:**
```bash
.\SpecialScripts\validate-project.bat
```

**Troubleshooting:**
```bash
.\SpecialScripts\check-health.bat
.\SpecialScripts\view-logs.bat
.\SpecialScripts\reset-database.bat
```

## API Endpoints

### Business Logic (External)
- `POST /api/sanitize` - Sanitize message (replace SQL keywords with asterisks)

### CRUD Operations (Internal)
- `GET /api/sensitivewords` - Get all sensitive words
- `GET /api/sensitivewords/{id}` - Get word by ID
- `POST /api/sensitivewords` - Create new word
- `PUT /api/sensitivewords/{id}` - Update word
- `DELETE /api/sensitivewords/{id}` - Delete word

### Monitoring
- `GET /health` - Overall health status (checks database connectivity)
- `GET /metrics` - Performance metrics (request count, duration, active sanitizations)

## Testing

### Quick Test Commands

```bash
# Build the solution
dotnet build

# Run all tests (from solution root)
dotnet test

# Run only unit tests
dotnet test SensitiveWords.Tests.Unit

# Run only integration tests
dotnet test SensitiveWords.Tests.Integration
```

### Code Coverage

**Automated Coverage Script (Recommended):**

```bash
# From the project root, run the automated coverage script
.\SpecialScripts\check-coverage.bat

# Options when prompted:
# - View summary in console
# - Open HTML report in browser
# - Both
```

This script automatically:
1. Cleans previous test results
2. Runs both unit and integration tests with coverage collection
3. Merges coverage from all test projects
4. Generates HTML, text, and badge reports
5. Displays summary statistics

**Manual Coverage Commands:**

```bash
# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults"

# Generate detailed HTML report
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary;Badges"

# View summary
cat TestResults/CoverageReport/Summary.txt

# Open HTML report
start TestResults/CoverageReport/index.html
```

### Current Coverage Statistics

| Layer | Coverage | Details |
|-------|----------|---------|
| **Overall** | **85.6%** | 495 of 578 coverable lines |
| Application | 100% | All handlers, commands, queries, DTOs |
| Domain | 94.5% | Entities, value objects, services |
| Infrastructure | 77.7% | Dapper repositories (requires real DB) |
| API | 80% | Controllers and middleware |

**Branch Coverage:** 71.6% (149 of 208 branches)
**Method Coverage:** 97.4% (77 of 79 methods)

### Test Organization

**Unit Tests (53 tests):**
- `Application/Handlers/` - CQRS handler tests with mocked dependencies
- `Domain/Entities/` - Entity validation and behavior tests
- `Domain/Services/` - SanitizationService logic tests
- `Domain/ValueObjects/` - Value object immutability tests

**Integration Tests (11 tests):**
- `API/SanitizeControllerTests.cs` - End-to-end sanitization API tests
- `API/SensitiveWordsControllerTests.cs` - CRUD operations with real database

## Database

### Database: `SensitiveWordsDb`

**Tables:**

```sql
CREATE TABLE SensitiveWords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Word NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE UNIQUE INDEX IX_SensitiveWords_Word ON SensitiveWords(Word);
CREATE INDEX IX_SensitiveWords_IsActive ON SensitiveWords(IsActive);
```

**Stored Procedures:**

The application uses stored procedures for all database operations:
- `sp_GetAllSensitiveWords` - Retrieve all words with optional active filtering
- `sp_GetSensitiveWordById` - Get word by GUID
- `sp_CreateSensitiveWord` - Insert new word with duplicate checking
- `sp_UpdateSensitiveWord` - Update word and set UpdatedAt timestamp
- `sp_DeleteSensitiveWord` - Soft delete by setting IsActive = 0

This approach provides:
- Better performance through query plan caching
- Enhanced security (SQL injection prevention)
- Centralized business logic
- Easier database optimization

## Configuration

### Connection Strings

**Development (appsettings.Development.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SensitiveWordsDb;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

### Logging Configuration

**Serilog** is configured for structured logging with:
- Console output (development)
- File output: `Logs/log-.txt` (rolling daily)
- Minimum level: Information
- SQL enrichment for performance tracking

### Health Checks

Health checks are configured to monitor:
- Database connectivity (SQL Server ping)
- Overall application health status

Access at: `https://localhost:64725/health`

## Technology Stack

**Core Framework:**
- .NET 8.0
- ASP.NET Core Web API
- C# 12

**Data Access:**
- Dapper 2.1.35 (Lightweight ORM)
- SQL Server 2019+ / SQL Server Express
- Stored Procedures for all CRUD operations

**Testing:**
- xUnit 2.9.2 (Test framework)
- Moq 4.20.72 (Mocking framework)
- Microsoft.AspNetCore.Mvc.Testing (Integration testing)
- Coverlet.collector (Code coverage)
- ReportGenerator (Coverage reports)

**Logging & Monitoring:**
- Serilog 8.0.3 (Structured logging)
- AspNetCore.HealthChecks.SqlServer (Health monitoring)

**API Documentation:**
- Swashbuckle.AspNetCore 7.2.0 (Swagger/OpenAPI)

## Key Features

### Architecture Patterns
- **Clean Architecture** - Clear separation of concerns across 4 layers
- **Domain-Driven Design (DDD)** - Rich domain models with business logic
- **CQRS Pattern** - Separated commands and queries with dedicated handlers
- **Repository Pattern** - Abstracted data access through interfaces

### Performance
- **Dapper ORM** - Lightweight, high-performance data access
- **Stored Procedures** - Optimized database queries with plan caching
- **Async/Await** - Fully asynchronous operations throughout
- **Connection Pooling** - Efficient database connection management

### Quality
- **85.6% Code Coverage** - Comprehensive test suite (64 tests)
- **100% Application Layer Coverage** - All business logic tested
- **Unit & Integration Tests** - Both isolated and end-to-end testing
- **Automated Coverage Script** - Easy coverage reporting

### Observability
- **Structured Logging** - Serilog with file and console sinks
- **Health Checks** - Database connectivity monitoring
- **Performance Metrics** - Request tracking middleware
- **Swagger UI** - Interactive API documentation

### Security
- **Parameterized Queries** - SQL injection prevention via stored procedures
- **Input Validation** - Data annotations on all DTOs
- **HTTPS Enforcement** - TLS required in production
- **Unique Constraints** - Database-level duplicate prevention

## Example Usage

### Sanitize a Message

**Request:**
```http
POST /api/sanitize
Content-Type: application/json

{
  "message": "SELECT * FROM users WHERE password = 'admin'"
}
```

**Response:**
```json
{
  "originalMessage": "SELECT * FROM users WHERE password = 'admin'",
  "sanitizedMessage": "****** * **** users WHERE password = 'admin'",
  "detectedWords": ["SELECT", "FROM"],
  "replacementCount": 2
}
```

### Manage Sensitive Words

**Create Word:**
```http
POST /api/sensitivewords
Content-Type: application/json

{
  "word": "DELETE"
}
```

**Update Word:**
```http
PUT /api/sensitivewords/{id}
Content-Type: application/json

{
  "word": "DELETE",
  "isActive": false
}
```

**Get All Words:**
```http
GET /api/sensitivewords?activeOnly=true
```

## Development Guidelines

### Adding New Sensitive Words
1. New words are automatically activated (`IsActive = true`)
2. Words are case-insensitive during sanitization
3. Duplicate words are rejected at database level
4. All operations are logged for auditing

### Testing Best Practices
- Run `.\SpecialScripts\check-coverage.bat` before committing
- Maintain 85%+ overall coverage
- Write integration tests for new endpoints
- Use unique test data (GUIDs) to avoid conflicts

### Code Structure
```
Domain/
  ├── Entities/          # Business entities with validation
  ├── ValueObjects/      # Immutable value objects
  └── Services/          # Domain services (SanitizationService)

Application/
  ├── Commands/          # Write operations
  ├── Queries/           # Read operations
  ├── Handlers/          # Command/Query handlers
  └── DTOs/              # Data transfer objects

Infrastructure/
  ├── Data/              # DapperContext
  └── Repositories/      # Data access implementations

API/
  ├── Controllers/       # REST endpoints
  └── Middleware/        # Performance metrics, error handling
```

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.Development.json`
- Ensure database and tables exist (run migration scripts)
- Test connection: `sqlcmd -S localhost\SQLEXPRESS -Q "SELECT @@VERSION"`

### Test Failures
- Integration tests require database to be set up
- Clean test data: Delete and recreate database if needed
- Check for unique constraint violations in tests
- Verify all stored procedures exist

### Coverage Report Not Generated
- Install ReportGenerator: `dotnet tool install -g dotnet-reportgenerator-globaltool`
- Verify tool is in PATH: `reportgenerator --version`
- Clean TestResults folder before running

## Author

**Auratius February**
Email: auratius@gmail.com

---

**Flash.SensitiveWords Microservice** - Enterprise-grade SQL keyword sanitization with .NET 8, Clean Architecture, and comprehensive test coverage.
