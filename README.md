# Demo-Shop-DotNet-API

A learning project for ASP.NET Core that implements an e-commerce API using modern architecture patterns and best practices.
The project is a refactoring of an NestJS API found in this repository:
[github.com/christian-wandling/demo-shop-public](https://github.com/christian-wandling/demo-shop-public)

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
