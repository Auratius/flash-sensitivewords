# Flash.SensitiveWords - Jira Stories

Complete list of user stories, technical tasks, and epics for the project.

---

## Epic 1: Project Foundation & Architecture

### Story 1.1: Project Setup and Architecture Design
**Type:** Technical Task
**Priority:** Highest
**Story Points:** 8

**Description:**
Set up the initial project structure using Clean Architecture with CQRS pattern for the Flash.SensitiveWords microservice.

**Acceptance Criteria:**
- [ ] Solution structure created with 4 layers (API, Application, Domain, Infrastructure)
- [ ] .NET 8 SDK configured
- [ ] Project references properly set up
- [ ] Git repository initialized
- [ ] Architecture documentation created

**Technical Notes:**
- Use Clean Architecture principles
- Implement CQRS pattern for separation of commands and queries
- Follow Domain-Driven Design (DDD) patterns

---

### Story 1.2: Database Design and Setup
**Type:** Technical Task
**Priority:** Highest
**Story Points:** 5

**Description:**
Design and implement the database schema for storing sensitive SQL keywords.

**Acceptance Criteria:**
- [ ] Database schema designed
- [ ] SensitiveWords table created with proper fields (Id, Word, IsActive, CreatedAt, UpdatedAt)
- [ ] Indexes created for performance
- [ ] SQL migration scripts created (01-04)
- [ ] Seed data script with common SQL keywords

**Technical Notes:**
- Use SQL Server
- Create stored procedures for all CRUD operations
- Add sample data (SELECT, DELETE, DROP, INSERT, UPDATE, etc.)

---

## Epic 2: Core API Development

### Story 2.1: Domain Layer - Sensitive Word Entity
**Type:** Story
**Priority:** Highest
**Story Points:** 3

**Description:**
As a developer, I need a domain entity to represent a sensitive word so that business rules are enforced at the domain level.

**Acceptance Criteria:**
- [ ] SensitiveWord entity created with all properties
- [ ] Business rules implemented (word cannot be empty, auto-uppercase)
- [ ] Value objects created for Word
- [ ] Domain events defined
- [ ] Unit tests for all business rules

**Technical Notes:**
- Make entity an aggregate root
- Ensure immutability where appropriate
- Validate all business rules

---

### Story 2.2: Application Layer - CQRS Commands and Queries
**Type:** Story
**Priority:** Highest
**Story Points:** 8

**Description:**
As a developer, I need CQRS handlers for all operations so that read and write operations are properly separated.

**Acceptance Criteria:**
- [ ] Commands created (Create, Update, Delete)
- [ ] Queries created (GetAll, GetById, Sanitize)
- [ ] Command handlers implemented
- [ ] Query handlers implemented
- [ ] DTOs created for all operations
- [ ] Unit tests for all handlers (100% coverage)

**Commands:**
- CreateSensitiveWordCommand
- UpdateSensitiveWordCommand
- DeleteSensitiveWordCommand

**Queries:**
- GetAllSensitiveWordsQuery
- GetSensitiveWordByIdQuery
- SanitizeMessageQuery

---

### Story 2.3: Infrastructure Layer - Dapper Repository
**Type:** Technical Task
**Priority:** High
**Story Points:** 5

**Description:**
Implement data access layer using Dapper for high-performance database operations.

**Acceptance Criteria:**
- [ ] ISensitiveWordRepository interface created in Domain
- [ ] SensitiveWordRepository implemented in Infrastructure
- [ ] Dapper configured with connection management
- [ ] All CRUD operations call stored procedures
- [ ] Integration tests with real database

**Technical Notes:**
- Use Dapper (not Entity Framework) for performance
- Call stored procedures for all operations
- Implement proper connection disposal

---

### Story 2.4: API Layer - REST Endpoints
**Type:** Story
**Priority:** High
**Story Points:** 5

**Description:**
As an API consumer, I need REST endpoints to manage sensitive words so that I can integrate with the service.

**Acceptance Criteria:**
- [ ] SensitiveWordsController created
- [ ] GET /api/sensitivewords (get all)
- [ ] GET /api/sensitivewords/{id} (get by id)
- [ ] POST /api/sensitivewords (create)
- [ ] PUT /api/sensitivewords/{id} (update)
- [ ] DELETE /api/sensitivewords/{id} (delete)
- [ ] POST /api/sanitize (sanitize message)
- [ ] Swagger documentation auto-generated
- [ ] API returns proper HTTP status codes

---

### Story 2.5: Message Sanitization Logic
**Type:** Story
**Priority:** Highest
**Story Points:** 8

**Description:**
As a user, I need to sanitize messages by replacing SQL keywords with asterisks so that I can filter dangerous content.

**Acceptance Criteria:**
- [ ] Sanitization algorithm implemented
- [ ] Detects all active sensitive words in message
- [ ] Replaces found words with asterisks (same length)
- [ ] Returns original message, sanitized message, and detected words
- [ ] Case-insensitive matching
- [ ] Word boundary detection
- [ ] Performance optimized for large messages
- [ ] Unit tests covering edge cases

**Example:**
```
Input: "SELECT * FROM users WHERE id=1"
Output: "****** * **** users WHERE id=1"
Detected: ["SELECT", "FROM"]
```

---

## Epic 3: Testing & Quality

### Story 3.1: Unit Testing Suite
**Type:** Technical Task
**Priority:** High
**Story Points:** 8

**Description:**
Implement comprehensive unit tests for all layers with mocked dependencies.

**Acceptance Criteria:**
- [ ] 53+ unit tests created
- [ ] Application layer: 100% coverage
- [ ] Domain layer: 95%+ coverage
- [ ] All tests use mocked dependencies
- [ ] Tests follow AAA pattern (Arrange, Act, Assert)
- [ ] xUnit, Moq, FluentAssertions configured

**Test Categories:**
- Command handler tests
- Query handler tests
- Domain entity tests
- Validation tests
- Edge case tests

---

### Story 3.2: Integration Testing Suite
**Type:** Technical Task
**Priority:** High
**Story Points:** 5

**Description:**
Implement integration tests that verify end-to-end functionality with real database.

**Acceptance Criteria:**
- [ ] 11+ integration tests created
- [ ] WebApplicationFactory configured
- [ ] Test database setup/teardown automated
- [ ] API endpoint tests
- [ ] Database integration tests
- [ ] Tests run independently

---

### Story 3.3: Code Coverage Reporting
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 3

**Description:**
Set up automated code coverage reporting with 85%+ target.

**Acceptance Criteria:**
- [ ] Coverlet configured
- [ ] ReportGenerator installed
- [ ] check-coverage.bat script created
- [ ] HTML coverage report generated
- [ ] Overall coverage: 85.6%+
- [ ] Coverage reports in TestResults folder

---

## Epic 4: Developer Experience & Tooling

### Story 4.1: Developer Helper Scripts - Database
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 5

**Description:**
Create automated scripts to simplify database operations for developers.

**Acceptance Criteria:**
- [ ] setup-database.bat (automated setup)
- [ ] reset-database.bat (drop and recreate)
- [ ] backup-database.bat (timestamped backups)
- [ ] generate-migration.bat (create migration files)
- [ ] seed-test-data.bat (add test data)
- [ ] All scripts include error handling
- [ ] Scripts provide clear user feedback

---

### Story 4.2: Developer Helper Scripts - Build & Test
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 5

**Description:**
Create automated scripts for building, testing, and validating the project.

**Acceptance Criteria:**
- [ ] clean-build.bat (clean build with validation)
- [ ] run-tests.bat (interactive test runner with options)
- [ ] validate-project.bat (pre-commit validation)
- [ ] check-coverage.bat (coverage analysis)
- [ ] install-tools.bat (install dev tools)
- [ ] All scripts exit with proper codes

---

### Story 4.3: Developer Helper Scripts - Runtime
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 3

**Description:**
Create scripts for running and monitoring the application.

**Acceptance Criteria:**
- [ ] run-api.bat (start API only)
- [ ] check-health.bat (system health check)
- [ ] view-logs.bat (log viewer with filters)
- [ ] Scripts check prerequisites
- [ ] Clear status messages

---

### Story 4.4: Full Stack Startup Scripts
**Type:** Technical Task
**Priority:** High
**Story Points:** 5

**Description:**
Create scripts that start both API and React frontend together for seamless development.

**Acceptance Criteria:**
- [ ] start-fullstack.bat (single window with PowerShell jobs)
- [ ] start-fullstack-windows.bat (separate windows)
- [ ] Checks database connectivity
- [ ] Builds .NET solution
- [ ] Installs npm dependencies if needed
- [ ] Opens browser automatically
- [ ] Handles errors gracefully

---

## Epic 5: React Frontend Development

### Story 5.1: React Project Setup
**Type:** Technical Task
**Priority:** High
**Story Points:** 3

**Description:**
Set up React project with TypeScript, Vite, and Tailwind CSS.

**Acceptance Criteria:**
- [ ] Vite project created with React + TypeScript
- [ ] Tailwind CSS configured
- [ ] Lucide React icons installed
- [ ] Inter font from Google Fonts
- [ ] Project structure organized by feature
- [ ] Environment variables configured

---

### Story 5.2: API Service Layer
**Type:** Technical Task
**Priority:** High
**Story Points:** 3

**Description:**
Create TypeScript service layer for API communication.

**Acceptance Criteria:**
- [ ] api.ts service file created
- [ ] sensitiveWordsApi object with all methods
- [ ] healthApi object for health checks
- [ ] TypeScript interfaces for all requests/responses
- [ ] Error handling implemented
- [ ] Configurable base URL

---

### Story 5.3: Message Sanitizer Component
**Type:** Story
**Priority:** High
**Story Points:** 5

**Description:**
As a user, I need a UI to test message sanitization so that I can see how SQL keywords are detected.

**Acceptance Criteria:**
- [ ] Text input for messages
- [ ] "Sanitize Message" button
- [ ] Display original and sanitized messages
- [ ] Show detected keywords count
- [ ] Show list of detected keywords
- [ ] Loading states
- [ ] Error handling
- [ ] Responsive design

---

### Story 5.4: Words Manager Component
**Type:** Story
**Priority:** High
**Story Points:** 8

**Description:**
As a user, I need a UI to manage sensitive words so that I can customize the detection list.

**Acceptance Criteria:**
- [ ] Display list of words (7 items visible, scrollable)
- [ ] Search/filter functionality
- [ ] Add new word form
- [ ] Inline editing of existing words
- [ ] Toggle active/inactive status
- [ ] Delete word with confirmation
- [ ] Show total and active word counts
- [ ] Loading states
- [ ] Error messages

---

### Story 5.5: Health Stats Dashboard
**Type:** Story
**Priority:** Medium
**Story Points:** 3

**Description:**
As a user, I need to monitor system health so that I know if the API is working properly.

**Acceptance Criteria:**
- [ ] Display uptime
- [ ] Display memory usage (MB)
- [ ] Display CPU time
- [ ] Display thread count
- [ ] Auto-refresh every 5 seconds
- [ ] Online/offline indicator
- [ ] Timestamp of last update
- [ ] Compact design at top of page

---

### Story 5.6: Interactive Help Guide
**Type:** Story
**Priority:** Low
**Story Points:** 5

**Description:**
As a user, I need contextual help so that I understand how to use each feature.

**Acceptance Criteria:**
- [ ] Floating help button (bottom right)
- [ ] Modal with tabbed interface
- [ ] Quick Start tab with 3-step guide
- [ ] Message Sanitizer help tab
- [ ] Words Manager help tab
- [ ] Health Stats help tab
- [ ] Examples and screenshots
- [ ] Close button

---

### Story 5.7: UI Layout and Navigation
**Type:** Story
**Priority:** High
**Story Points:** 5

**Description:**
As a user, I need a clean, organized interface so that I can easily navigate between features.

**Acceptance Criteria:**
- [ ] Header with logo and title
- [ ] Title centered with yellow color (#facc15)
- [ ] Logo positioned on left
- [ ] Tab navigation (Sanitizer, Words Manager)
- [ ] Active tab styling (lime green)
- [ ] Footer with API URL info
- [ ] Footer always visible at bottom
- [ ] Responsive layout
- [ ] Professional color scheme

---

## Epic 6: Documentation & Deployment

### Story 6.1: API Documentation
**Type:** Documentation
**Priority:** Medium
**Story Points:** 3

**Description:**
Create comprehensive API documentation for developers.

**Acceptance Criteria:**
- [ ] README.md for API project (simplified, human tone)
- [ ] Quick start guide
- [ ] API endpoint examples
- [ ] Common issues section
- [ ] Architecture overview
- [ ] Links to detailed docs

---

### Story 6.2: React Documentation
**Type:** Documentation
**Priority:** Medium
**Story Points:** 3

**Description:**
Create React frontend documentation for developers.

**Acceptance Criteria:**
- [ ] README.md for React project (simplified, human tone)
- [ ] Quick start guide
- [ ] Component descriptions
- [ ] Configuration guide
- [ ] Common issues section
- [ ] Deployment instructions

---

### Story 6.3: Root Project Documentation
**Type:** Documentation
**Priority:** Medium
**Story Points:** 2

**Description:**
Create main README summarizing the entire project.

**Acceptance Criteria:**
- [ ] README.md at project root (casual, friendly tone)
- [ ] Project overview
- [ ] Quick start with one command
- [ ] Links to sub-READMEs
- [ ] Architecture summary
- [ ] All helper scripts documented

---

### Story 6.4: Architecture Documentation
**Type:** Documentation
**Priority:** Medium
**Story Points:** 5

**Description:**
Create detailed architecture documentation explaining system design.

**Acceptance Criteria:**
- [ ] UMLDiagrams.md created
- [ ] Domain layer explained
- [ ] Application layer explained (CQRS)
- [ ] Infrastructure layer explained
- [ ] System flows documented
- [ ] Deployment architecture
- [ ] Use cases described
- [ ] No PlantUML code, just explanations

---

### Story 6.5: Swagger/OpenAPI Documentation
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 2

**Description:**
Configure Swagger for interactive API documentation.

**Acceptance Criteria:**
- [ ] Swagger UI enabled in development
- [ ] All endpoints documented
- [ ] Request/response examples
- [ ] Schema definitions
- [ ] Available at /swagger endpoint

---

## Epic 7: Monitoring & Logging

### Story 7.1: Structured Logging with Serilog
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 3

**Description:**
Implement structured logging throughout the application.

**Acceptance Criteria:**
- [ ] Serilog configured
- [ ] File logging to Logs folder
- [ ] Console logging in development
- [ ] Log levels configured (Debug, Info, Warning, Error)
- [ ] Structured log format
- [ ] Log rotation configured

---

### Story 7.2: Health Check Endpoint
**Type:** Story
**Priority:** Medium
**Story Points:** 3

**Description:**
As a system administrator, I need a health check endpoint so that I can monitor service availability.

**Acceptance Criteria:**
- [ ] GET /health endpoint
- [ ] Returns system metrics (uptime, memory, CPU, threads)
- [ ] Returns 200 OK when healthy
- [ ] JSON response with detailed stats
- [ ] Fast response time (<100ms)

---

## Epic 8: Performance & Security

### Story 8.1: Performance Optimization
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 3

**Description:**
Optimize application performance for production use.

**Acceptance Criteria:**
- [ ] Dapper used instead of Entity Framework
- [ ] Database indexes on frequently queried columns
- [ ] Stored procedures for all database operations
- [ ] Async/await throughout
- [ ] Connection pooling configured
- [ ] Response time <200ms for most operations

---

### Story 8.2: Input Validation
**Type:** Technical Task
**Priority:** High
**Story Points:** 3

**Description:**
Implement comprehensive input validation to prevent bad data.

**Acceptance Criteria:**
- [ ] FluentValidation configured
- [ ] Validation for all command requests
- [ ] Word cannot be empty
- [ ] Word length limits enforced
- [ ] Proper error messages
- [ ] 400 Bad Request on validation failures

---

### Story 8.3: Error Handling Middleware
**Type:** Technical Task
**Priority:** Medium
**Story Points:** 2

**Description:**
Implement global error handling middleware for consistent error responses.

**Acceptance Criteria:**
- [ ] Global exception handler middleware
- [ ] Consistent error response format
- [ ] Proper HTTP status codes
- [ ] Error logging
- [ ] Doesn't expose sensitive information

---

## Backlog Items (Future Enhancements)

### Story 9.1: Authentication & Authorization
**Type:** Story
**Priority:** Low
**Story Points:** 8

**Description:**
Add JWT-based authentication and role-based authorization.

**Acceptance Criteria:**
- [ ] JWT authentication configured
- [ ] User registration/login endpoints
- [ ] Role-based access control
- [ ] Admin-only endpoints for sensitive operations
- [ ] Token refresh mechanism

---

### Story 9.2: Rate Limiting
**Type:** Technical Task
**Priority:** Low
**Story Points:** 3

**Description:**
Implement rate limiting to prevent API abuse.

**Acceptance Criteria:**
- [ ] Rate limiting middleware
- [ ] Configurable limits per endpoint
- [ ] 429 Too Many Requests response
- [ ] Rate limit headers in response

---

### Story 9.3: Caching Layer
**Type:** Technical Task
**Priority:** Low
**Story Points:** 5

**Description:**
Add Redis caching for frequently accessed data.

**Acceptance Criteria:**
- [ ] Redis configured
- [ ] Cache for GetAll words query
- [ ] Cache invalidation on updates
- [ ] Configurable TTL
- [ ] Fallback to database if cache fails

---

### Story 9.4: Docker Support
**Type:** Technical Task
**Priority:** Low
**Story Points:** 5

**Description:**
Containerize the application with Docker.

**Acceptance Criteria:**
- [ ] Dockerfile for API
- [ ] Dockerfile for React
- [ ] docker-compose.yml for full stack
- [ ] SQL Server in container
- [ ] Health checks in compose file
- [ ] Volume mounts for data persistence

---

### Story 9.5: CI/CD Pipeline
**Type:** Technical Task
**Priority:** Low
**Story Points:** 8

**Description:**
Set up automated CI/CD pipeline.

**Acceptance Criteria:**
- [ ] GitHub Actions or Azure DevOps pipeline
- [ ] Automated build on push
- [ ] Run all tests
- [ ] Code coverage check (85% minimum)
- [ ] Deploy to staging on merge to main
- [ ] Manual approval for production

---

## Story Summary by Epic

| Epic | Stories | Total Points |
|------|---------|-------------|
| 1. Foundation & Architecture | 2 | 13 |
| 2. Core API Development | 5 | 29 |
| 3. Testing & Quality | 3 | 16 |
| 4. Developer Experience | 4 | 18 |
| 5. React Frontend | 7 | 32 |
| 6. Documentation | 5 | 15 |
| 7. Monitoring & Logging | 2 | 6 |
| 8. Performance & Security | 3 | 8 |
| 9. Backlog (Future) | 5 | 29 |
| **Total** | **36** | **166** |

---

## Labels/Tags to Use

- `api` - Backend API work
- `frontend` - React frontend work
- `testing` - Testing related
- `documentation` - Documentation work
- `devops` - Developer tooling and scripts
- `bug` - Bug fixes
- `enhancement` - New features
- `performance` - Performance improvements
- `security` - Security related

---

## Definition of Done (DoD)

For each story to be considered complete:
- [ ] Code written and peer reviewed
- [ ] Unit tests written (if applicable)
- [ ] Integration tests written (if applicable)
- [ ] Documentation updated
- [ ] Code coverage maintained at 85%+
- [ ] No high-priority bugs
- [ ] Tested in development environment
- [ ] Code merged to main branch

---

## Sprint Suggestions

**Sprint 1 (Foundation):** Stories 1.1, 1.2, 2.1
**Sprint 2 (Core API):** Stories 2.2, 2.3, 2.4, 2.5
**Sprint 3 (Testing):** Stories 3.1, 3.2, 3.3
**Sprint 4 (Developer Tools):** Stories 4.1, 4.2, 4.3, 4.4
**Sprint 5 (React - Core):** Stories 5.1, 5.2, 5.3, 5.4
**Sprint 6 (React - Polish):** Stories 5.5, 5.6, 5.7
**Sprint 7 (Documentation):** Stories 6.1, 6.2, 6.3, 6.4, 6.5
**Sprint 8 (Production Ready):** Stories 7.1, 7.2, 8.1, 8.2, 8.3

---

## Notes

- All points are estimated using Fibonacci sequence (1, 2, 3, 5, 8, 13)
- Adjust story points based on team velocity
- Review and refine stories during sprint planning
- Break down larger stories (8+ points) if needed
- Backlog items are prioritized for future sprints
