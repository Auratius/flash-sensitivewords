# UML Diagrams

Visual documentation of the Flash.SensitiveWords architecture and workflows.

## Architecture Diagrams

### 1. Domain Layer Class Diagram
Shows the core business entities and value objects.

**What it contains:**
- SensitiveWord entity (aggregate root)
- SanitizationResult value object
- Domain models and their relationships

**Link:** [View Diagram](UMLDiagrams/1.%20Class%20Diagram%20-%20Domain%20Layer.png)

---

### 2. Application Layer Class Diagram
Shows the CQRS commands, queries, and handlers.

**What it contains:**
- Commands (CreateSensitiveWordCommand, DeleteSensitiveWordCommand, etc.)
- Queries (GetAllSensitiveWordsQuery, SanitizeMessageQuery)
- Handlers for each command and query
- DTOs and response models

**Link:** [View Diagram](UMLDiagrams/2.%20Class%20Diagram%20-%20Application%20Layer.png)

---

### 3. Infrastructure Layer Class Diagram
Shows data access and external dependencies.

**What it contains:**
- Repository implementations
- Database context
- Dapper integration
- SQL Server stored procedure calls

**Link:** [View Diagram](UMLDiagrams/3.%20Class%20Diagram%20-%20Infrastructure%20Layer.png)

---

### 4. Component Diagram - Overall Architecture
High-level view of how all the layers fit together.

**What it contains:**
- API Layer (Controllers, Middleware)
- Application Layer (CQRS)
- Domain Layer (Business Logic)
- Infrastructure Layer (Database)
- React Frontend
- Component interactions and dependencies

**Link:** [View Diagram](UMLDiagrams/4.%20Component%20Diagram%20-%20Overall%20Architecture.png)

---

## Workflow Diagrams

### 5. Sequence Diagram - Sanitize Message
Step-by-step flow of sanitizing a message.

**What it shows:**
1. User sends message to API
2. Controller receives request
3. Handler processes sanitization logic
4. Repository fetches sensitive words
5. Message is sanitized
6. Result returned to user

**Link:** [View Diagram](UMLDiagrams/5.%20Sequence%20Diagram%20-%20Sanitize%20Message.png)

---

### 6. Sequence Diagram - Create Sensitive Word
Step-by-step flow of adding a new sensitive word.

**What it shows:**
1. User submits new word
2. Controller validates request
3. Command handler processes creation
4. Repository saves to database
5. Success response returned

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
- User actions (sanitize messages, manage words, view health)
- System boundaries
- Actor interactions
- Use case relationships

**Link:** [View Diagram](UMLDiagrams/8.%20Use%20Case%20Diagram.png)

---

## Quick Reference

| Diagram | Purpose | Good For |
|---------|---------|----------|
| Domain Layer | Understanding business logic | New developers learning the domain |
| Application Layer | Understanding CQRS patterns | Implementing new features |
| Infrastructure Layer | Understanding data access | Database changes |
| Component Diagram | Overall architecture view | System design discussions |
| Sanitize Sequence | Message sanitization flow | Understanding main feature |
| Create Word Sequence | Word management flow | CRUD operations |
| Deployment | Production setup | DevOps and deployment |
| Use Case | User capabilities | Requirements and testing |

---

**Generated:** 2025-11-14
**Total Diagrams:** 8
**Format:** PNG images
