# Flash.SensitiveWords

A full-stack application for detecting and sanitizing SQL keywords in messages. Catches dangerous SQL words like SELECT, DROP, DELETE before they can do any damage.

## What's this project?

This is a microservice system with two parts:
1. **API Backend** - .NET 8 REST API that does the actual keyword detection and sanitization
2. **React Frontend** - A web UI where you can test things out and manage your word list

The idea is simple: you send a message, we scan it for SQL keywords, replace them with asterisks, and tell you what we found.

## Quick Start

The absolute fastest way to get everything running:

```bash
# From the project root
.\SpecialScripts\start-fullstack-windows.bat
```

This one script will:
- Check if your database is set up
- Build the .NET API
- Install npm packages if needed
- Start the API in one window
- Start React in another window
- Open your browser to http://localhost:5173

That's it. Everything just works.

## What you need installed

- .NET 8 SDK
- Node.js 18+ (with npm)
- SQL Server or SQL Server Express
- A web browser

## Project Structure

```
flash-sensitivewords/
├── SensitiveWords.MicroService/     # .NET 8 API
│   ├── SensitiveWords.API/          # REST endpoints
│   ├── SensitiveWords.Application/  # Business logic
│   ├── SensitiveWords.Domain/       # Core models
│   ├── SensitiveWords.Infrastructure/ # Database stuff
│   └── Database/Scripts/            # SQL setup scripts
│
├── SensitiveWords.React/            # React UI
│   ├── src/components/              # UI components
│   ├── src/services/                # API calls
│   └── src/types/                   # TypeScript types
│
├── SpecialScripts/                  # Helper scripts
│   ├── start-fullstack-windows.bat  # Start everything
│   ├── setup-database.bat           # Set up database
│   ├── run-tests.bat                # Run tests
│   └── ...and more
│
└── Documentation/                   # Architecture docs
    └── UMLDiagrams.md              # How everything fits together
```

## The Two Parts

### API (Backend)

**Tech:** .NET 8, ASP.NET Core, Dapper, SQL Server

The API handles:
- Scanning messages for SQL keywords
- Managing the sensitive words database
- Providing health monitoring
- Running on ports 64725 (HTTPS) and 64726 (HTTP)

**Test Coverage:** 85.6% overall (we actually care about this)

[Read more in the API README](SensitiveWords.MicroService/README.md)

### UI (Frontend)

**Tech:** React 18, TypeScript, Vite, Tailwind CSS

The UI gives you:
- Message sanitizer (test in real-time)
- Words manager (add, edit, delete keywords)
- Health dashboard (monitor the API)
- Help guide (built-in documentation)

Runs on port 5173 and talks to the API over HTTPS.

[Read more in the React README](SensitiveWords.React/README.md)

## Helpful Scripts

We made scripts for everything you'd normally type out. They're all in the `SpecialScripts` folder.

### Starting Things

```bash
# Start both API and React (recommended)
.\SpecialScripts\start-fullstack-windows.bat

# Or if you want them in one window
.\SpecialScripts\start-fullstack.bat

# Just the API
.\SpecialScripts\run-api.bat
```

### Database Stuff

```bash
# First time setup
.\SpecialScripts\setup-database.bat

# Start over fresh
.\SpecialScripts\reset-database.bat

# Make a backup
.\SpecialScripts\backup-database.bat

# Add test data
.\SpecialScripts\seed-test-data.bat
```

### Testing and Building

```bash
# Run tests
.\SpecialScripts\run-tests.bat

# Check code coverage
.\SpecialScripts\check-coverage.bat

# Clean build
.\SpecialScripts\clean-build.bat

# Check everything before committing
.\SpecialScripts\validate-project.bat
```

### Monitoring

```bash
# System health check
.\SpecialScripts\check-health.bat

# View logs
.\SpecialScripts\view-logs.bat
```

## How it works

1. You type a message like "SELECT * FROM users WHERE admin=true"
2. API scans it and finds "SELECT" and "FROM"
3. Returns "****** * **** users WHERE admin=true"
4. Also tells you it found 2 keywords

The sensitive words are stored in SQL Server, so you can add/remove them without restarting anything.

## Architecture

Built using Clean Architecture and CQRS:
- **Clean Architecture** means clear separation between business logic and infrastructure
- **CQRS** means we separate reading data from writing data (makes things faster)
- **DDD** means we model the business domain properly

If you want the full breakdown, check out the [Architecture Documentation](Documentation/UMLDiagrams.md).

## API Examples

**Sanitize a message:**
```bash
curl -X POST https://localhost:64725/api/sanitize \
  -H "Content-Type: application/json" \
  -d '{"message": "SELECT * FROM users"}'
```

**Get all sensitive words:**
```bash
curl https://localhost:64725/api/sensitivewords
```

**Add a new word:**
```bash
curl -X POST https://localhost:64725/api/sensitivewords \
  -H "Content-Type: application/json" \
  -d '{"word": "TRUNCATE"}'
```

Or just use the Swagger UI at https://localhost:64725/swagger

## Common Issues

**Nothing works?**
Run `.\SpecialScripts\check-health.bat` - it'll tell you what's wrong.

**Database errors?**
Run `.\SpecialScripts\setup-database.bat` to set it up.

**Port conflicts?**
Something's using your ports. Check what's running on 64725/64726 (API) or 5173 (React).

**Tests failing?**
Make sure your database is running and properly set up.

## Development Workflow

Here's how most people use this:

**Day one:**
```bash
.\SpecialScripts\install-tools.bat
.\SpecialScripts\setup-database.bat
.\SpecialScripts\start-fullstack-windows.bat
```

**Every day after:**
```bash
.\SpecialScripts\start-fullstack-windows.bat
```

**Before committing:**
```bash
.\SpecialScripts\validate-project.bat
```

## Test Coverage

We've got 64 tests with 85.6% coverage:
- 53 unit tests (fast, no database needed)
- 11 integration tests (full end-to-end with database)
- 100% coverage on application layer
- 94.5% coverage on domain layer

Run them with:
```bash
.\SpecialScripts\run-tests.bat
```

## Tech Stack Summary

**Backend:**
- .NET 8
- ASP.NET Core Web API
- Dapper (for speed over EF Core)
- SQL Server
- xUnit, Moq, FluentAssertions

**Frontend:**
- React 18
- TypeScript
- Vite
- Tailwind CSS
- Lucide React icons

**Tools:**
- Serilog for logging
- Coverlet for code coverage
- ReportGenerator for coverage reports

## What's Next?

Things you might want to add:
- Authentication/authorization
- Rate limiting
- Caching layer (Redis?)
- More test data
- Docker support
- CI/CD pipeline

The architecture supports all of this - it's built to grow.

## URLs

When everything's running:
- **React UI:** http://localhost:5173
- **API Swagger:** https://localhost:64725/swagger
- **API Health:** https://localhost:64725/health
- **API Base:** https://localhost:64725/api

## Getting Help

1. Check the READMEs (you're reading one now)
2. Look at the [Architecture Docs](Documentation/UMLDiagrams.md)
3. Run `.\SpecialScripts\check-health.bat`
4. Check browser console (F12) for frontend errors
5. Check the logs with `.\SpecialScripts\view-logs.bat`

## License

This is a university project for demonstrating microservice architecture, Clean Architecture, and CQRS patterns.

---

Made with .NET, React, and way too much coffee ☕
