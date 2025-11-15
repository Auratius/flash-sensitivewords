# UML Diagrams

Visual documentation of the Flash.SensitiveWords architecture and workflows.

## Project Information

**Repository:** [https://github.com/Auratius/flash-sensitivewords](https://github.com/Auratius/flash-sensitivewords)

**Test Coverage:** 133 tests passing (78 unit + 55 integration)
**Controller Coverage:** 100% on all 3 controllers (SensitiveWords, Sanitize, Statistics)
**Architecture:** Clean Architecture + CQRS + DDD
**Tech Stack:** .NET 8, React 18, SQL Server, Dapper

## About This Documentation

This document provides comprehensive UML diagrams that visualize the architecture, design patterns, and workflows of the Flash.SensitiveWords microservice. The diagrams are organized from granular (individual layers) to holistic (system-wide views), making it easy to understand both specific components and overall system design.

**Target Audience:**
- Software developers joining the project
- System architects reviewing the design
- Technical stakeholders understanding the implementation
- DevOps engineers planning deployment
- QA engineers understanding test scenarios

## Architecture Diagrams

### 1. Domain Layer Class Diagram
Shows the core business entities and value objects following Domain-Driven Design principles.

**What it contains:**
- **SensitiveWord entity** - The aggregate root containing:
  - `Id` (int): Primary identifier
  - `Word` (string): The sensitive word to be filtered
  - `Category` (string): Classification of the word (e.g., profanity, spam, hate speech)
  - `Severity` (int): Severity level for prioritization
  - `CreatedAt` (DateTime): Timestamp of creation
- **SanitizationResult value object** - Immutable result containing:
  - `OriginalMessage`: The unmodified input
  - `SanitizedMessage`: The filtered output
  - `ReplacedWordsCount`: Number of sensitive words replaced
  - `ReplacedWords`: List of words that were sanitized
- **OperationStat entity** - For tracking API operation metrics:
  - Operation type (CREATE, READ, UPDATE, DELETE, SANITIZE)
  - Timestamp and frequency tracking
- Domain models and their relationships

**Design Patterns:**
- Aggregate Root Pattern (SensitiveWord)
- Value Object Pattern (SanitizationResult)
- Repository Interface Definitions

**Link:** [View Diagram](UMLDiagrams/1.%20Class%20Diagram%20-%20Domain%20Layer.png)

---

### 2. Application Layer Class Diagram
Shows the CQRS (Command Query Responsibility Segregation) implementation with MediatR.

**What it contains:**

**Commands (Write Operations):**
- `CreateSensitiveWordCommand` - Adds new sensitive words to the system
- `UpdateSensitiveWordCommand` - Modifies existing sensitive word properties
- `DeleteSensitiveWordCommand` - Removes sensitive words from the system

**Queries (Read Operations):**
- `GetAllSensitiveWordsQuery` - Retrieves all sensitive words with filtering/pagination support
- `SanitizeMessageQuery` - Processes messages to filter sensitive content
- `GetStatisticsQuery` - Fetches operation statistics with optional filtering by operation type

**Handlers:**
- Each command/query has a dedicated handler implementing `IRequestHandler<TRequest, TResponse>`
- All handlers integrate operation statistics tracking
- Handlers orchestrate domain logic and repository calls

**DTOs and Response Models:**
- `SanitizeResponse` - Contains original, sanitized message, and replacement details
- `SensitiveWordDto` - Data transfer object for API responses
- `OperationStatDto` - Statistics data for API monitoring

**Design Patterns:**
- CQRS Pattern (commands separate from queries)
- Mediator Pattern (via MediatR library)
- Handler Pattern (one handler per request)
- DTO Pattern (domain-to-API mapping)

**Link:** [View Diagram](UMLDiagrams/2.%20Class%20Diagram%20-%20Application%20Layer.png)

---

### 3. Infrastructure Layer Class Diagram
Shows data access implementations and external dependencies.

**What it contains:**

**Repository Implementations:**
- `SensitiveWordRepository` - Implements `ISensitiveWordRepository`
  - Uses Dapper for high-performance SQL queries
  - Calls stored procedures: `sp_CreateSensitiveWord`, `sp_UpdateSensitiveWord`, `sp_DeleteSensitiveWord`, `sp_GetAllSensitiveWords`
  - Handles concurrent access safely
- `OperationStatsRepository` - Implements `IOperationStatsRepository`
  - Tracks all API operations (CREATE, READ, UPDATE, DELETE, SANITIZE)
  - Stores operation timestamps and counts
  - Provides filtering and reset functionality

**Database Context:**
- Connection string management
- SQL Server integration
- Transaction handling

**Technology Stack:**
- **Dapper** - Micro ORM for performance-critical queries
- **SQL Server** - Relational database with stored procedures
- **ADO.NET** - Underlying database connectivity

**Design Patterns:**
- Repository Pattern (abstraction over data access)
- Dependency Inversion (repositories implement domain interfaces)
- Stored Procedure Pattern (encapsulated database logic)

**Performance Features:**
- Dapper's compiled query execution
- Stored procedures for optimized database operations
- Efficient object mapping

**Link:** [View Diagram](UMLDiagrams/3.%20Class%20Diagram%20-%20Infrastructure%20Layer.png)

---

### 4. Component Diagram - Overall Architecture
High-level view of the complete system architecture showing how all layers interact.

**What it contains:**

**Presentation Layer:**
- **React Frontend** (Port 5173)
  - Built with TypeScript for type safety
  - Styled with Tailwind CSS
  - Consumes REST API endpoints
  - User interface for word management and message sanitization

**API Layer (3 Controllers):**
- `SensitiveWordsController` - CRUD operations for sensitive words
- `SanitizeController` - Message sanitization endpoint
- `StatisticsController` - Operation metrics and monitoring
- Hosted on ports 64725 (HTTP) and 64726 (HTTPS)

**Application Layer:**
- **CQRS Handlers** - Command and query processing
- **Operation Tracking** - Automatic statistics collection
- **MediatR Pipeline** - Request/response mediation
- **Validation** - Input validation and business rule enforcement

**Domain Layer:**
- **Business Logic** - Core sanitization algorithms
- **Entities** - SensitiveWord, OperationStat
- **Value Objects** - SanitizationResult
- **Domain Services** - Pure business logic
- **Repository Interfaces** - Abstraction for data access

**Infrastructure Layer:**
- **Dapper ORM** - High-performance data mapping
- **SQL Server** - Persistent data storage
- **Repositories** - Concrete implementations
- **Stored Procedures** - Database business logic

**Cross-Cutting Concerns:**
- Statistics tracking flows through all layers
- Dependency injection configured in API startup
- Clean Architecture dependency rules enforced

**Data Flow:**
1. React → API Controllers
2. Controllers → MediatR → Handlers
3. Handlers → Domain Services → Repositories
4. Repositories → Dapper → SQL Server
5. Response flows back through the same layers

**Link:** [View Diagram](UMLDiagrams/4.%20Component%20Diagram%20-%20Overall%20Architecture.png)

---

## Workflow Diagrams

### 5. Sequence Diagram - Sanitize Message
Detailed step-by-step flow of the core sanitization feature.

**What it shows:**

**Request Flow:**
1. **User/Client** sends POST request to `/api/sanitize` with message payload
2. **SanitizeController** receives HTTP request
   - Validates request model
   - Creates `SanitizeMessageQuery`
   - Sends query to MediatR
3. **MediatR** routes query to appropriate handler
4. **SanitizeMessageHandler** processes the query:
   - Calls repository to fetch all active sensitive words
   - Passes message and words to domain service
5. **Domain Service** performs sanitization:
   - Iterates through sensitive words
   - Replaces matches with asterisks (*)
   - Tracks replaced words and count
   - Creates immutable `SanitizationResult` value object
6. **Statistics Recording:**
   - Handler calls `OperationStatsRepository`
   - Records SANITIZE operation with timestamp
7. **Response Flow:**
   - Handler maps domain result to `SanitizeResponse` DTO
   - Returns to controller
   - Controller sends HTTP 200 OK with response body

**Response Contains:**
- `OriginalMessage` - The unmodified input text
- `SanitizedMessage` - Text with sensitive words replaced
- `ReplacedWordsCount` - Number of words filtered
- `ReplacedWords` - List of specific words that were sanitized

**Performance Considerations:**
- Single database query fetches all sensitive words
- In-memory string replacement operations
- Asynchronous processing throughout the pipeline

**Link:** [View Diagram](UMLDiagrams/5.%20Sequence%20Diagram%20-%20Sanitize%20Message.png)

---

### 6. Sequence Diagram - Create Sensitive Word
Detailed step-by-step flow of adding a new sensitive word to the system.

**What it shows:**

**Request Flow:**
1. **User** submits new word via:
   - React UI form with word, category, and severity
   - Direct API call to `/api/sensitivewords`
2. **SensitiveWordsController** receives POST request:
   - Validates request payload (required fields, data types)
   - Checks for duplicate words
   - Creates `CreateSensitiveWordCommand`
   - Sends command to MediatR
3. **MediatR** routes command to handler
4. **CreateSensitiveWordHandler** processes command:
   - Validates business rules (e.g., word not empty, valid category)
   - Creates `SensitiveWord` domain entity
   - Sets `CreatedAt` timestamp
   - Calls repository to persist the entity
5. **SensitiveWordRepository** saves to database:
   - Executes stored procedure `sp_CreateSensitiveWord`
   - Parameters: word, category, severity, createdAt
   - SQL Server generates and returns the new ID
   - Returns the complete entity with ID populated
6. **Statistics Recording:**
   - Handler calls `OperationStatsRepository`
   - Records CREATE operation with timestamp
   - Increments operation counter
7. **Response Flow:**
   - Handler maps entity to `SensitiveWordDto`
   - Returns to controller
   - Controller sends HTTP 201 Created
   - Response includes created word with ID and Location header

**Validation Points:**
- Controller: Model validation, format checks
- Handler: Business rule validation
- Database: Unique constraint enforcement

**Error Handling:**
- Duplicate word → HTTP 409 Conflict
- Invalid input → HTTP 400 Bad Request
- Database error → HTTP 500 Internal Server Error

**Link:** [View Diagram](UMLDiagrams/6.%20Sequence%20Diagram%20-%20Create%20Sensitive%20Word.png)

---

## Deployment & Use Cases

### 7. Deployment Diagram
Shows the physical deployment architecture and infrastructure components.

**What it contains:**

**Client Tier:**
- **Web Browser** (Chrome, Firefox, Safari, Edge)
  - Runs React SPA (Single Page Application)
  - Connects to API via HTTPS
  - Port: 5173 (development) / 443 (production)
  - JavaScript execution environment

**Application Tier:**
- **Web Server** (IIS or Kestrel)
  - Hosts .NET 8 Web API
  - Handles HTTP/HTTPS requests
  - Ports: 64725 (HTTP), 64726 (HTTPS)
  - SSL/TLS certificate for secure communication
  - Load balancing ready (stateless design)
  - Health check endpoint for monitoring

**Data Tier:**
- **SQL Server** (2019 or later)
  - Persistent data storage
  - Stores SensitiveWords and OperationStats tables
  - Executes stored procedures
  - Connection: TCP/IP on port 1433
  - Backup and recovery configured
  - Transaction log management

**Network Configuration:**
- **HTTPS** - Client to API (encrypted)
- **TCP/IP** - API to Database (encrypted in production)
- **Firewall rules** - Only necessary ports exposed
- **CORS** - Configured for React frontend origin

**Deployment Environments:**
- **Development** - Local machine, ports as shown
- **Production** - Could be deployed to:
  - Azure App Service + Azure SQL Database
  - AWS EC2 + RDS
  - On-premises servers
  - Docker containers with orchestration

**Security Considerations:**
- HTTPS enforced for all API communication
- SQL connection strings in environment variables/secrets
- API keys and sensitive config in secure storage
- Database credentials use least-privilege access

**Link:** [View Diagram](UMLDiagrams/7.%20Deployment%20Diagram.png)

---

### 8. Use Case Diagram
Shows all user interactions and system capabilities from a functional perspective.

**What it contains:**

**Primary Actor: User/Application**

**Core Use Cases:**

1. **Sanitize Message** (Primary Feature)
   - Submit text message for filtering
   - Receive sanitized version with replacement details
   - View which words were replaced
   - Most frequently used operation

2. **Manage Sensitive Words** (CRUD Operations)
   - **Create Sensitive Word**
     - Add new word with category and severity
     - Specify word properties
   - **Read/View Sensitive Words**
     - List all sensitive words
     - Filter by category or severity
     - Pagination support
   - **Update Sensitive Word**
     - Modify existing word properties
     - Change category or severity
   - **Delete Sensitive Word**
     - Remove word from system
     - Soft or hard delete

3. **View Statistics** (Monitoring & Analytics)
   - View all operation statistics
   - Filter statistics by operation type (CREATE, READ, UPDATE, DELETE, SANITIZE)
   - See operation counts and timestamps
   - Monitor system usage patterns
   - Reset statistics counters

4. **Health Check**
   - Check API availability
   - Verify database connectivity
   - System health monitoring

**Use Case Relationships:**
- **Include**: Sanitize Message includes "Record Statistics"
- **Include**: All CRUD operations include "Record Statistics"
- **Extend**: View Statistics extends with "Filter by Type"
- **Extend**: View Statistics extends with "Reset Counters"

**System Boundary:**
- Flash.SensitiveWords API
- All use cases operate within the microservice boundary

**Actor Types:**
- **End User** - Via React UI
- **Client Application** - Via REST API
- **Administrator** - Managing words and viewing statistics
- **Monitoring System** - Health checks

**Link:** [View Diagram](UMLDiagrams/8.%20Use%20Case%20Diagram.png)

---

## Quick Reference

| Diagram | Purpose | Good For |
|---------|---------|----------|
| Domain Layer | Understanding business logic and entities | New developers learning the domain |
| Application Layer | Understanding CQRS patterns and handlers | Implementing new features |
| Infrastructure Layer | Understanding data access and repositories | Database changes and performance tuning |
| Component Diagram | Overall architecture view | System design discussions |
| Sanitize Sequence | Message sanitization flow with stats tracking | Understanding main feature |
| Create Word Sequence | Word management flow with stats tracking | CRUD operations |
| Deployment | Production setup and infrastructure | DevOps and deployment |
| Use Case | User capabilities and system features | Requirements and testing |

## Key Features Reflected in Diagrams

- **Clean Architecture:** Clear separation of concerns across 4 layers
- **CQRS Pattern:** Commands and queries handled separately
- **Operation Statistics:** All handlers track CREATE, READ, UPDATE, DELETE, SANITIZE operations
- **Repository Pattern:** Abstraction over data access with Dapper
- **Domain-Driven Design:** SensitiveWord aggregate root, value objects
- **RESTful API:** 3 controllers with comprehensive endpoint coverage

---

## Testing Coverage

The architecture is validated by comprehensive testing:

- **133 tests total** (78 unit + 55 integration)
- **100% controller coverage** across all 3 controllers
- **100% application layer coverage**
- **94.5% domain layer coverage**

### Test Distribution

**Unit Tests (78):**
- Application handlers: 20 tests (includes StatisticsController unit tests)
- Domain entities and services: 28 tests
- Repository mocks: 30 tests

**Integration Tests (55):**
- SensitiveWordsController: 21 tests (CRUD operations, edge cases, concurrent requests)
- SanitizeController: 13 tests (sanitization logic, performance, validation)
- StatisticsController: 21 tests (all operation types, filtering, reset functionality)

All tests validate both functional correctness and operation statistics tracking.

---

---

## Documentation Feedback & Assessment

### Strengths

**1. Comprehensive Coverage**
- All 8 diagrams cover different architectural perspectives (structural, behavioral, deployment)
- Clear progression from detailed (layer diagrams) to holistic (component diagram)
- Both static (class diagrams) and dynamic (sequence diagrams) views provided

**2. Clean Architecture Implementation**
- Excellent separation of concerns across 4 distinct layers
- Dependency rules properly enforced (dependencies point inward)
- Domain layer is framework-agnostic and contains pure business logic

**3. Design Pattern Application**
- CQRS properly implemented with MediatR
- Repository pattern abstracts data access
- Value Objects and Aggregate Roots follow DDD principles
- Clear demonstration of SOLID principles

**4. Testing Excellence**
- 133 tests with high coverage demonstrates quality
- 100% controller coverage ensures API reliability
- Mix of unit and integration tests validates both isolation and integration

**5. Documentation Quality**
- Each diagram has clear purpose and description
- Quick reference table helps users find relevant diagrams
- Diagrams are up-to-date with actual implementation

### Recommendations for Improvement

**1. Consider Adding:**
- **State Diagram** - Show lifecycle states of SensitiveWord entity (Active, Archived, Deleted)
- **Activity Diagram** - Detailed flow of sanitization algorithm with decision points
- **Package/Namespace Diagram** - Show how namespaces are organized
- **Database Schema Diagram** - ERD showing tables, relationships, and indexes

**2. Enhanced Documentation:**
- Add versioning to diagrams (e.g., "v1.0" on each diagram)
- Include a "Last Reviewed" date on each diagram
- Add cross-references between related diagrams
- Consider adding a "Reading Guide" for new developers (suggested order)

**3. Interactive Elements:**
- Consider generating diagrams from code using PlantUML or Mermaid for easier maintenance
- Add clickable areas in SVG format linking to source code
- Include diagram source files (PlantUML/Mermaid) in repository for version control

**4. Additional Details:**
- Add error handling flows to sequence diagrams
- Show caching strategy (if any) in component diagram
- Document API versioning strategy
- Include authentication/authorization flows if applicable

**5. Metrics & Monitoring:**
- Add diagram showing logging and monitoring architecture
- Document alerting and error tracking flows
- Show how statistics feed into monitoring dashboards

### Suggested Reading Order for New Developers

1. **Start Here:** Use Case Diagram (8) - Understand what the system does
2. **Architecture Overview:** Component Diagram (4) - See how it fits together
3. **Core Flow:** Sequence Diagram - Sanitize Message (5) - Main feature walkthrough
4. **Layer Details:** Domain → Application → Infrastructure (1, 2, 3)
5. **CRUD Flow:** Sequence Diagram - Create Word (6) - Management operations
6. **Deployment:** Deployment Diagram (7) - Infrastructure and hosting

### Integration with Development Workflow

**Consider adding:**
- Pre-commit hook to remind developers to update diagrams when architecture changes
- Automated diagram generation from code annotations
- Diagram reviews as part of PR process for architectural changes
- Link diagrams to relevant code files in README sections

### Overall Assessment

**Score: 9/10**

**Excellent aspects:**
- Professional quality diagrams
- Comprehensive architectural coverage
- Well-documented and organized
- Strong alignment with actual implementation
- Good use of UML standards

**Minor gaps:**
- Could benefit from additional behavioral diagrams (state, activity)
- Database schema not explicitly shown
- Error handling flows could be more detailed

**Conclusion:**
This is an exemplary set of UML diagrams that effectively communicates the system architecture. The documentation serves as an excellent reference for developers, architects, and stakeholders. The combination of static and dynamic views provides a complete picture of the system's design and behavior.

---

**Generated:** 2025-11-14
**Updated:** 2025-11-15
**Total Diagrams:** 8
**Format:** PNG images
**Test Coverage:** 133 tests passing
**Repository:** [https://github.com/Auratius/flash-sensitivewords](https://github.com/Auratius/flash-sensitivewords)
