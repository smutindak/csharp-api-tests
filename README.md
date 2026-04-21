# csharp-api-tests

A C# test automation framework demonstrating REST API testing, database testing, and multi-layer integration testing patterns using NUnit, RestSharp, Playwright, and Dapper.

## Tech Stack

| Category | Library |
|---|---|
| Language / Runtime | C# 12 / .NET 8.0 |
| Test Framework | NUnit 3.14 |
| HTTP / API Testing | RestSharp 114, Playwright 1.59 |
| Database | PostgreSQL via Npgsql 10 + Dapper 2.1 |
| Assertions | Shouldly 4.3 |
| Coverage | Coverlet |

## Project Structure

```
├── Models/
│   ├── User.cs                  # User entity
│   └── Post.cs                  # Post entity
├── Services/
│   ├── DbService.cs             # PostgreSQL CRUD operations
│   └── PostsService.cs          # RestSharp client for JSONPlaceholder API
├── Tests/
│   ├── DbTests.cs               # Database-layer tests
│   ├── PostsTests.cs            # REST API tests via RestSharp
│   ├── ApiDbTests.cs            # API + DB integration tests
│   └── PlaywrightApiTests.cs    # API tests via Playwright
└── .github/workflows/test.yml   # GitHub Actions CI pipeline
```

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 16+ running on `localhost:5432`

## Database Setup

Create the database, user, table, and seed data before running tests:

```sql
CREATE DATABASE qadb;
CREATE USER qauser WITH PASSWORD 'qapass';
GRANT ALL PRIVILEGES ON DATABASE qadb TO qauser;

\c qadb

CREATE TABLE users (
    id         SERIAL PRIMARY KEY,
    name       VARCHAR(100) NOT NULL,
    email      VARCHAR(100) UNIQUE NOT NULL,
    role       VARCHAR(50)  DEFAULT 'user',
    is_active  BOOLEAN      DEFAULT true,
    created_at TIMESTAMP    DEFAULT NOW()
);

INSERT INTO users (name, email, role) VALUES
    ('Alice Kamau',   'alice@example.com', 'admin'),
    ('Bob Otieno',    'bob@example.com',   'user'),
    ('Carol Wanjiku', 'carol@example.com', 'user');
```

## Running Tests

```bash
# Restore dependencies
dotnet restore

# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test class
dotnet test --filter "ClassName=CSharpApiTests.Tests.DbTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Test Suites

| Suite | Count | What it covers |
|---|---|---|
| `DbTests` | 5 | Query, insert, delete, and seed-data validation against PostgreSQL |
| `PostsTests` | 4 | GET, POST, DELETE against the [JSONPlaceholder](https://jsonplaceholder.typicode.com) public API via RestSharp |
| `ApiDbTests` | 4 | End-to-end: API call followed by database state assertion |
| `PlaywrightApiTests` | 4 | Same API endpoints exercised via Playwright's `IAPIRequestContext` |

## Configuration

Connection strings and base URLs are hardcoded in the service constructors:

- **Database** — `Services/DbService.cs`: `Host=localhost;Port=5432;Database=qadb;Username=qauser;Password=qapass`
- **API base URL** — `Services/PostsService.cs`: `https://jsonplaceholder.typicode.com`

Update these values directly if your environment differs.

## CI/CD

GitHub Actions runs on every push and pull request to `master`:

1. Spins up a PostgreSQL 16 service container
2. Seeds the database
3. Restores, builds, and runs all tests
4. Uploads `.trx` test result files as artifacts (retained 90 days)

See [.github/workflows/test.yml](.github/workflows/test.yml) for the full pipeline definition.
