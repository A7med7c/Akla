# Restaurants API
A Clean Architecture ASP.NET Core Web API for managing restaurants, dishes, carts, orders, payments, and user identity operations.

## Features
- Restaurant management via MediatR commands/queries: create, update, delete, get by id, and paged/filterable list (`GetAllRestaurantsQuery` supports search, paging, and sorting).
- Restaurant logo upload to Azure Blob Storage (`UploadRestaurantLogoCommand`) with external-storage error handling.
- Dishes management per restaurant: create, list, get by id, update, and delete.
- Shopping cart workflows for authenticated users: get cart, add item, update item quantity, remove item.
- Order workflows for authenticated users: create order, checkout, get order by id, and update order status.
- Paymob payment integration: checkout can generate a Paymob payment URL and callback endpoint updates payment status.
- Identity features:
  - Built-in ASP.NET Core Identity API endpoints mapped under `/api/identity` (`MapIdentityApi<User>()`).
  - Custom admin endpoints for assigning/unassigning user roles and updating user details.
- Authorization with roles (`Admin`, `Owner`, `User`, `Driver`) and custom policies (`HasNationality`, `AtLeast20`, `OwnsTwoRestaurants`) plus custom claims principal factory.
- FluentValidation validators across restaurant, dish, cart, order, and payment commands/queries.
- Global error handling middleware and request-time logging middleware.
- Serilog logging configured for Console, File, and Application Insights sinks.
- Startup seeding and automatic EF Core migration application via `RestaurantSeeder`.

## Tech Stack
| Language/Framework | Version |
|---|---|
| .NET / ASP.NET Core Web API | `net9.0` |
| ASP.NET Core Identity EF Core | `9.0.4` |
| Entity Framework Core SQL Server | `9.0.4` |
| MediatR | `12.5.0` |
| FluentValidation.AspNetCore | `11.3.0` |
| AutoMapper | `13.0.1` |
| Serilog.AspNetCore | `9.0.0` |
| Serilog.Settings.Configuration | `9.0.0` |
| Serilog.Sinks.Console | `6.0.0` |
| Serilog.Sinks.File | `6.0.0` |
| Serilog.Sinks.ApplicationInsights | `5.0.1` |
| Serilog.Formatting.Compact | `3.0.0` |
| Swashbuckle.AspNetCore (Swagger/SwaggerGen/SwaggerUI) | `8.1.1` |
| Azure.Storage.Blobs | `12.29.0-beta.1` |
| xUnit (test project) | `2.5.3` |
| Microsoft.NET.Test.Sdk (test project) | `17.8.0` |

## Architecture
This solution follows a Clean Architecture + CQRS style with MediatR:

`API -> Application -> Domain`
`          ↓`
`   Infrastructure`

- **API (`Restaurants.Api`)**: HTTP endpoints, middleware, Swagger setup, and host startup.
- **Application (`Restaurants.Application`)**: use cases (commands/queries/handlers), DTOs, validators, mapping, and user context abstraction.
- **Domain (`Restaurants.Dpomain`)**: entities, repository contracts, domain constants, and domain exceptions.
- **Infrastructure (`Restaurants.Infrastructure`)**: EF Core persistence, repository implementations, Identity/auth wiring, authorization handlers, payments, blob storage, and seeding.

## Project Structure
```text
.
├── Restaurants.Api/                 # Presentation layer (controllers, middleware, startup)
├── Restaurants.Application/         # Application layer (CQRS handlers, validators, DTOs)
├── Restaurants.Dpomain/             # Domain layer (entities, contracts, constants, exceptions)
├── Restaurants.Infrastructure/      # Infrastructure layer (EF Core, repositories, auth, payments)
├── Restaurants.Application.Tests/   # Test project for application layer
├── .github/workflows/               # CI workflows folder (currently empty)
├── Restaurants.sln                  # Solution file
└── README.md
```

## Getting Started
### Prerequisites
- .NET SDK 9.0
- SQL Server instance

### Installation & Setup
1. Restore dependencies:
   ```bash
   dotnet restore Restaurants.sln
   ```
2. Configure `Restaurants.Api/appsettings.Development.json`:
   - `ConnectionStrings:RestaurantsDb`
   - `Paymob:ApiKey`, `Paymob:IntegrationId`, `Paymob:IframeId`
   - `BlobStorage:ConnectionString`, `BlobStorage:LogosContainerName`
3. Migrations are applied automatically at startup by `RestaurantSeeder` (`Database.MigrateAsync()`).

Optional manual migration update:
```bash
dotnet ef database update --project Restaurants.Infrastructure --startup-project Restaurants.Api
```

### Running the project
```bash
dotnet run --project Restaurants.Api
```

Swagger/OpenAPI is enabled in Development.

## API Endpoints
### Restaurants
- `GET /api/restaurants` — Get paged/filtered/sorted restaurants.
- `GET /api/restaurants/{id}` — Get restaurant by id.
- `POST /api/restaurants` — Create restaurant.
- `POST /api/restaurants/{id}/logos` — Upload restaurant logo.
- `PATCH /api/restaurants/{id}` — Update restaurant.
- `DELETE /api/restaurants/{id}` — Delete restaurant.

### Dishes
- `POST /api/restaurants/{restaurantId}/dishes` — Create dish for restaurant.
- `GET /api/restaurants/{restaurantId}/dishes` — List dishes for restaurant.
- `GET /api/restaurants/{restaurantId}/dishes/{dishId}` — Get dish by id for restaurant.
- `PUT /api/restaurants/{restaurantId}/dishes/{dishId}` — Update dish for restaurant.
- `DELETE /api/restaurants/{restaurantId}/dishes/{dishId}` — Delete dish for restaurant.

### Cart
- `GET /api/cart` — Get current authenticated user's cart.
- `POST /api/cart/items` — Add item to cart.
- `PATCH /api/cart/items/{id}` — Update cart item.
- `DELETE /api/cart/items/{id}` — Remove cart item.

### Orders
- `GET /api/orders/{id}` — Get order by id.
- `POST /api/orders` — Create order.
- `POST /api/orders/checkout` — Checkout current cart (supports payment method selection).
- `PATCH /api/orders/{id}/status` — Update order status.

### Payments
- `POST /api/payments/callback` — Handle Paymob callback and update payment status.

### Identity
- `PATCH /api/identity/user` — Update user details (marked `[AllowAnonymous]` in current controller).
- `POST /api/identity/userRole` — Assign role to user (admin controller).
- `DELETE /api/identity/userRole` — Unassign role from user (admin controller).
- Additional ASP.NET Core Identity endpoints are mapped under `/api/identity` via `MapIdentityApi<User>()`.

## Authentication & Authorization
- Authentication is enabled with ASP.NET Core Identity API endpoints (`AddIdentityApiEndpoints<User>()`).
- Authorization is role- and policy-based:
  - Roles: `Admin`, `Owner`, `User`, `Driver`.
  - Policies: `HasNationality`, `AtLeast20`, `OwnsTwoRestaurants`.
- Custom claims principal factory adds `Nationality` and `DateOfBirth` claims.
- Swagger is configured with a bearer security scheme.

## CI/CD & Deployment
- **CI/CD pipelines:** Not configured (no GitHub Actions workflow YAML files and no Azure DevOps pipeline YAML found).
- **Azure deployment IaC files:** Not configured.
- **Containerization:** `Restaurants.Api/Dockerfile` is present.
- **docker-compose:** Not configured.

## Running Tests
Test project found: `Restaurants.Application.Tests`.

Run tests with:
```bash
dotnet test Restaurants.Application.Tests/Restaurants.Application.Tests.csproj
```

## License
MIT
