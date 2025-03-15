# Demo-Shop-DotNet-API

A learning project for ASP.NET Core that implements an e-commerce API using modern architecture patterns and best practices.

The project is a refactoring of a NestJS API found in this repository:

```
http://github.com/christian-wandling/demo-shop-public
```

## Overview

This project serves as a practical exploration of ASP.NET Core, demonstrating how to build a robust e-commerce API using Domain-Driven Design (DDD) principles, Command Query Responsibility Segregation (CQRS), and the Repository Pattern.

## Technologies & Patterns

### Architecture
- **Domain-Driven Design (DDD)**: Focusing on the core domain and domain logic
- **CQRS**: Separating read and write operations for more scalable architecture
- **Repository Pattern**: Abstracting data access and persistence

### Backend Framework & ORM
- **ASP.NET Core**: Modern, cross-platform framework for building APIs
- **Entity Framework Core**: Object-relational mapper for database operations
- **PostgreSQL**: Relational database for data persistence

### Libraries & Tools
- **Ardalis.Endpoints**: Clean API endpoint architecture
- **Ardalis.Results**: Standardized operation results
- **Ardalis.GuardClauses**: Input validation and defensive programming
- **Serilog**: Structured logging
- **Sentry**: Error tracking and monitoring
- **FluentValidation**: Validation rules in a fluent interface
- **AutoMapper**: Object-to-object mapping
- **MediatR**: Implementing mediator pattern for CQRS
- **Scrutor**: Assembly scanning and decoration extensions for DI

### Containerization & Deployment

- **Docker**: Platform for developing, shipping, and running applications in containers
- **Docker Compose**: Tool for defining and running multi-container Docker applications

### Authentication & Authorization

- **Keycloak**: Open source Identity and Access Management solution

### Testing
- **xUnit**: Testing framework
- **AutoFixture**: Auto-generation of test data
- **FluentAssertions**: Fluent assertion syntax
- **NSubstitute**: Mocking framework
- **Coverlet**: Cross-platform code coverage library for .NET
- **ReportGenerator**: Tool for creating code coverage reports from Coverlet data

## Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/downloads)
- [Dotnet-ef](https://www.nuget.org/packages/dotnet-ef)
- [Dotnet-reportgenerator-globaltool](https://www.nuget.org/packages/dotnet-reportgenerator-globaltool) (optional - generate test coverage reports)
- Make (optional - run convenience commands like `make up)

### Installation
1. Clone the repository
```
git clone https://github.com/christian-wandling/demo-shop-dotnet-api.git
```

2. Set up environment variables

```
# Edit .env file with your configuration
cp .env.example .env
```

You can choose to omit configuring sentry or see the section on setting up [sentry](#sentry-setup)

3. Add the database connection string to user secrets (used by EF Core and integration tests)
```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost; Port=5432; Database=your_postgres_db; Username=your_postgres_user; Password=your_postgres_password" --project src/DemoShop.Api
```

4. Create shared docker network (or remove network from compose file)
```
docker network create shared
```

4. Create the containers
```
make up
```

5. Initialize database  
```
make db-update
```

### Usage

Browse the Swagger documentation
```
http://localhost:3000/api
```

Or query the api from the command line 
```
curl http://localhost:3000/api/v1/products
```

### Sentry Setup

1. Go to [sentry.io](https://sentry.io/welcome/) to create an account and follow the steps to create a project

2. Add configuration values to your .env file

```
npx @sentry/wizard@latest -i sourcemaps --saas
```

### Frontend Integration

The API can be consumed by the frontend application available in the following repository:
```
http://github.com/christian-wandling/demo-shop-public
```

To use the frontend with this API, ensure both applications are running and connected via the shared Docker network.

## Authentication

### Create a user via Keycloak admin console

1. Access the Keycloak server to add a user

```
http://localhost:8080/admin/master/console/#/demo_shop/users/add-user
```

2. To login use `KEYCLOAK_ADMIN` and `KEYCLOAK_ADMIN_PASSWORD` defined in your [.env](.env) file.

3. Fill `Email`, `First Name` and `Last name`

4. Navigate to the `Credentials` tab and use `Set Password` to create as password

5. Fill `Password` and `Password Confirmation` and deselect `Temporary`

### Get bearer token

Request an auth token from the Keycloak API
```
curl -X POST \
'http://localhost:8080/realms/demo_shop/protocol/openid-connect/token' \
-H 'Content-Type: application/x-www-form-urlencoded' \
-d 'username=your_username&password=your_password&grant_type=password&client_id=demo_shop_ui'
```

## Testing

Run unit tests
```
make test
```

Run integration tests
```
make integration-test
```

Create unit test coverage report
```
make test-coverage
```

Create integration test coverage report
```
make integration-test-coverage
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.


