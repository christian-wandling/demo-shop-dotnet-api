# Demo-Shop-DotNet-API

A learning project for ASP.NET Core that implements an e-commerce API using modern architecture patterns and best practices.

The project is a refactoring of a NestJS API found in this repository:

>http://github.com/christian-wandling/demo-shop-public


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

### Testing
- **xUnit**: Testing framework
- **AutoFixture**: Auto-generation of test data
- **FluentAssertions**: Fluent assertion syntax
- **NSubstitute**: Mocking framework

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/downloads)
- [Dotnet-ef](https://www.nuget.org/packages/dotnet-ef)
- [Dotnet-reportgenerator-globaltool](https://www.nuget.org/packages/dotnet-reportgenerator-globaltool) (optional - generate test coverage reports)
- Make (optional - run convenience commands like `make up)

### Installation
1. Clone the repository
> git clone https://github.com/christian-wandling/demo-shop-dotnet-api.git

2. Add an .env at the root of the repository
> see [.env.example](.env.example) for required variables

3. Add the default connection string to user secrets (to connect to the database container from the outside)
> dotnet user-secrets set "ConnectionStrings:LocalConnection" "Host=localhost; Port=5432; Database=***your_postgres_db***; Username=***your_postgres_user***; Password=***your_postgres_password***" --project src/DemoShop.Api

4. Create shared docker network to use the api with the frontend (or remove network from compose file)
> docker network create shared

4. Create the containers
> make up

5. Initialize database
> make db-update

### Usage

Browse the Swagger documentation
> http://localhost:3000/api

Or query the api from the command line 
> curl http://localhost:3000/api/v1/products

### Frontend Integration

The API can be consumed by the frontend application available in the following repository:
>http://github.com/christian-wandling/demo-shop-public

To use the frontend with this API, ensure both applications are running and connected via the shared Docker network.

### Authentication

1. Access the Keycloak admin console
> http://localhost:8080/admin/

2. Log in with the credentials defined in <our> [.env](.env) file.

3. Select the `demo_shop` realm, create a user and add credentials

4. Request an auth token from the Keycloak API
> curl -X POST \
> 'http://localhost:8080/realms/demo_shop/protocol/openid-connect/token' \
> -H 'Content-Type: application/x-www-form-urlencoded' \
> -d 'username=***your_username***&password=***your_password***&grant_type=password&client_id=demo_shop_ui'

### Testing

Run unit tests
> make test

Run integration tests
> make integrations-test

Create unit test coverage report
> make test-coverage

Create integration test coverage report
> make integration-test-report

## License

This project is licensed under the MIT License - see the LICENSE file for details.


