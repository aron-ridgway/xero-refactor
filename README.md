# refactor-this project refactored
This project is a .NET 8 Minimal API designed to replace and improve upon a legacy Framework 4.5.2 project. Rather than attempting to upgrade and refactor the legacy project, it was determined that starting fresh with a new .NET Core Minimal API would be easier, more efficient, and less error-prone. The legacy Framework project had tightly coupled components, outdated dependencies, and architectural limitations that made unpicking and modernizing the codebase highly complex. Starting anew allowed the project to fully leverage modern .NET capabilities, resulting in improved performance, enhanced security, and maintainability while incorporating best practices in code design, testing, and infrastructure.

## Key Features
* Modern Framework: Built on .NET 8 for cutting-edge performance and features.
* ORM: Uses Dapper with MS SQL Server for lightweight, high-performance database interaction.
* Authentication: Secured with API Key authentication for simplicity and control.
* Testing: Integration tests implemented using Testcontainers for reliable, isolated test environments.
* Documentation and Testing: Integrated Swagger/OpenAPI for interactive API testing and comprehensive documentation.

## Code Design
* Minimal API: Simplified and modern API structure with focus on lightweight, fast endpoints.
* Singleton SQL Connection Factory: A singleton design pattern is used for the SQL connection factory to manage database connections efficiently.
* SOLID Principles: The codebase adheres to SOLID principles to ensure scalability, maintainability, and ease of understanding.
* Readability: Code is clean, commented, and follows C# coding conventions for ease of maintenance.

## Quality
* Meaningful Responses: Returns standard HTTP status codes.
* Database Connections with Using Statements: All database connections are explicitly wrapped in using statements to ensure proper disposal and prevent connection leaks.

## Error Handling
* Global Error Handling: Centralized exception handling via middleware for consistent error responses.
* Try-Catch in Services: Services include try-catch blocks to handle domain-specific exceptions and log detailed error information.
* Logging: Integrated with logging frameworks to capture errors and insights for better diagnostics.

## Code Performance
* Optimized Queries: Uses parameterized queries in Dapper to ensure efficient database interaction.
* Asynchronous Programming: Fully leverages async/await to maximize API throughput.

## Security
* HTTPS Enforcement: The API is configured to use HTTPS by default, ensuring secure communication.
* API Key Authentication: Provides secure access control with configurable API key validation.
* Input Validation: Strict validation of all incoming data to prevent injection attacks.

## Infra/Operability
* Containerization: Supports Docker for consistent deployment across environments.
* Testcontainers for Integration Tests: Provides isolated, realistic test environments with minimal setup.
* Observability: Integrated logging and monitoring tools to ensure reliability and quick troubleshooting in production.

## Documentation
* API Documentation: Swagger/OpenAPI is integrated into the project for:
* Auto-generated, interactive API documentation.
* Simplified testing of endpoints directly within the Swagger UI or using the .http file.
* Setup Guide: A detailed guide is provided for setup and configuration (see below).

## Testing
* Integration Tests: Comprehensive tests using Testcontainers to verify end-to-end API functionality.
* Endpoint Tests: Verified the API key Authentication for all endpoints

## Setup Guide
### Prerequisites
* .NET 8 SDK
* Docker (for running Testcontainers)

### Build and Run
```
dotnet build
dotnet run --launch-profile https
```

### Swagger Access
https://localhost:7239/swagger/index.html

### Configuration
* Update the appsettings.json or use environment variables for sensitive configurations like API keys and connection strings.

## Improvements

As the service grows, there are several improvements to consider implementing in the future to enhance performance, scalability, maintainability, and overall robustness of the application:

1. Caching (Redis, Memory, etc.)
Reasoning: Caching frequently accessed data, such as product information or user data, can significantly reduce database load and improve response times. Using a distributed cache like Redis would help improve performance in a scalable way, especially for large applications with many users or high traffic. In-memory caching could also be used for data that doesn't need to be persisted between application restarts but still benefits from being readily available.

2. Migrations (EF Core or Other Library for DB Management)
Reasoning: Migrations allow for controlled changes to the database schema, ensuring that the structure is updated safely as the application evolves. While Entity Framework Core (EF Core) can manage migrations out-of-the-box, alternative libraries could be used for more control or if lightweight database management is preferred. This would help automate the process of applying schema updates and maintaining consistency across different environments (e.g., dev, test, production).

3. API Keys Moved into the DB (Per Customer)
Reasoning: Storing API keys in the database, rather than hardcoded or in app settings, would enable more flexible key management. Each customer could have a unique API key, making it easier to track usage and manage permissions. This change would also provide better audit capabilities and simplify revocation or regeneration of keys per customer, improving overall security and customer management.

4. Repository Layer Between Services and DB
Reasoning: As the application scales, introducing a Repository Layer between the services and the database helps to decouple the data access logic from the business logic. This promotes separation of concerns, making the codebase more maintainable, testable, and flexible. A repository layer can also provide additional abstraction and the option to switch to a different database or ORM in the future without modifying the core service logic.

5. CI/CD Pipelines
Reasoning: Implementing CI/CD pipelines ensures that code is continuously integrated, tested, and deployed in an automated, repeatable, and consistent manner. It streamlines the development and release processes, reduces human error, and ensures that the codebase is always in a deployable state. CI/CD pipelines also make it easier to automate testing and deployment, enabling faster iteration and a more reliable production environment.

6. Resource Cleanup (e.g., Deleting Associated Data When Deleting a Product)
Reasoning: When deleting a product, it’s important to also delete any associated data, such as product options. This prevents orphaned data and ensures the database remains clean and consistent. Implementing cascading deletes or appropriate cleanup operations will help maintain data integrity and avoid unnecessary data bloat, which can slow down the system over time.

7. Move Primary Key Creation into the DB
Reasoning: Moving the creation of primary keys into the database (using auto-incrementing or GUID-based primary keys) ensures uniqueness and consistency across different environments. It also simplifies data management by centralizing primary key logic within the database, rather than relying on application-level logic, reducing the chance of collisions and improving overall data integrity.

8. Enforce Relationships with Foreign Keys (e.g., ProductOption.ProductID > Product.Id)
Reasoning: Enforcing relationships through foreign key constraints at the database level ensures referential integrity. This means that data is consistent across tables, and it prevents situations where a product option could exist without a corresponding product. Foreign keys enforce business rules directly in the database, reducing the risk of data inconsistency and errors that could arise from a lack of validation in the application layer.

