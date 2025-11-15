# UML Diagrams

Visual documentation of the Flash.SensitiveWords architecture and workflows.

## Project Overview

**Test Coverage:** 133 tests passing (78 unit + 55 integration)
**Controller Coverage:** 100% on all 3 controllers (SensitiveWords, Sanitize, Statistics)
**Architecture:** Clean Architecture + CQRS + DDD
**Tech Stack:** .NET 8, React 18, SQL Server, Dapper

## Architecture Diagrams

### 1. Domain Layer Class Diagram
Shows the core business entities and value objects.

**What it contains:**
- SensitiveWord entity (aggregate root)
- SanitizationResult value object
- OperationStat entity (for tracking API operations)
- Domain models and their relationships

**Link:** [View Diagram](UMLDiagrams/1.%20Class%20Diagram%20-%20Domain%20Layer.png)

---

### 2. Application Layer Class Diagram
Shows the CQRS commands, queries, and handlers.

**What it contains:**
- Commands (CreateSensitiveWordCommand, DeleteSensitiveWordCommand, UpdateSensitiveWordCommand)
- Queries (GetAllSensitiveWordsQuery, SanitizeMessageQuery, GetStatisticsQuery)
- Handlers for each command and query
- DTOs and response models (SanitizeResponse, SensitiveWordDto, OperationStatDto)
- Operation statistics tracking in all handlers

**Link:** [View Diagram](UMLDiagrams/2.%20Class%20Diagram%20-%20Application%20Layer.png)

---

### 3. Infrastructure Layer Class Diagram
Shows data access and external dependencies.

**What it contains:**
- Repository implementations (SensitiveWordRepository, OperationStatsRepository)
- Database context
- Dapper integration for high-performance data access
- SQL Server stored procedure calls (CRUD operations + statistics tracking)

**Link:** [View Diagram](UMLDiagrams/3.%20Class%20Diagram%20-%20Infrastructure%20Layer.png)

---

### 4. Component Diagram - Overall Architecture
High-level view of how all the layers fit together.

**What it contains:**
- API Layer (3 Controllers: SensitiveWords, Sanitize, Statistics)
- Application Layer (CQRS handlers with operation tracking)
- Domain Layer (Business Logic and entities)
- Infrastructure Layer (Dapper + SQL Server)
- React Frontend (TypeScript + Tailwind CSS)
- Component interactions and dependencies
- Statistics tracking flow across all layers

**Link:** [View Diagram](UMLDiagrams/4.%20Component%20Diagram%20-%20Overall%20Architecture.png)

---

## Workflow Diagrams

### 5. Sequence Diagram - Sanitize Message
Step-by-step flow of sanitizing a message.

**What it shows:**
1. User sends message to API
2. SanitizeController receives request
3. SanitizeMessageHandler processes sanitization logic
4. Repository fetches sensitive words from database
5. Message is sanitized using domain service
6. Operation statistics are recorded (SANITIZE operation)
7. Result returned to user with original message, sanitized message, and words replaced count

**Link:** [View Diagram](UMLDiagrams/5.%20Sequence%20Diagram%20-%20Sanitize%20Message.png)

---

### 6. Sequence Diagram - Create Sensitive Word
Step-by-step flow of adding a new sensitive word.

**What it shows:**
1. User submits new word via React UI or API
2. SensitiveWordsController validates request
3. CreateSensitiveWordHandler processes command
4. Repository saves to database via stored procedure
5. Operation statistics are recorded (CREATE operation)
6. Success response returned with created word details

**Link:** [View Diagram](UMLDiagrams/6.%20Sequence%20Diagram%20-%20Create%20Sensitive%20Word.png)

---

## Deployment & Use Cases

### 7. Deployment Diagram
Shows how the system is deployed and runs.

**What it contains:**
- Web Server (IIS/Kestrel) hosting the API
- Database Server (SQL Server)
- Client Browser running React app
- Network connections and protocols (HTTPS, TCP)
- Port configurations (64725, 64726, 5173)

**Link:** [View Diagram](UMLDiagrams/7.%20Deployment%20Diagram.png)

---

### 8. Use Case Diagram
Shows all the things users can do with the system.

**What it contains:**
- User actions (sanitize messages, manage words, view health, view statistics)
- CRUD operations (Create, Read, Update, Delete sensitive words)
- Statistics tracking (view all operations, filter by type, reset counters)
- System boundaries
- Actor interactions
- Use case relationships

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

**Generated:** 2025-11-14
**Updated:** 2025-11-15
**Total Diagrams:** 8
**Format:** PNG images
**Test Coverage:** 133 tests passing
