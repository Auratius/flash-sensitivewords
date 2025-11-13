# UML Diagrams - Flash.SensitiveWords Microservice

This document contains comprehensive architectural explanations for the Flash.SensitiveWords microservice solution.

**Project**: Flash.SensitiveWords SQL Keyword Sanitization System
**Architecture**: Clean Architecture with DDD and CQRS
**Test Coverage**: 85.6% (64 tests: 53 unit + 11 integration)
**Technology**: .NET 8, Dapper, SQL Server, xUnit

## Table of Contents
1. [Domain Layer Architecture](#1-domain-layer-architecture)
2. [Application Layer Architecture](#2-application-layer-architecture)
3. [Infrastructure Layer Architecture](#3-infrastructure-layer-architecture)
4. [Overall System Architecture](#4-overall-system-architecture)
5. [Message Sanitization Flow](#5-message-sanitization-flow)
6. [Word Creation Flow](#6-word-creation-flow)
7. [Deployment Architecture](#7-deployment-architecture)
8. [System Use Cases](#8-system-use-cases)

---

## 1. Domain Layer Architecture

### Overview
The Domain Layer is the heart of the application, containing pure business logic with no external dependencies. It follows Domain-Driven Design (DDD) principles and is completely framework-agnostic.

### Core Components

**Entities:**
- **SensitiveWord Entity** (Aggregate Root)
  - Properties: `Id` (Guid), `Word` (string), `IsActive` (bool), `CreatedAt`, `UpdatedAt`
  - Business Rules:
    - Word cannot be null or empty
    - Word is automatically converted to uppercase
    - Id is immutable once created
    - Timestamps are managed automatically
  - Methods: `UpdateWord()`, `Activate()`, `Deactivate()`
  - Role: Represents a sensitive SQL keyword that should be sanitized from messages

**Value Objects:**
- **SanitizedMessage** (Immutable)
  - Properties: `OriginalMessage`, `SanitizedText`, `DetectedWords` (List<string>), `ReplacementCount` (int)
  - Characteristics: No identity, equality by value, thread-safe, immutable
  - Role: Encapsulates the result of a sanitization operation

**Repository Interface:**
- **ISensitiveWordRepository**
  - Methods:
    - `GetByIdAsync(Guid id)` - Retrieve word by ID
    - `GetAllAsync()` - Retrieve all words
    - `GetByWordAsync(string word)` - Find word by text
    - `CreateAsync(string word)` - Create new word
    - `UpdateAsync(Guid id, string word, bool isActive)` - Update existing word
    - `DeleteAsync(Guid id)` - Soft delete word
  - Role: Defines the contract for data access without coupling to implementation

**Domain Service:**
- **SanitizationService** (implements ISanitizationService)
  - Dependencies: ISensitiveWordRepository
  - Key Methods:
    - `SanitizeMessageAsync(string message)` - Main sanitization logic
    - `SanitizeMessage(string message, IEnumerable<string> words)` - Synchronous version
  - Algorithm:
    1. Fetches active sensitive words from repository
    2. Sorts words by length (longest first) to handle overlapping matches
    3. Builds regex patterns with word boundaries (`\b`)
    4. Performs case-insensitive replacement
    5. Replaces each word with asterisks of equal length
    6. Tracks which words were detected and replacement count
  - Role: Core business logic for message sanitization

### Design Principles Applied
- **Aggregate Root**: SensitiveWord manages its own invariants
- **Value Object Pattern**: SanitizedMessage is immutable and has no identity
- **Repository Pattern**: Abstract data access through interfaces
- **Dependency Inversion**: Domain defines interfaces, infrastructure implements them
- **No External Dependencies**: Pure C# with no framework coupling

---

## 2. Application Layer Architecture

### Overview
The Application Layer orchestrates application flow using the CQRS (Command Query Responsibility Segregation) pattern. It separates read operations (queries) from write operations (commands), each handled by dedicated handlers.

### Data Transfer Objects (DTOs)

**Request DTOs:**
- **CreateSensitiveWordRequest**
  - Properties: `Word` (required, 1-100 characters)
  - Validation: Data annotations ensure word is not empty

- **UpdateSensitiveWordRequest**
  - Properties: `Word` (required), `IsActive` (bool)
  - Validation: Word length between 1-100 characters

- **SanitizeRequest**
  - Properties: `Message` (required)
  - Role: Contains text to be sanitized

**Response DTOs:**
- **SensitiveWordDto**
  - Properties: `Id`, `Word`, `IsActive`, `CreatedAt`, `UpdatedAt`
  - Role: Represents a sensitive word for API responses

- **SanitizeResponse**
  - Properties: `OriginalMessage`, `SanitizedMessage`, `DetectedWords`, `ReplacementCount`
  - Role: Contains sanitization results with before/after comparison

### Commands (Write Operations)

**CreateSensitiveWordCommand:**
- Input: `Word` (string)
- Handler: CreateSensitiveWordHandler
- Process:
  1. Validates word doesn't already exist
  2. Creates new SensitiveWord entity
  3. Persists to database via repository
  4. Returns new Guid
- Returns: `Guid` (ID of created word)

**UpdateSensitiveWordCommand:**
- Input: `Id` (Guid), `Word` (string), `IsActive` (bool)
- Handler: UpdateSensitiveWordHandler
- Process:
  1. Retrieves existing word by ID
  2. Updates word text and active status
  3. Persists changes
- Returns: `bool` (success/failure)

**DeleteSensitiveWordCommand:**
- Input: `Id` (Guid)
- Handler: DeleteSensitiveWordHandler
- Process:
  1. Soft deletes word (sets IsActive = false)
  2. Updates timestamp
- Returns: `bool` (success/failure)

### Queries (Read Operations)

**GetAllSensitiveWordsQuery:**
- Input: `ActiveOnly` (bool?, optional filter)
- Handler: GetAllSensitiveWordsHandler
- Process:
  1. Retrieves words from repository
  2. Optionally filters by IsActive status
  3. Maps entities to DTOs
- Returns: `IEnumerable<SensitiveWordDto>`

**GetSensitiveWordByIdQuery:**
- Input: `Id` (Guid)
- Handler: GetSensitiveWordByIdHandler
- Process:
  1. Retrieves specific word by ID
  2. Maps entity to DTO
- Returns: `SensitiveWordDto` or null

**SanitizeMessageQuery:**
- Input: `Message` (string)
- Handler: SanitizeMessageHandler
- Process:
  1. Delegates to ISanitizationService
  2. Gets SanitizedMessage value object
  3. Maps to SanitizeResponse DTO
- Returns: `SanitizeResponse`

### Handler Responsibilities
- **Input Validation**: Ensure commands/queries have valid data
- **Business Rule Enforcement**: Check preconditions (e.g., duplicate words)
- **Orchestration**: Coordinate between domain services and repositories
- **DTO Mapping**: Transform domain entities to/from DTOs
- **Error Handling**: Throw appropriate exceptions (InvalidOperationException for business rule violations)

### CQRS Benefits
- **Separation of Concerns**: Read and write operations are independent
- **Scalability**: Queries and commands can be optimized separately
- **Testability**: Each handler has a single responsibility
- **Clarity**: Intent is clear from command/query names

---

## 3. Infrastructure Layer Architecture

### Overview
The Infrastructure Layer provides concrete implementations of domain interfaces, specifically focusing on data access using Dapper micro-ORM with stored procedures.

### Data Access Components

**DapperContext:**
- Purpose: Manages database connection creation
- Configuration:
  - Reads connection string from `appsettings.json`
  - Key: `ConnectionStrings:DefaultConnection`
  - Value: `Server=localhost\SQLEXPRESS;Database=SensitiveWordsDb;Integrated Security=true;TrustServerCertificate=true`
- Method: `CreateConnection()` returns `IDbConnection` (SQL Server)
- Lifecycle: Registered as Singleton in DI container

**SensitiveWordRepository:**
- Implements: `ISensitiveWordRepository` from Domain layer
- Dependencies: `DapperContext`
- Data Access Strategy: All operations use stored procedures (no inline SQL)

### Stored Procedures

**sp_GetAllSensitiveWords:**
- Parameters: `@ActiveOnly BIT` (optional)
- Returns: All words or filtered by IsActive status
- Used by: GetAllAsync()

**sp_GetSensitiveWordById:**
- Parameters: `@Id UNIQUEIDENTIFIER`
- Returns: Single word or NULL
- Used by: GetByIdAsync()

**sp_CreateSensitiveWord:**
- Parameters: `@Word NVARCHAR(100)`
- Business Logic: Checks for duplicates, generates new Guid, sets timestamps
- Returns: New Guid
- Used by: CreateAsync()

**sp_UpdateSensitiveWord:**
- Parameters: `@Id UNIQUEIDENTIFIER`, `@Word NVARCHAR(100)`, `@IsActive BIT`
- Business Logic: Updates word and UpdatedAt timestamp
- Returns: Affected rows (success indicator)
- Used by: UpdateAsync()

**sp_DeleteSensitiveWord:**
- Parameters: `@Id UNIQUEIDENTIFIER`
- Business Logic: Soft delete (sets IsActive = 0)
- Returns: Affected rows (success indicator)
- Used by: DeleteAsync()

### Repository Methods

**GetByIdAsync(Guid id):**
- Executes: sp_GetSensitiveWordById
- Maps: Dynamic result to SensitiveWord entity
- Returns: Single entity or null

**GetAllAsync():**
- Executes: sp_GetAllSensitiveWords
- Handles: Optional activeOnly filtering
- Maps: Collection of dynamic results to List<SensitiveWord>
- Returns: IEnumerable<SensitiveWord>

**CreateAsync(string word):**
- Validation: Checks if word already exists
- Executes: sp_CreateSensitiveWord
- Transaction: Handled at database level
- Returns: New Guid

**UpdateAsync(Guid id, string word, bool isActive):**
- Executes: sp_UpdateSensitiveWord
- Updates: Both word text and active status
- Returns: bool (true if updated, false if not found)

**DeleteAsync(Guid id):**
- Executes: sp_DeleteSensitiveWord
- Note: Soft delete (record remains in database)
- Returns: bool (true if deleted, false if not found)

### Dapper Benefits
- **Performance**: Near-native ADO.NET speed (minimal overhead)
- **Simplicity**: Direct SQL execution with parameter mapping
- **No ORM Overhead**: No change tracking, lazy loading, or proxy generation
- **Explicit**: Stored procedures make database operations clear and auditable
- **Caching**: Database can cache stored procedure execution plans

### Infrastructure Design Principles
- **Dependency Inversion**: Infrastructure depends on Domain interfaces
- **Adapter Pattern**: Repository adapts Dapper to domain interfaces
- **Separation of Concerns**: Data access logic isolated from business logic
- **Testability**: Can be mocked/stubbed in unit tests

---

## 4. Overall System Architecture

### Component Overview

**Client Applications:**
- Web browsers (JavaScript/TypeScript consumers)
- Mobile applications (iOS/Android)
- Other microservices
- API testing tools (Postman, Swagger UI)
- All communicate via REST API over HTTPS

**SensitiveWords.API (Presentation Layer):**

*Controllers:*
- **SanitizeController**
  - Endpoint: `POST /api/sanitize`
  - Purpose: External-facing sanitization service
  - Delegates to: SanitizeMessageHandler

- **SensitiveWordsController**
  - Endpoints: CRUD operations on `/api/sensitivewords`
  - Purpose: Internal word management
  - Delegates to: Various command/query handlers

*Middleware:*
- **PerformanceMetricsMiddleware**
  - Tracks: Request count, duration, active sanitizations
  - Exposes: Metrics at `/metrics` endpoint

*Infrastructure:*
- **Health Checks**: Database connectivity monitoring at `/health`
- **Swagger/OpenAPI**: Interactive API documentation at `/swagger`
- **CORS**: Configured for cross-origin requests
- **Serilog**: Structured logging to console and file

**SensitiveWords.Application (Application Layer):**
- **Command Handlers**: Process Create/Update/Delete operations
- **Query Handlers**: Process read operations and sanitization
- **DTOs**: Data contracts for API communication
- **Validation**: Input validation using Data Annotations

**SensitiveWords.Domain (Domain Layer):**
- **Entities**: SensitiveWord aggregate root
- **Value Objects**: SanitizedMessage
- **Domain Services**: SanitizationService with business logic
- **Interfaces**: Repository and service contracts

**SensitiveWords.Infrastructure (Data Access Layer):**
- **Repositories**: SensitiveWordRepository implementation
- **Data Context**: DapperContext for connection management
- **ORM**: Dapper micro-ORM
- **Database**: SQL Server with stored procedures

**SQL Server Database:**
- **Table**: SensitiveWords
- **Stored Procedures**: 5 procedures for CRUD operations
- **Indexes**: Unique on Word, Non-unique on IsActive
- **Constraints**: Primary key on Id, Unique on Word

**External Services (Optional):**
- **Application Insights**: Telemetry and monitoring
- **Log Aggregation**: Centralized log collection (Serilog sinks)

### Communication Flow
1. Client makes HTTPS request to API
2. API controller receives request, validates model
3. Controller invokes appropriate handler (command or query)
4. Handler coordinates with domain services/repositories
5. Repository executes stored procedure via Dapper
6. Database processes request, returns results
7. Repository maps results to domain entities
8. Handler maps entities to DTOs
9. Controller returns HTTP response to client

### Architectural Patterns
- **Clean Architecture**: Dependency rule (dependencies point inward)
- **Layered Architecture**: Clear separation of concerns
- **CQRS**: Separate read and write paths
- **Repository Pattern**: Abstract data access
- **Dependency Injection**: IoC throughout application
- **Middleware Pattern**: Cross-cutting concerns (logging, metrics)

### Technology Stack Integration
- **ASP.NET Core 8.0**: Web framework and dependency injection
- **Dapper 2.1.35**: Data access and object mapping
- **SQL Server**: Persistence and business logic (stored procedures)
- **Serilog 8.0.3**: Structured logging
- **Swashbuckle**: API documentation generation
- **xUnit 2.9.2 + Moq 4.20.72**: Testing framework

---

## 5. Message Sanitization Flow

### Overview
This flow describes the end-to-end process when a client requests message sanitization, from HTTP request to response.

### Step-by-Step Process

**Step 1: Client Request**
- Client sends: `POST /api/sanitize`
- Headers: `Content-Type: application/json`
- Body: `{ "message": "SELECT * FROM users WHERE password = 'admin'" }`
- Protocol: HTTPS

**Step 2: API Controller Reception**
- SanitizeController receives request
- Model Binding: Deserializes JSON to SanitizeRequest DTO
- Validation: Checks ModelState (Required attribute on Message)
- If invalid: Returns 400 Bad Request with validation errors

**Step 3: Handler Invocation**
- Controller creates: `SanitizeMessageQuery` with message text
- Invokes: `SanitizeMessageHandler.HandleAsync(query)`
- Passes control to Application layer

**Step 4: Domain Service Invocation**
- Handler calls: `ISanitizationService.SanitizeMessageAsync(message)`
- Domain service (SanitizationService) executes core logic

**Step 5: Fetch Active Words**
- Service calls: `ISensitiveWordRepository.GetAllAsync()`
- Repository creates: Database connection via DapperContext
- Executes: `sp_GetAllSensitiveWords @ActiveOnly = 1`
- Database returns: Active sensitive words (SELECT, FROM, DROP, etc.)
- Repository maps: Dynamic results to `List<SensitiveWord>` entities

**Step 6: Sanitization Algorithm**
- Service receives: Collection of active SensitiveWord entities
- Sorts words: By length descending (e.g., "SELECT" before "SEL")
- Builds patterns: Regex with word boundaries `\b{word}\b`
- Loops through words:
  - Pattern: `\bSELECT\b` (case-insensitive)
  - Replacement: `******` (6 asterisks for 6-letter word)
  - Tracks: Which words were matched (DetectedWords list)
  - Counts: Total replacements made
- Result: "****** * **** users WHERE password = 'admin'"
- Detected: ["SELECT", "FROM"]
- Count: 2 replacements

**Step 7: Create Value Object**
- Service creates: `SanitizedMessage` value object
- Properties:
  - OriginalMessage: Original input
  - SanitizedText: Result with asterisks
  - DetectedWords: ["SELECT", "FROM"]
  - ReplacementCount: 2
- Returns: Immutable value object to handler

**Step 8: DTO Mapping**
- Handler receives: SanitizedMessage value object
- Maps to: SanitizeResponse DTO
- Copies all properties from value object to DTO
- Returns: DTO to controller

**Step 9: HTTP Response**
- Controller receives: SanitizeResponse DTO
- Status: 200 OK
- Serializes: DTO to JSON
- Response Body:
  ```json
  {
    "originalMessage": "SELECT * FROM users WHERE password = 'admin'",
    "sanitizedMessage": "****** * **** users WHERE password = 'admin'",
    "detectedWords": ["SELECT", "FROM"],
    "replacementCount": 2
  }
  ```

**Step 10: Client Processing**
- Client receives: JSON response
- Parses: Response data
- Displays: Before/after comparison to user
- Use case: Show sanitized version for logging or display purposes

### Performance Characteristics
- **Database Call**: Single query to fetch active words (cached)
- **Regex Processing**: O(n × m) where n = message length, m = number of words
- **Async Operations**: All I/O operations are asynchronous
- **Response Time**: Typically < 50ms for average message

### Error Scenarios

**500 Internal Server Error:**
- Cause: Database connection failure
- Logged: Exception details via Serilog
- Response: `{ "error": "Database connection failed" }`

**400 Bad Request:**
- Cause: Empty or missing message
- Validation: Data Annotations catch this
- Response: `{ "errors": { "Message": ["The Message field is required."] } }`

### Logging Points
- Request received (Information level)
- Words retrieved from database (Debug level)
- Sanitization performed (Information level)
- Response sent (Information level)
- Any exceptions (Error level)

---

## 6. Word Creation Flow

### Overview
This flow describes the process of creating a new sensitive word through the API, including validation, duplicate checking, and database persistence.

### Step-by-Step Process

**Step 1: Client Request**
- Client sends: `POST /api/sensitivewords`
- Headers: `Content-Type: application/json`
- Body: `{ "word": "DELETE" }`
- Authentication: (Optional - would be added in production)

**Step 2: Controller Validation**
- SensitiveWordsController receives request
- Model Binding: Deserializes to CreateSensitiveWordRequest
- Validation Checks:
  - Required: Word field must not be empty
  - StringLength: Between 1 and 100 characters
  - Format: (Data annotations validate)
- If invalid: Returns 400 Bad Request
- Error example: `{ "errors": { "Word": ["The Word field is required."] } }`

**Step 3: Create Command**
- Controller creates: `CreateSensitiveWordCommand("DELETE")`
- Command properties:
  - Word: "DELETE"
  - IsReadOnly: All properties immutable
- Controller invokes: `CreateSensitiveWordHandler.HandleAsync(command)`

**Step 4: Duplicate Check**
- Handler calls: `repository.GetByWordAsync("DELETE")`
- Repository executes: `sp_GetAllSensitiveWords`
- Queries database: Check if "DELETE" already exists
- Database returns: NULL (word doesn't exist) or entity (duplicate)

**Step 5a: Word Already Exists (Alternative Flow)**
- If word found: Handler throws `InvalidOperationException("Word already exists")`
- Controller catches: Exception in try-catch block
- Returns: 400 Bad Request
- Response: `{ "error": "Word 'DELETE' already exists" }`
- Flow ends

**Step 5b: Word Does Not Exist (Main Flow)**
- Repository returns: NULL
- Handler proceeds: To entity creation

**Step 6: Entity Creation**
- Handler creates: `new SensitiveWord("DELETE")`
- Entity initialization:
  - Id: `Guid.NewGuid()` (e.g., "3fa85f64-5717-4562-b3fc-2c963f66afa6")
  - Word: "DELETE" (converted to uppercase if needed)
  - IsActive: true (default)
  - CreatedAt: `DateTime.UtcNow`
  - UpdatedAt: `DateTime.UtcNow`
- Validation: Entity constructor validates word is not null/empty
- Entity returns: Valid SensitiveWord instance

**Step 7: Repository Persistence**
- Handler calls: `repository.CreateAsync("DELETE")`
- Repository creates: Database connection via DapperContext
- Executes: `sp_CreateSensitiveWord @Word = 'DELETE'`
- Stored Procedure Logic:
  1. Checks for duplicates (database constraint)
  2. Generates new GUID
  3. Inserts record with timestamps
  4. Returns new GUID
- Repository receives: New Guid from database
- Maps: Guid result to return value

**Step 8: Handler Response**
- Repository returns: Guid (e.g., "3fa85f64-5717-4562-b3fc-2c963f66afa6")
- Handler returns: Guid to controller
- No mapping needed: Guid is passed through

**Step 9: HTTP Response**
- Controller receives: Guid
- Creates: CreatedAtAction result
- Status: 201 Created
- Headers:
  - `Location: /api/sensitivewords/3fa85f64-5717-4562-b3fc-2c963f66afa6`
- Response Body: `{ "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6" }`

**Step 10: Client Processing**
- Client receives: 201 Created response
- Parses: New word ID
- Optional: Refresh word list via `GET /api/sensitivewords`
- Displays: Success message to user

### Business Rules Enforced

**At Application Layer:**
- Word must not be empty (data annotations)
- Word length: 1-100 characters
- Word must not already exist (checked by handler)

**At Domain Layer:**
- Word is immutable once created (value type property)
- Id is auto-generated and immutable
- Timestamps are auto-managed
- IsActive defaults to true

**At Database Layer:**
- Unique constraint on Word column
- Primary key constraint on Id
- Default values for IsActive, timestamps
- Stored procedure validates all inputs

### Error Scenarios

**400 Bad Request - Validation:**
- Empty word: `{ "errors": { "Word": ["The Word field is required."] } }`
- Word too long: `{ "errors": { "Word": ["Word must be between 1 and 100 characters"] } }`

**400 Bad Request - Duplicate:**
- Word exists: `{ "error": "Word 'DELETE' already exists" }`

**500 Internal Server Error:**
- Database failure: `{ "error": "Database connection failed" }`
- Constraint violation: `{ "error": "Database constraint violation" }`

### Transaction Management
- Database: Handled at stored procedure level
- Application: No distributed transactions (single database call)
- Rollback: Automatic if stored procedure fails

### Logging Points
- Request received with word (Information)
- Duplicate check performed (Debug)
- Entity created (Debug)
- Database insert executed (Information)
- Response sent with new ID (Information)
- Any errors (Error level)

---

## 7. Deployment Architecture

### Overview
The deployment architecture describes how the Flash.SensitiveWords microservice is deployed and configured across different servers and environments.

### Server Infrastructure

**Client Machine:**
- **Web Browsers**: Chrome, Firefox, Edge, Safari
- **Mobile Apps**: iOS and Android applications
- **API Consumers**: Other microservices, integration tools
- **Testing Tools**: Postman, Swagger UI, curl
- Connection: HTTPS to application server

**Application Server:**
- **Runtime**: .NET 8.0
- **Web Server**: Kestrel (built into ASP.NET Core)
- **Operating System**: Windows Server or Linux
- **Ports**:
  - 64725: HTTPS (TLS encrypted)
  - 64726: HTTP (development only, redirects to HTTPS)
- **Components**:
  - Flash.SensitiveWords.API.dll (main application)
  - Application Layer DLL
  - Domain Layer DLL
  - Infrastructure Layer DLL
  - Dependencies (Dapper, Serilog, etc.)
- **Logging**:
  - Console output (STDOUT)
  - File output: `./Logs/log-YYYYMMDD.txt` (rolling daily)
- **Configuration**:
  - appsettings.json
  - appsettings.Development.json (local)
  - appsettings.Production.json (production)
- **Health Checks**: `/health` endpoint for monitoring

**Database Server:**
- **DBMS**: SQL Server 2019+ or SQL Server Express
- **Database**: SensitiveWordsDb
- **Port**: 1433 (default SQL Server port)
- **Authentication**:
  - Local: Integrated Security (Windows Auth)
  - Production: SQL Authentication with strong password
- **Connection String**:
  ```
  Server=localhost\SQLEXPRESS;
  Database=SensitiveWordsDb;
  Integrated Security=true;
  TrustServerCertificate=true
  ```
- **Objects**:
  - 1 Table: SensitiveWords
  - 5 Stored Procedures: sp_GetAllSensitiveWords, sp_GetSensitiveWordById, sp_CreateSensitiveWord, sp_UpdateSensitiveWord, sp_DeleteSensitiveWord
  - 2 Indexes: Unique on Word, Non-unique on IsActive
- **Backup**: Regular automated backups (daily recommended)

### Network Communication

**Client to Application Server:**
- Protocol: HTTPS (TLS 1.2+)
- Port: 64725
- Content-Type: application/json
- Authentication: (Optional - JWT/OAuth would be added in production)

**Application Server to Database:**
- Protocol: SQL Server Protocol (TDS - Tabular Data Stream)
- Port: 1433
- Encryption: TrustServerCertificate=true
- Connection Pooling: Enabled (default)
- Connection Timeout: 30 seconds (default)

**Application Server to External Services:**
- Application Insights: HTTPS (optional telemetry)
- Log Aggregation: HTTPS or TCP (Serilog sinks)

### Deployment Options

**Option 1: Local Development**
- Application Server: Developer workstation
- Database Server: SQL Server Express on localhost
- Configuration: appsettings.Development.json
- HTTPS: Self-signed certificate
- Use Case: Development and testing

**Option 2: On-Premises Deployment**
- Application Server: Windows Server or Linux server
- Database Server: Dedicated SQL Server instance
- Load Balancer: IIS or Nginx (for multiple app instances)
- Firewall: Open ports 64725 (HTTPS), 1433 (SQL from app server only)
- Use Case: Enterprise internal deployment

**Option 3: Azure Cloud Deployment**
- Application Server: Azure App Service (Web App)
- Database Server: Azure SQL Database
- Monitoring: Application Insights (built-in)
- Scaling: Auto-scale based on CPU/memory
- Security: Azure AD authentication, Key Vault for secrets
- Use Case: Cloud-native SaaS deployment

**Option 4: AWS Cloud Deployment**
- Application Server: AWS Elastic Beanstalk or ECS
- Database Server: AWS RDS for SQL Server
- Monitoring: CloudWatch
- Scaling: Auto Scaling Groups
- Security: IAM roles, Secrets Manager
- Use Case: AWS-based infrastructure

### Configuration Management

**Development (appsettings.Development.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SensitiveWordsDb;Integrated Security=true;TrustServerCertificate=true"
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

**Production (appsettings.Production.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql-server;Database=SensitiveWordsDb;User Id=AppUser;Password={VAULT};Encrypt=true"
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

### Monitoring and Observability

**Health Checks:**
- Endpoint: `/health`
- Checks: Database connectivity, API responsiveness
- Format: JSON with status and details
- Polling: Every 30 seconds (recommended)

**Metrics:**
- Endpoint: `/metrics`
- Data: Request count, duration, active sanitizations
- Format: Plain text key-value pairs

**Logging:**
- Framework: Serilog
- Levels: Debug, Information, Warning, Error, Fatal
- Sinks: Console, File (./Logs/)
- Structured: JSON format for log aggregation
- Retention: 30 days (configurable)

**Application Insights (Optional):**
- Telemetry: Request/response times, exceptions, dependencies
- Alerts: Performance degradation, error rate spikes
- Dashboard: Real-time application health

### Security Considerations

**Transport Security:**
- HTTPS: Required for all production traffic
- TLS Version: 1.2 minimum, 1.3 preferred
- Certificate: Valid SSL certificate (not self-signed)

**Database Security:**
- Authentication: SQL Auth with strong password (20+ characters)
- Network: Restrict access to application server IP only
- Encryption: Enable Transparent Data Encryption (TDE)

**Application Security:**
- Secrets: Store in Azure Key Vault or AWS Secrets Manager
- Connection Strings: Never commit to source control
- Input Validation: Data annotations + stored procedure validation
- SQL Injection: Prevented by stored procedures and Dapper parameters

**Recommendations for Production:**
- Add JWT authentication/authorization
- Implement rate limiting
- Enable CORS with specific origins
- Add API key validation
- Implement request throttling
- Use Azure AD or OAuth identity provider

### Scaling Strategies

**Horizontal Scaling:**
- Multiple API instances behind load balancer
- Stateless application (no session state)
- Database connection pooling per instance

**Vertical Scaling:**
- Increase server CPU/RAM
- Increase database tier (more DTUs/vCores)

**Performance Optimization:**
- Enable response caching
- Add Redis cache for frequently accessed words
- Use Application Insights for bottleneck identification
- Optimize stored procedures with execution plans

---

## 8. System Use Cases

### Overview
This section describes the business functionality of the Flash.SensitiveWords microservice from the perspective of different users and system interactions.

### Actor Definitions

**1. End User (Public User):**
- Role: Anyone consuming the sanitization API
- Access Level: Public endpoints
- Primary Goal: Sanitize messages containing sensitive SQL keywords

**2. Administrator:**
- Role: System admin managing sensitive word database
- Access Level: CRUD endpoints (would require authentication in production)
- Primary Goal: Maintain and update sensitive word list

**3. System Monitor:**
- Role: DevOps engineer or monitoring system
- Access Level: Health check and metrics endpoints
- Primary Goal: Ensure system availability and performance

**4. SQL Server Database:**
- Role: Backend data store
- Type: System actor
- Purpose: Persist sensitive words and enforce data integrity

### Use Cases for End Users

**UC1: Sanitize Message**
- **Primary Actor**: End User
- **Preconditions**: API is running and healthy
- **Trigger**: User needs to sanitize text before logging or displaying
- **Main Flow**:
  1. User sends message to `/api/sanitize` endpoint
  2. System fetches active sensitive words from database
  3. System applies regex-based sanitization algorithm
  4. System replaces sensitive words with asterisks
  5. System returns original message, sanitized message, detected words, and count
- **Postconditions**: Message is sanitized, original preserved
- **Alternative Flows**:
  - No sensitive words found: Returns original message unchanged
  - Empty message: Returns 400 Bad Request
  - Database unavailable: Returns 500 Internal Server Error
- **Business Rules**:
  - Case-insensitive matching (SELECT, select, SeLeCt all match)
  - Word boundary matching (won't replace "SELECT" inside "SELECTION")
  - Replacement is same length as original word
  - Only active words are used for sanitization
- **Example**:
  - Input: "SELECT * FROM users"
  - Output: "****** * **** users" with detectedWords: ["SELECT", "FROM"]

**UC2: View Sanitization Result**
- **Primary Actor**: End User
- **Preconditions**: Sanitization has completed successfully
- **Trigger**: User wants to see before/after comparison
- **Main Flow**:
  1. System returns SanitizeResponse DTO
  2. User views originalMessage property
  3. User views sanitizedMessage property
  4. User views detectedWords list
  5. User views replacementCount
- **Postconditions**: User understands what was sanitized
- **Use Case Extension**: UC1 Sanitize Message

### Use Cases for Administrators

**UC3: View All Sensitive Words**
- **Primary Actor**: Administrator
- **Preconditions**: Database contains sensitive words
- **Trigger**: Admin needs to review current word list
- **Main Flow**:
  1. Admin calls `GET /api/sensitivewords`
  2. System retrieves all words from database
  3. System maps entities to DTOs
  4. System returns list of SensitiveWordDto objects
- **Postconditions**: Admin sees complete word list with metadata
- **Alternative Flows**:
  - Filter active only: `GET /api/sensitivewords?activeOnly=true`
  - Empty list: Returns empty array []
- **Includes**: UC4 Search Sensitive Words (filtering)

**UC4: Search Sensitive Words**
- **Primary Actor**: Administrator
- **Preconditions**: Word list exists
- **Trigger**: Admin wants to filter words by active status
- **Main Flow**:
  1. Admin provides activeOnly parameter
  2. System filters results based on IsActive flag
  3. System returns filtered list
- **Postconditions**: Admin sees filtered results
- **Extends**: UC3 View All Sensitive Words

**UC5: Create Sensitive Word**
- **Primary Actor**: Administrator
- **Preconditions**: Admin has valid word to add
- **Trigger**: New SQL keyword needs to be added to sanitization list
- **Main Flow**:
  1. Admin sends POST to `/api/sensitivewords` with word
  2. System validates input (not empty, length 1-100)
  3. System checks for duplicates
  4. System creates new SensitiveWord entity
  5. System persists to database via sp_CreateSensitiveWord
  6. System returns 201 Created with new Guid
- **Postconditions**: New word added to database, available for sanitization
- **Alternative Flows**:
  - Duplicate word: Returns 400 Bad Request "Word already exists"
  - Invalid input: Returns 400 Bad Request with validation errors
  - Database failure: Returns 500 Internal Server Error
- **Business Rules**:
  - Word must be unique (case-insensitive)
  - Word is stored in uppercase
  - IsActive defaults to true
  - CreatedAt and UpdatedAt timestamps are set automatically
- **Includes**: UC3 View All (refresh after creation)

**UC6: Update Sensitive Word**
- **Primary Actor**: Administrator
- **Preconditions**: Word exists in database
- **Trigger**: Admin needs to modify existing word or change active status
- **Main Flow**:
  1. Admin sends PUT to `/api/sensitivewords/{id}`
  2. System validates input
  3. System retrieves existing word by ID
  4. System updates word text and/or IsActive status
  5. System updates UpdatedAt timestamp
  6. System persists changes via sp_UpdateSensitiveWord
  7. System returns 204 No Content
- **Postconditions**: Word is updated in database
- **Alternative Flows**:
  - Word not found: Returns 404 Not Found
  - Invalid ID: Returns 400 Bad Request
  - Validation failure: Returns 400 Bad Request
- **Quick Toggle Extension**: UC7 Toggle Word Active Status

**UC7: Toggle Word Active Status**
- **Primary Actor**: Administrator
- **Preconditions**: Word exists
- **Trigger**: Admin wants to temporarily enable/disable a word
- **Main Flow**:
  1. Admin calls update with only IsActive changed
  2. System updates active status
  3. Word remains in database but won't be used if inactive
- **Postconditions**: Word active status changed
- **Extends**: UC6 Update Sensitive Word
- **Business Value**: Allows temporary disabling without deletion

**UC8: Delete Sensitive Word**
- **Primary Actor**: Administrator
- **Preconditions**: Word exists
- **Trigger**: Word is no longer needed in sanitization list
- **Main Flow**:
  1. Admin sends DELETE to `/api/sensitivewords/{id}`
  2. System validates ID
  3. System performs soft delete (sets IsActive = 0)
  4. System updates UpdatedAt timestamp
  5. System returns 204 No Content
- **Postconditions**: Word is inactive, not used in sanitization
- **Alternative Flows**:
  - Word not found: Returns 404 Not Found (or 400 based on implementation)
  - Invalid ID: Returns 400 Bad Request
- **Business Rule**: Soft delete preserves data for audit trail
- **Includes**: UC3 View All (refresh after deletion)

### Use Cases for System Monitors

**UC9: Check API Health**
- **Primary Actor**: System Monitor (DevOps or monitoring tool)
- **Preconditions**: API is deployed
- **Trigger**: Scheduled health check (every 30-60 seconds)
- **Main Flow**:
  1. Monitor calls `GET /health`
  2. System checks database connectivity
  3. System pings SQL Server
  4. System evaluates overall health status
  5. System returns JSON with health status
- **Postconditions**: Monitor knows if system is healthy
- **Response Format**:
  ```json
  {
    "status": "Healthy",
    "results": {
      "database": {
        "status": "Healthy",
        "description": "Database is accessible"
      }
    }
  }
  ```
- **Alternative Flows**:
  - Database unreachable: Returns "Unhealthy" status
  - API degraded: Returns "Degraded" status
- **Includes**: UC10 Check Database Readiness

**UC10: Check Database Readiness**
- **Primary Actor**: System Monitor
- **Preconditions**: Database server is deployed
- **Trigger**: Part of health check
- **Main Flow**:
  1. Health check invokes database ping
  2. System attempts connection to SQL Server
  3. System executes simple query
  4. Database responds with success
- **Postconditions**: Database status is known
- **Part Of**: UC9 Check API Health

**UC11: View System Metrics**
- **Primary Actor**: System Monitor
- **Preconditions**: API has processed requests
- **Trigger**: Monitor wants to see performance metrics
- **Main Flow**:
  1. Monitor calls `GET /metrics`
  2. System returns current metrics
  3. Monitor parses metrics data
- **Postconditions**: Monitor has performance data
- **Metrics Included**:
  - Total request count
  - Average request duration (ms)
  - Active sanitization operations
  - Peak requests per second
- **Use Case**: Performance monitoring and capacity planning

**UC12: View Performance Logs**
- **Primary Actor**: System Monitor
- **Preconditions**: Serilog is configured and writing logs
- **Trigger**: Investigating performance issue or error
- **Main Flow**:
  1. Monitor accesses log files in `./Logs/` directory
  2. Monitor parses structured JSON logs
  3. Monitor filters by timestamp, level, or message
  4. Monitor identifies issues or patterns
- **Postconditions**: Monitor understands system behavior
- **Log Aggregation**: Logs can be forwarded to centralized system

### System Interactions with Database

**UC13: Database Persists Sensitive Words**
- **Primary Actor**: SQL Server Database
- **Trigger**: Application creates, updates, or deletes words
- **Main Flow**:
  1. Repository calls stored procedure
  2. Database validates input parameters
  3. Database enforces constraints (unique, primary key)
  4. Database executes INSERT/UPDATE/DELETE
  5. Database updates timestamps
  6. Database returns result to application
- **Postconditions**: Data is persisted with integrity
- **Constraints Enforced**:
  - Primary key on Id
  - Unique index on Word
  - Default values for IsActive, timestamps
  - Data type validation (UNIQUEIDENTIFIER, NVARCHAR, BIT, DATETIME2)

**UC14: Database Provides Query Results**
- **Primary Actor**: SQL Server Database
- **Trigger**: Application queries for words
- **Main Flow**:
  1. Repository calls sp_GetAllSensitiveWords or sp_GetSensitiveWordById
  2. Database executes query with optional filtering
  3. Database uses indexes for performance (IX_SensitiveWords_Word, IX_SensitiveWords_IsActive)
  4. Database returns result set
  5. Application maps results to entities
- **Postconditions**: Application has requested data
- **Performance**: Indexes ensure fast lookup

### Use Case Relationships

**Include Relationships**:
- UC1 Sanitize Message **includes** UC3 View All (fetches active words)
- UC5 Create **includes** UC3 View All (refresh after creation)
- UC6 Update **includes** UC3 View All (refresh after update)
- UC8 Delete **includes** UC3 View All (refresh after deletion)
- UC9 Check Health **includes** UC10 Check Database Readiness

**Extend Relationships**:
- UC2 View Result **extends** UC1 Sanitize Message (on success)
- UC4 Search **extends** UC3 View All (filtering)
- UC7 Toggle Status **extends** UC6 Update (specific update type)

### Business Value

**For End Users:**
- Automatically sanitize sensitive SQL keywords from messages
- Prevent SQL injection patterns in logged data
- Safe display of user-generated content

**For Administrators:**
- Easy maintenance of sensitive word list
- No code deployment needed to update words
- Audit trail with timestamps

**For System Monitors:**
- Real-time health and performance visibility
- Proactive issue detection
- SLA compliance monitoring

**For Organization:**
- Security compliance (prevents sensitive data exposure)
- Reduced risk of SQL injection attacks
- Centralized sanitization logic (single source of truth)

---

## Architecture Highlights

### Clean Architecture Principles

**1. Dependency Rule**
- Dependencies point inward: API → Application → Domain
- Domain has no dependencies on outer layers
- Infrastructure implements domain interfaces

**2. Framework Independence**
- Domain layer is pure C#, no framework coupling
- Can swap out ASP.NET Core for another framework
- Can replace Dapper with Entity Framework or ADO.NET

**3. Testability**
- Each layer can be tested independently
- 64 tests total: 53 unit tests (mocked), 11 integration tests (real DB)
- 85.6% overall coverage (100% Application, 94.5% Domain)

**4. Separation of Concerns**
- Each layer has a single responsibility
- Clear boundaries between business logic and infrastructure

### Design Patterns Used

**1. CQRS (Command Query Responsibility Segregation)**
- Commands: CreateSensitiveWordCommand, UpdateSensitiveWordCommand, DeleteSensitiveWordCommand
- Queries: GetAllSensitiveWordsQuery, GetSensitiveWordByIdQuery, SanitizeMessageQuery
- Handlers: Dedicated handler for each command/query
- Benefits: Scalability, clarity of intent, separate optimization

**2. Repository Pattern**
- Interface: ISensitiveWordRepository in Domain layer
- Implementation: SensitiveWordRepository in Infrastructure layer
- Abstraction: Data access details hidden from business logic

**3. Domain-Driven Design (DDD)**
- Aggregate Root: SensitiveWord entity
- Value Objects: SanitizedMessage (immutable)
- Domain Services: SanitizationService
- Ubiquitous Language: Commands, queries, entities reflect business terminology

**4. Dependency Injection (DI)**
- ASP.NET Core built-in DI container
- Constructor injection throughout
- Inversion of Control (IoC) for loose coupling

**5. Mediator Pattern**
- Handlers mediate between controllers and domain
- Controllers don't directly call repositories or services
- Centralized request handling logic

**6. Value Object Pattern**
- SanitizedMessage is immutable
- No identity, equality by value
- Thread-safe and side-effect-free

### Technology Stack Summary

**Backend:**
- .NET 8.0, ASP.NET Core Web API, C# 12

**Data Access:**
- Dapper 2.1.35 (lightweight ORM)
- SQL Server 2019+ / SQL Server Express
- Stored Procedures for all CRUD operations

**Testing:**
- xUnit 2.9.2 (test framework)
- Moq 4.20.72 (mocking framework)
- Microsoft.AspNetCore.Mvc.Testing (integration testing)
- Coverlet.collector (code coverage)
- ReportGenerator (coverage reports)

**Monitoring:**
- Serilog 8.0.3 (structured logging)
- AspNetCore.HealthChecks.SqlServer (health monitoring)

**Documentation:**
- Swashbuckle.AspNetCore 7.2.0 (Swagger/OpenAPI)

**Coverage:**
- **Overall**: 85.6% (495 of 578 coverable lines)
- **Application**: 100% (all handlers, commands, queries, DTOs)
- **Domain**: 94.5% (entities, value objects, services)
- **Infrastructure**: 77.7% (Dapper repositories - requires real DB for full coverage)
- **API**: 80% (controllers and middleware)
- **Branch Coverage**: 71.6% (149 of 208 branches)
- **Method Coverage**: 97.4% (77 of 79 methods)

### Key Architectural Decisions

**1. Stored Procedures Over Inline SQL**
- Rationale: Better performance (query plan caching), enhanced security, centralized logic
- Trade-off: Less flexible than dynamic SQL, requires database deployment

**2. Dapper Over Entity Framework**
- Rationale: Lightweight, high-performance, explicit control
- Trade-off: No automatic change tracking, manual mapping

**3. Soft Delete Over Hard Delete**
- Rationale: Audit trail, data recovery, historical tracking
- Implementation: IsActive flag, delete sets to false

**4. Immutable Commands and Queries**
- Rationale: Thread-safety, clarity of intent, prevents accidental modification
- Implementation: Readonly properties, initialized in constructor

**5. DTO Separation**
- Rationale: Decouple API contracts from domain entities, versioning flexibility
- Trade-off: Requires mapping code, duplication of properties

**6. Async/Await Throughout**
- Rationale: Scalability, better thread utilization, non-blocking I/O
- Implementation: All repository and handler methods are asynchronous

---

**Last Updated**: 2025-11-13
**Project**: Flash.SensitiveWords Microservice
**Author**: Auratius February (auratius@gmail.com)
**Test Coverage**: 85.6% (64 tests total)
**Architecture**: Clean Architecture with DDD and CQRS
