# Akla

A Clean Architecture ASP.NET Core Web API for managing restaurants, dishes, carts, orders, payments, and user identity operations.

## Features

- Restaurant management via MediatR commands/queries: create, update, delete, get by id, and paged/filterable list (`GetAllRestaurantsQuery` supports search, paging, and sorting).
- Restaurant logo upload to Azure Blob Storage (`UploadRestaurantLogoCommand`) with external-storage error handling.
- Dishes management per restaurant: create, list, get by id, update, and delete.
- Shopping cart workflows for authenticated users: get cart, add item, update item quantity, remove item.
- Order workflows for authenticated users: create order, checkout, get order by id, and update order status.
- Paymob payment integration: checkout generates a Paymob payment URL and a callback endpoint updates payment status.
- Identity features:
  - Built-in ASP.NET Core Identity API endpoints mapped under `/api/identity` (`MapIdentityApi<User>()`).
  - Custom admin endpoints for assigning/unassigning user roles and updating user details.
- Authorization with roles (`Admin`, `Owner`, `User`, `Driver`) and custom policies (`HasNationality`, `AtLeast20`, `OwnsTwoRestaurants`) plus a custom claims principal factory.
- FluentValidation validators across restaurant, dish, cart, order, and payment commands/queries.
- Global error handling middleware and request-time logging middleware.
- Serilog logging configured for Console, File, and Application Insights sinks.
- Startup seeding and automatic EF Core migration application via `RestaurantSeeder`.
- Unit test coverage for API middleware, application validation/handlers, user context behavior, and infrastructure authorization requirements.
- GitHub Actions CI/CD pipelines for pull request validation, release publishing, artifact upload, and Azure Web App deployment.

## Tech Stack

| Language / Framework              | Version                              |
| --------------------------------- | ------------------------------------ |
| .NET / ASP.NET Core Web API       | `net9.0`                             |
| ASP.NET Core Identity EF Core     | `9.0.4`                              |
| Entity Framework Core SQL Server  | `9.0.4`                              |
| MediatR                           | `12.5.0`                             |
| FluentValidation.AspNetCore       | `11.3.0`                             |
| AutoMapper                        | `13.0.1`                             |
| Serilog.AspNetCore                | `9.0.0`                              |
| Serilog.Settings.Configuration    | `9.0.0`                              |
| Serilog.Sinks.Console             | `6.0.0`                              |
| Serilog.Sinks.File                | `6.0.0`                              |
| Serilog.Sinks.ApplicationInsights | `5.0.1`                              |
| Serilog.Formatting.Compact        | `3.0.0`                              |
| Swashbuckle.AspNetCore (Swagger)  | `8.1.1`                              |
| Azure.Storage.Blobs               | `12.29.0-beta.1`                     |
| Paymob                            | Payment gateway (custom integration) |
| xUnit v3 MTP                      | `3.2.2`                              |
| Moq                               | `4.20.72`                            |

## Architecture

This solution follows Clean Architecture with a CQRS pattern implemented via MediatR:

```
API → Application → Domain
           ↓
    Infrastructure
```

- **API (`Restaurants.Api`)**: HTTP endpoints, middleware, Swagger setup, and host startup.
- **Application (`Restaurants.Application`)**: use cases (commands/queries/handlers), DTOs, validators, mapping, and user context abstraction.
- **Domain (`Restaurants.Domain`)**: entities, repository contracts, domain constants, and domain exceptions.
- **Infrastructure (`Restaurants.Infrastructure`)**: EF Core persistence, repository implementations, Identity/auth wiring, authorization handlers, payments, blob storage, and seeding.

## Project Structure

```text
.
├── Restaurants.Api/                  # Presentation layer (controllers, middleware, startup)
├── Restaurants.Application/          # Application layer (CQRS handlers, validators, DTOs)
├── Restaurants.Domain/               # Domain layer (entities, contracts, constants, exceptions)
├── Restaurants.Infrastructure/       # Infrastructure layer (EF Core, repositories, auth, payments)
├── Restaurants.Api.Test/             # Unit tests for API middleware and presentation behavior
├── Restaurants.Domain.Test/          # Unit tests for application/domain-facing behavior
├── Restaurants.Infrastructure.Test/  # Unit tests for infrastructure authorization behavior
├── .github/workflows/                # GitHub Actions CI/CD workflows
├── Restaurants.sln                   # Solution file
└── README.md
```

## Getting Started

### Prerequisites

- .NET SDK 9.0
- SQL Server instance
- (Optional) Docker

### Installation & Setup

1. Clone the repository and restore dependencies:

   ```bash
   git clone https://github.com/A7med7c/Akla.git
   cd Akla
   dotnet restore Restaurants.sln
   ```

2. Configure `Restaurants.Api/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "RestaurantsDb": "<your-sql-server-connection-string>"
     },
     "Paymob": {
       "ApiKey": "<your-paymob-api-key>",
       "IntegrationId": "<your-integration-id>",
       "IframeId": "<your-iframe-id>"
     },
     "BlobStorage": {
       "ConnectionString": "<your-azure-blob-connection-string>",
       "LogosContainerName": "<your-container-name>"
     }
   }
   ```

3. Migrations are applied automatically at startup via `RestaurantSeeder`. To apply manually:
   ```bash
   dotnet ef database update --project Restaurants.Infrastructure --startup-project Restaurants.Api
   ```

### Running the project

```bash
dotnet run --project Restaurants.Api
```

Swagger UI is available at `https://localhost:{port}/swagger` in Development mode.

### Running with Docker

A `Dockerfile` is included in `Restaurants.Api`. To build and run:

```bash
docker build -f Restaurants.Api/Dockerfile -t akla-api .
docker run -p 8080:80 \
  -e ConnectionStrings__RestaurantsDb="<connection-string>" \
  akla-api
```

## API Endpoints

### Restaurants

| Method   | Route                         | Description                                         |
| -------- | ----------------------------- | --------------------------------------------------- |
| `GET`    | `/api/restaurants`            | Get paged, filtered, and sorted list of restaurants |
| `GET`    | `/api/restaurants/{id}`       | Get restaurant by id                                |
| `POST`   | `/api/restaurants`            | Create a new restaurant                             |
| `POST`   | `/api/restaurants/{id}/logos` | Upload restaurant logo to Azure Blob Storage        |
| `PATCH`  | `/api/restaurants/{id}`       | Update restaurant details                           |
| `DELETE` | `/api/restaurants/{id}`       | Delete restaurant                                   |

### Dishes

| Method   | Route                                             | Description                      |
| -------- | ------------------------------------------------- | -------------------------------- |
| `POST`   | `/api/restaurants/{restaurantId}/dishes`          | Create a dish for a restaurant   |
| `GET`    | `/api/restaurants/{restaurantId}/dishes`          | List all dishes for a restaurant |
| `GET`    | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Get a specific dish              |
| `PUT`    | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Update a dish                    |
| `DELETE` | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Delete a dish                    |

### Cart

| Method   | Route                  | Description               |
| -------- | ---------------------- | ------------------------- |
| `GET`    | `/api/cart`            | Get current user's cart   |
| `POST`   | `/api/cart/items`      | Add item to cart          |
| `PATCH`  | `/api/cart/items/{id}` | Update cart item quantity |
| `DELETE` | `/api/cart/items/{id}` | Remove item from cart     |

### Orders

| Method  | Route                     | Description                                |
| ------- | ------------------------- | ------------------------------------------ |
| `GET`   | `/api/orders/{id}`        | Get order by id                            |
| `POST`  | `/api/orders`             | Create a new order                         |
| `POST`  | `/api/orders/checkout`    | Checkout cart (returns Paymob payment URL) |
| `PATCH` | `/api/orders/{id}/status` | Update order status                        |

### Payments

| Method | Route                    | Description                              |
| ------ | ------------------------ | ---------------------------------------- |
| `POST` | `/api/payments/callback` | Paymob callback — updates payment status |

### Identity

| Method   | Route                    | Description                          |
| -------- | ------------------------ | ------------------------------------ |
| `PATCH`  | `/api/identity/user`     | Update user details                  |
| `POST`   | `/api/identity/userRole` | Assign role to user (Admin only)     |
| `DELETE` | `/api/identity/userRole` | Unassign role from user (Admin only) |

> Additional built-in ASP.NET Core Identity endpoints (register, login, refresh token, etc.) are available under `/api/identity` via `MapIdentityApi<User>()`.

## Authentication & Authorization

- Authentication is provided by ASP.NET Core Identity API endpoints (`AddIdentityApiEndpoints<User>()`).
- Swagger is configured with a Bearer security scheme for testing authenticated endpoints.
- Authorization is role- and policy-based:

| Type     | Values                                              |
| -------- | --------------------------------------------------- |
| Roles    | `Admin`, `Owner`, `User`, `Driver`                  |
| Policies | `HasNationality`, `AtLeast20`, `OwnsTwoRestaurants` |

- A custom claims principal factory injects `Nationality` and `DateOfBirth` claims into the user's identity.

## CI/CD & Deployment

GitHub Actions workflows are configured under `.github/workflows/`:

- **CI (`main.yml`)**: runs on pull requests targeting `master` and on manual `workflow_dispatch`. The workflow restores dependencies, builds the solution, and runs `dotnet test`.
- **CD (`release.yml`)**: runs on pushes to `master` and on manual `workflow_dispatch`. The workflow restores dependencies, builds in Release mode, publishes `Restaurants.Api`, uploads the API artifact, and deploys to the Azure Web App `Akla-api-dev` using the `PUBLISH_PROFILE_DEV` secret.
- **Containerization:** `Dockerfile` present in `Restaurants.Api/` — see [Running with Docker](#running-with-docker) above.

## Running Tests

Run all tests from the solution root:

```bash
dotnet test
```

Run a specific test project:

```bash
dotnet test Restaurants.Api.Test/Restaurants.Api.Test.csproj
dotnet test Restaurants.Domain.Test/Restaurants.Application.Test.csproj
dotnet test Restaurants.Infrastructure.Test/Restaurants.Infrastructure.Test.csproj
```

Current test coverage includes:

- `ErrorHandlingMiddleware` success and exception mapping behavior.
- `CreateRestaurantCommandValidator` valid and invalid command scenarios.
- `CreateRestaurantCommandHandler` behavior.
- `OwnsTwoRestaurantsReqirementHandler` authorization success and failure scenarios.
- User and current-user context behavior.

## License
 
MIT
