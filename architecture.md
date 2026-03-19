# Full-Stack Application Architecture Template

> **Purpose:** Generic architecture blueprint for building a full-stack web application using **Clean Architecture** (.NET backend) and a **modular Angular frontend**. Replace `{AppName}` with your project name (e.g., `Acme`, `Atlas`, `Pulse`) throughout.

---

## Table of Contents

1. [High-Level Overview](#1-high-level-overview)
2. [Tech Stack Summary](#2-tech-stack-summary)
3. [Backend Architecture](#3-backend-architecture)
   - [3.1 Solution Structure](#31-solution-structure)
   - [3.2 Layer Dependencies](#32-layer-dependencies)
   - [3.3 Domain Layer](#33-domain-layer)
   - [3.4 Application Layer](#34-application-layer)
   - [3.5 Infrastructure Layer](#35-infrastructure-layer)
   - [3.6 API Layer](#36-api-layer)
   - [3.7 Configuration & Secrets](#37-configuration--secrets)
   - [3.8 Database & Migrations](#38-database--migrations)
4. [Frontend Architecture](#4-frontend-architecture)
   - [4.1 Project Setup](#41-project-setup)
   - [4.2 Module Architecture](#42-module-architecture)
   - [4.3 Core Module](#43-core-module)
   - [4.4 Shared Module](#44-shared-module)
   - [4.5 Feature Modules](#45-feature-modules)
   - [4.6 Layout Module](#46-layout-module)
   - [4.7 Routing & Navigation](#47-routing--navigation)
   - [4.8 State Management](#48-state-management)
   - [4.9 Styling Architecture](#49-styling-architecture)
   - [4.10 SSR Infrastructure](#410-ssr-infrastructure)
   - [4.11 Testing](#411-testing)
5. [End-to-End Patterns](#5-end-to-end-patterns)
   - [5.1 Authentication Flow](#51-authentication-flow)
   - [5.2 API Communication](#52-api-communication)
   - [5.3 Server-Side Pagination](#53-server-side-pagination)
   - [5.4 CRUD Feature Pattern](#54-crud-feature-pattern)
6. [Data Table Specification](#6-data-table-specification)
   - [6.1 Architecture Overview](#61-architecture-overview)
   - [6.2 Backend API Contract](#62-backend-api-contract)
   - [6.3 Frontend Table Utilities](#63-frontend-table-utilities)
   - [6.4 Component TypeScript Pattern](#64-component-typescript-pattern)
   - [6.5 Component HTML Template](#65-component-html-template)
   - [6.6 Column Filter Types](#66-column-filter-types)
   - [6.7 Global Table Styles](#67-global-table-styles)
   - [6.8 Service Layer Pattern](#68-service-layer-pattern)
   - [6.9 Checklist for Adding a New Table](#69-checklist-for-adding-a-new-table)
7. [Development Setup](#7-development-setup)
8. [Folder Structure Reference](#8-folder-structure-reference)

---

## 1. High-Level Overview

This is a full-stack web application using **Clean Architecture** on the backend and a **modular NgModule-based** Angular frontend. Communication is via REST API with JWT authentication.

```
┌─────────────────────────────────────────────────────┐
│                    Browser (SPA)                     │
│         Angular 21 + PrimeNG 21 + PrimeFlex 4       │
└──────────────────────┬──────────────────────────────┘
                       │ HTTPS (JWT Bearer)
                       ▼
┌─────────────────────────────────────────────────────┐
│                  ASP.NET Core API                    │
│           .NET 10 · REST · Swagger · CORS            │
├─────────────────────────────────────────────────────┤
│  Controllers → Services → EF Core → SQL Server       │
│  (Clean Architecture: Api → App → Infra → Domain)    │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
              ┌─────────────────┐
              │   SQL Server     │
              │   (LocalDB)      │
              └─────────────────┘
```

---

## 2. Tech Stack Summary

### Backend

| Technology | Version | Purpose |
|---|---|---|
| **.NET** | 10.0 | Runtime & SDK |
| **ASP.NET Core** | 10.0 | Web API framework |
| **Entity Framework Core** | 10.0 | ORM / data access |
| **SQL Server LocalDB** | — | Development database |
| **Swashbuckle** | 6.x | Swagger / OpenAPI documentation |
| **BCrypt.Net-Next** | 4.x | Password hashing |
| **JWT Bearer** | built-in | Authentication tokens |
| **Docker** | Linux containers | Containerization (optional) |

### Frontend

| Technology | Version | Purpose |
|---|---|---|
| **Angular** | 21.x | SPA framework |
| **TypeScript** | 5.x | Language |
| **PrimeNG** | 21.x | UI component library (Aura theme) |
| **PrimeFlex** | 4.x | CSS utility classes |
| **PrimeIcons** | 7.x | Icon library |
| **RxJS** | 7.x | Reactive programming |
| **Chart.js** | 4.x | Charts (via PrimeNG ChartModule) |
| **jsPDF** | 4.x | PDF export |
| **jspdf-autotable** | 5.x | PDF table generation |
| **xlsx** | 0.18.x | Excel/spreadsheet export |
| **Express** | 5.x | SSR server |
| **@angular/ssr** | 21.x | Server-side rendering |
| **Vitest** | 4.x | Unit testing |
| **Playwright** | 1.x | E2E testing |

---

## 3. Backend Architecture

### 3.1 Solution Structure

```
{AppName}.Backend/
├── {AppName}.slnx                     # XML-format solution file
├── Directory.Build.props              # Shared MSBuild properties
├── README.md
├── seed_data.sql                      # Reference SQL seed data
├── src/
│   ├── {App}.Api/                     # Presentation layer (controllers, config)
│   ├── {App}.Application/             # Business contracts (DTOs, interfaces)
│   ├── {App}.Domain/                  # Enterprise entities & constants
│   └── {App}.Infrastructure/          # Data access & service implementations
└── tests/
    └── {App}.Tests/                   # Unit & integration tests
```

The solution uses the new `.slnx` (XML) format instead of the traditional `.sln`.

### 3.2 Layer Dependencies

```
                    ┌─────────────┐
                    │  {App}.Api  │  ← Entry point (controllers, DI, middleware)
                    └──────┬──────┘
                           │ references
              ┌────────────┼────────────┐
              ▼                         ▼
    ┌──────────────────┐    ┌──────────────────────┐
    │ {App}.Application│    │ {App}.Infrastructure  │
    │ (interfaces/DTOs)│    │  (EF Core, services)  │
    └────────┬─────────┘    └───────┬──────┬────────┘
             │                      │      │
             │ references           │      │ references
             ▼                      ▼      ▼
        ┌──────────────┐   {App}.Application + {App}.Domain
        │ {App}.Domain │
        │  (entities)  │   ← Zero dependencies (root)
        └──────────────┘
```

**Key rule:** Domain has zero project references. Application depends only on Domain. Infrastructure implements Application interfaces. API wires everything together via dependency injection.

### 3.3 Domain Layer

**Project:** `{App}.Domain.csproj` — Target: `net10.0` — Dependencies: **none** (except `Microsoft.AspNetCore.Identity.EntityFrameworkCore` if using identity base types)

#### Entities

Define your domain entities here. Each entity typically includes audit timestamps and optimistic concurrency. Example structure:

| Entity | Key Fields | Notes |
|---|---|---|
| `User` | `Id` (GUID) | BCrypt password hash, role FK, `IsActive`, `RowVersion` |
| `Role` | `Id` (int) | Role definitions (e.g., Admin, Editor, Viewer) |
| `Product` | `Id` (GUID) or business key | Core business entity with domain-specific fields |
| `Order` | `Id` (GUID) | Transaction header — customer, order number, status |
| `OrderItem` | `Id` (GUID) | Transaction line items — links product to order |
| `Customer` | `Id` (GUID) | Customer master data |
| `Category` | `Id` (int) | Lookup/classification entity |
| `Status` | `Id` (int) | Status definitions (e.g., Draft, Active, Completed, Cancelled) |
| `AuditLog` | `Id` (GUID) | Audit trail entries |
| `RefreshToken` | `Id` (GUID) | JWT refresh token storage |
| `SystemSettings` | `Id` (int) | Application configuration key-value |

> **Tip:** Add more entities as your domain requires. Follow the same patterns below for each.

#### Entity Patterns

```csharp
// Primary keys: GUID for transactional entities, int for lookups, string for natural keys
// Audit timestamps on all entities:
public DateTime CreatedAt { get; set; }
public string CreatedBy { get; set; }
public DateTime? UpdatedAt { get; set; }
public string? UpdatedBy { get; set; }

// Optimistic concurrency via row versioning:
public byte[] RowVersion { get; set; }

// Navigation properties for EF relationships:
public Status Status { get; set; }
public ICollection<OrderItem> Items { get; set; }
```

#### Status Constants

```csharp
// Static class mapping status IDs (seeded in DB)
public static class StatusConstants
{
    public const int Draft = 1;
    public const int Active = 2;
    public const int Completed = 3;
    public const int Cancelled = 4;
    // Add domain-specific statuses as needed
}
```

### 3.4 Application Layer

**Project:** `{App}.Application.csproj` — Target: `net10.0` — References: `{App}.Domain`

**NuGet:** `Microsoft.AspNetCore.Authentication.JwtBearer`, `BCrypt.Net-Next`

#### Interfaces

Organized in `Interfaces/` directory. Each domain concept has its own interface:

| Interface | Purpose |
|---|---|
| `IAuthService` | Login, refresh token, logout |
| `IUserService` | User CRUD operations |
| `IProductService` | Product management |
| `IOrderService` | Composite: extends `IOrderQueryService` + `IOrderCommandService` |
| `IOrderQueryService` | Read-only order operations (ISP) |
| `IOrderCommandService` | Write-only order operations (ISP) |
| `ICustomerService` | Customer management |
| `IAuditService` | Composite: extends `IAuditLogger` + `IAuditReader` |
| `IAuditLogger` | Write-only audit logging (ISP) |
| `IAuditReader` | Read-only audit queries (ISP) |
| `IDashboardService` | Dashboard KPI aggregations |
| `IReportService` | Report data queries |
| `IProfileService` | User self-service profile |
| `ISystemSettingsService` | System configuration |

> **Interface Segregation Principle (ISP):** For complex services, split into separate read and write interfaces composed into a combined interface. Add more interfaces as your domain grows.

#### DTOs

Each domain concept has a dedicated `{Concept}Dtos.cs` file containing all related DTOs:

```
DTOs/
├── Auth/AuthDtos.cs
├── Common/CommonDtos.cs              # PagedResult<T>, shared types
├── Customers/CustomerDtos.cs
├── Dashboard/DashboardDtos.cs
├── Orders/OrderDtos.cs
├── Products/ProductDtos.cs
├── Reports/ReportDtos.cs
├── Settings/SettingsDtos.cs
├── Users/UserDtos.cs
└── {Feature}/{Feature}Dtos.cs        # One per domain concept
```

**DTO Patterns:**

```csharp
// C# records with positional parameters (immutable by default)
public record ProductDto(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    DateTime CreatedAt
);

// Request DTOs use DataAnnotations for validation
public record CreateProductRequest(
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(20)] string Code,
    [Range(0.01, 999999)] decimal Price
);

// Shared generic pagination result
public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
) {
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

### 3.5 Infrastructure Layer

**Project:** `{App}.Infrastructure.csproj` — Target: `net10.0` — References: `{App}.Application`, `{App}.Domain`

**NuGet:** `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`

#### Dependency Injection

**File:** `DependencyInjection.cs` — Extension method `AddInfrastructure(IConfiguration)` called from `Program.cs`.

```csharp
// DbContext configuration
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    })
    .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
);

// Service registrations (all Scoped lifetime)
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IProductService, ProductService>();
// ... register all services

// ISP: Single implementation registered as multiple interfaces
services.AddScoped<OrderService>();
services.AddScoped<IOrderService>(sp => sp.GetRequiredService<OrderService>());
services.AddScoped<IOrderQueryService>(sp => sp.GetRequiredService<OrderService>());
services.AddScoped<IOrderCommandService>(sp => sp.GetRequiredService<OrderService>());
```

#### DbContext

**File:** `Data/AppDbContext.cs`

- One `DbSet<>` property per entity
- `OnModelCreating`: calls `ApplyConfigurationsFromAssembly` + seeds lookup data (Roles, Statuses, SystemSettings)
- Configurations in `Data/Configurations/` — one file per entity

**Configuration patterns:**

```csharp
// Fluent API for keys, constraints, indexes
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Price).HasPrecision(18, 4);
        builder.Property(p => p.RowVersion).IsRowVersion();

        // Composite performance indexes
        builder.HasIndex(p => new { p.StatusId, p.CreatedAt });

        // Relationship with restricted delete
        builder.HasOne(p => p.Status)
               .WithMany()
               .HasForeignKey(p => p.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

#### Services

All in `Services/` directory. Each implements one or more Application interfaces.

**Service patterns:**

```csharp
public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // Read operations use AsNoTracking()
    public async Task<PagedResult<ProductDto>> GetProductsAsync(/* params */)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Status)
            .Include(p => p.Category)
            .Where(/* filters */);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(/* sort */)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(/* map fields */))
            .ToListAsync();

        return new PagedResult<ProductDto>(items, totalCount, page, pageSize);
    }
}
```

**Key patterns across all services:**
- Constructor injection of `AppDbContext` (and `IConfiguration` where needed)
- `AsNoTracking()` for all read-only queries
- Eager loading via `.Include()` / `.ThenInclude()`
- Manual entity-to-DTO mapping (no AutoMapper)
- BCrypt for password hashing in `AuthService`
- JWT generation with `SymmetricSecurityKey` in `AuthService`
- Refresh token rotation with SHA256 hashing

#### Database Seeder

**File:** `Services/DatabaseSeeder.cs` — Registered as concrete class. Called on application startup to seed initial data (admin user, lookup tables, demo data).

### 3.6 API Layer

**Project:** `{App}.Api.csproj` — SDK: `Microsoft.NET.Sdk.Web` — Target: `net10.0`

**NuGet:** `Swashbuckle.AspNetCore`, `Microsoft.EntityFrameworkCore.Design`

#### Program.cs — Complete Pipeline

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Infrastructure DI (DbContext + all services)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. CORS — allow frontend on any localhost port
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.SetIsOriginAllowed(origin =>
            new Uri(origin).Host is "localhost" or "127.0.0.1")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

// 3. Controllers + Response caching + Brotli/Gzip compression
builder.Services.AddControllers();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes
        .Concat(new[] { "application/json" });
});

// 4. In-memory cache for report data
builder.Services.AddMemoryCache();

// 5. JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });

// 6. Swagger with JWT support
builder.Services.AddSwaggerGen(/* Bearer security definition, XML comments */);

// Middleware pipeline
app.UseCors("AllowFrontend");
app.UseResponseCompression();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Startup tasks
await dbContext.Database.MigrateAsync();          // Auto-apply migrations
await DatabaseSeeder.SeedAsync();                  // Seed data
await dashboardService.GetDashboardStatsAsync();   // EF model warmup
```

#### Controllers

All controllers follow a consistent pattern. Example structure:

| Controller | Route | Auth | Injected Service |
|---|---|---|---|
| `AppBaseController` | — (abstract) | — | Provides `GetCurrentUserId()` from JWT claims |
| `AuthController` | `api/v1/auth` | **None** | `IAuthService` |
| `ProductsController` | `api/v1/products` | `[Authorize]` | `IProductService` |
| `OrdersController` | `api/v1/orders` | `[Authorize]` | `IOrderService` |
| `CustomersController` | `api/v1/customers` | `[Authorize]` | `ICustomerService` |
| `DashboardController` | `api/v1/dashboard` | `[Authorize]` | `IDashboardService` |
| `ReportsController` | `api/v1/reports` | `[Authorize]` | `IReportService` |
| `AuditController` | `api/v1/audit` | `[Authorize]` | `IAuditReader` |
| `UsersController` | `api/v1/users` | `[Authorize(Roles = "Admin")]` | `IUserService` |
| `SettingsController` | `api/v1/settings` | `[Authorize]` | `ISystemSettingsService` |
| `ProfileController` | `api/v1/profile` | `[Authorize]` | `IProfileService` |

> Add one controller per domain resource. Follow the same pattern for each new feature.

**Controller conventions:**
- All use `[ApiController]` and `[Produces("application/json")]`
- Routes follow `api/v1/{resource}` pattern
- Most extend `AppBaseController` for user ID extraction from JWT
- Each controller injects exactly one service interface
- Dashboard and report controllers use `[ResponseCache]` attributes

### 3.7 Configuration & Secrets

**appsettings.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database={AppName}Db;Trusted_Connection=True"
  },
  "Jwt": {
    "Key": "<64-character-symmetric-key>",
    "Issuer": "{App}.Api",
    "Audience": "{App}.Client",
    "ExpiresInMinutes": 1440,
    "RefreshTokenExpiresInDays": 7
  }
}
```

**Launch profiles (launchSettings.json):**

| Profile | URL | Use |
|---|---|---|
| `http` | `http://localhost:{PORT}` | Development |
| `https` | `https://localhost:{PORT}` | Development with TLS |
| `Container` | `8080` / `8081` | Docker |

### 3.8 Database & Migrations

- **Provider:** SQL Server (LocalDB for dev, full SQL Server for production)
- **Approach:** Code-First with Fluent API configurations
- **Resilience:** Retry-on-failure (3 retries, 5s max delay)
- **Concurrency:** Optimistic concurrency via `RowVersion` columns
- **Performance indexes:** Composite indexes for query performance, covering indexes for reports

**Migration commands:**

```bash
# Add a new migration
dotnet ef migrations add {MigrationName} --project src/{App}.Infrastructure --startup-project src/{App}.Api

# Apply migrations
dotnet ef database update --project src/{App}.Infrastructure --startup-project src/{App}.Api
```

---

## 4. Frontend Architecture

### 4.1 Project Setup

**Angular CLI project generated with NgModule pattern** (not standalone components).

**angular.json key settings:**

| Setting | Value |
|---|---|
| Builder | `@angular/build:application` |
| Prefix | `app` |
| Style language | `scss` |
| Default component style | `scss`, `skipTests: true`, `standalone: false` |
| Output mode | `server` (SSR-ready) |
| Budgets | Initial: warn 2 MB / error 3 MB |

**TypeScript (tsconfig.json):**

| Option | Value |
|---|---|
| `strict` | `true` |
| `target` | `ES2022` |
| `module` | `preserve` |
| `experimentalDecorators` | `true` |
| `strictTemplates` | `true` |
| `strictInjectionParameters` | `true` |

**Global styles loaded (in order):**
1. `src/primeicons-local.css` — Local PrimeIcons
2. `node_modules/primeflex/primeflex.css` — PrimeFlex utilities
3. `src/styles.scss` — Global custom styles

**Environment files:**

```typescript
// src/environments/environment.ts (development)
export const environment = {
  production: false,
  apiUrl: 'https://localhost:{PORT}/api/v1',
  appVersion: '1.0.0',
  appName: '{AppName}'
};

// src/environments/environment.prod.ts (production)
export const environment = {
  production: true,
  apiUrl: 'https://api.{appname}.app/api/v1',
  appVersion: '1.0.0',
  appName: '{AppName}'
};
```

### 4.2 Module Architecture

```
AppModule (root)
├── CoreModule (singleton — guards, interceptor, services, models)
├── LayoutModule (shell — header, sidebar, footer, main-layout)
├── SharedModule (reusable UI components, pipes, utils)
└── Feature Modules (lazy-loaded per route)
    ├── AuthModule
    ├── DashboardModule
    ├── ProductsModule
    ├── OrdersModule
    ├── CustomersModule
    ├── ReportsModule
    ├── AuditModule
    ├── AdminModule
    ├── ProfileModule
    └── {FeatureName}Module          ← Add more as needed
```

**AppModule (root):**

```typescript
@NgModule({
  declarations: [App],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    CoreModule,
    LayoutModule,
    ToastModule,
    ConfirmDialogModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    providePrimeNG({ theme: { preset: Aura } }),
    MessageService,
    ConfirmationService
  ],
  bootstrap: [App]
})
export class AppModule {}
```

### 4.3 Core Module

**Purpose:** Singleton services, authentication guard, HTTP interceptor, TypeScript models. Imported only once in `AppModule`.

```
src/app/core/
├── core.module.ts           # Singleton-enforced module, registers HTTP_INTERCEPTORS
├── guards/
│   └── auth.guard.ts        # CanActivate guard — checks AuthService.isAuthenticated
├── interceptors/
│   └── auth.interceptor.ts  # Adds JWT Bearer header, handles 401 token refresh
├── models/                  # TypeScript interfaces (one per domain concept)
│   ├── index.ts             # Barrel export
│   ├── common.model.ts      # PagedResult<T>, ApiError
│   ├── user.model.ts        # User, LoginRequest, LoginResponse, AuthState
│   ├── product.model.ts     # Product
│   ├── order.model.ts       # Order, OrderItem
│   ├── customer.model.ts    # Customer
│   ├── dashboard.model.ts   # DashboardStats
│   ├── audit.model.ts       # AuditLog
│   └── {feature}.model.ts   # Add one per domain concept
└── services/                # Application services (one per domain concept)
    ├── index.ts             # Barrel export
    ├── auth.service.ts      # BehaviorSubject<AuthState>, localStorage, JWT
    ├── product.service.ts   # HttpClient → PagedResult<Product>
    ├── order.service.ts
    ├── customer.service.ts
    ├── dashboard.service.ts
    ├── audit.service.ts
    ├── report.service.ts
    ├── user.service.ts
    ├── profile.service.ts
    ├── settings.service.ts
    └── {feature}.service.ts # Add one per domain concept
```

**Auth Guard pattern:**

```typescript
@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  canActivate(route: ActivatedRouteSnapshot): boolean {
    if (!this.authService.isAuthenticated) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: route.url.join('/') }
      });
      return false;
    }
    // Optional role check via route.data['role']
    return true;
  }
}
```

**Auth Interceptor pattern:**

```typescript
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req, next): Observable<HttpEvent<any>> {
    // Skip auth endpoints
    if (req.url.includes('/auth/login') || req.url.includes('/auth/refresh')) {
      return next.handle(req);
    }
    // Add Bearer token
    const authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${this.authService.getToken()}` }
    });
    return next.handle(authReq).pipe(
      catchError(err => {
        if (err.status === 401) {
          return this.handle401(authReq, next); // Silent refresh
        }
        return throwError(() => err);
      })
    );
  }
}
```

**Service pattern:**

```typescript
@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) {}

  getProducts(params: {
    page: number; pageSize: number; sortField?: string;
    sortOrder?: number; search?: string; category?: string;
  }): Observable<PagedResult<Product>> {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);
    // ... add optional params
    return this.http.get<PagedResult<Product>>(this.apiUrl, { params: httpParams });
  }
}
```

### 4.4 Shared Module

**Purpose:** Reusable UI components, pipes, and utilities imported by feature modules.

```
src/app/shared/
├── shared.module.ts         # Declares & exports all shared items
├── index.ts                 # Barrel export
├── components/              # Reusable components
│   ├── app-page-header/     # Standard page header with title, subtitle, icon, actions
│   ├── app-kpi-card/        # KPI summary card (icon, value, label)
│   ├── app-data-table/      # Configurable PrimeNG data table wrapper
│   ├── app-selection-table/ # Table with row selection
│   ├── app-status-tag/      # Status badge renderer
│   ├── app-form-field/      # Form field wrapper with label/validation
│   ├── app-confirm-dialog/  # Confirmation dialog
│   ├── app-action-dialog/   # Action dialog with details
│   ├── app-chart-card/      # Chart.js card wrapper
│   ├── app-date-range/      # Date range picker
│   └── app-stepper/         # Multi-step wizard component
├── pipes/
│   ├── utc-to-local.pipe.ts # UTC → local timezone conversion
│   └── {custom}.pipe.ts     # Add domain-specific pipes as needed
└── utils/
    ├── date.utils.ts        # Date formatting helpers
    ├── export.utils.ts      # PDF & Excel export (jsPDF, xlsx)
    └── table.utils.ts       # Table helper functions (see Section 6)
```

**SharedModule imports & re-exports** these PrimeNG modules for use in features:
`ButtonModule`, `DialogModule`, `InputTextModule`, `InputIconModule`, `IconFieldModule`, `TableModule`, `ToastModule`, `TagModule`, `SkeletonModule`, `ChartModule`, `TooltipModule`, `DatePicker`, `MultiSelectModule`

### 4.5 Feature Modules

**Lazy-loaded feature modules** under `src/app/features/`:

| Feature | Route | Module | Components |
|---|---|---|---|
| **auth** | `/login` | `AuthModule` | `LoginComponent` |
| **dashboard** | `/dashboard` | `DashboardModule` | `DashboardComponent` |
| **products** | `/products` | `ProductsModule` | `ProductListComponent`, `ProductCreateComponent` |
| **orders** | `/orders` | `OrdersModule` | `OrderCreateComponent`, `OrderListComponent`, `OrderDetailComponent` |
| **customers** | `/customers` | `CustomersModule` | `CustomerListComponent`, `CustomerCreateComponent` |
| **reports** | `/reports` | `ReportsModule` | Report components (one per report type) |
| **audit** | `/audit` | `AuditModule` | `ActivityHistoryComponent`, `TransactionLogsComponent` |
| **admin** | `/admin` | `AdminModule` | `UsersComponent`, `SettingsComponent`, + domain admin components |
| **profile** | `/profile` | `ProfileModule` | `ProfileComponent` |

> Add one feature module per domain area. Each module follows the same structure pattern below.

**Feature module pattern:**

```typescript
@NgModule({
  declarations: [FeatureListComponent, FeatureCreateComponent],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    // PrimeNG modules specific to this feature
    TableModule,
    ButtonModule,
    InputTextModule,
    // Feature routing
    RouterModule.forChild([
      { path: '', component: FeatureListComponent },
      { path: 'create', component: FeatureCreateComponent }
    ])
  ]
})
export class FeatureModule {}
```

### 4.6 Layout Module

**Purpose:** Application shell — header, sidebar, footer, main layout container.

```
src/app/layout/
├── layout.module.ts
├── main-layout/             # Shell component with sidebar + content area
│   ├── main-layout.component.ts
│   ├── main-layout.component.html
│   └── main-layout.component.scss
├── header/
│   ├── header.component.*
│   └── notification-bell/   # Notification bell with badge (optional)
│       └── notification-bell.component.*
├── sidebar/
│   ├── sidebar.component.*  # Collapsible nav with menu sections
└── footer/
    └── footer.component.*
```

**Key behaviors:**
- **MainLayout:** Responsive sidebar collapse at 768px breakpoint (`@HostListener('window:resize')`)
- **Header:** Toggle sidebar, online/offline status, notification polling, user menu (Profile, Logout)
- **Sidebar:** Menu defined as `MenuSection[]` with `NavItem[]`; auto-expands active parent on navigation; role-based visibility

### 4.7 Routing & Navigation

**Strategy:** All feature modules lazy-loaded via `loadChildren`. Main layout shell protected by `AuthGuard`.

```typescript
const routes: Routes = [
  // Unprotected
  { path: 'login', loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule) },

  // Protected shell
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule) },
      { path: 'products', loadChildren: () => import('./features/products/products.module').then(m => m.ProductsModule) },
      { path: 'orders', loadChildren: () => import('./features/orders/orders.module').then(m => m.OrdersModule) },
      { path: 'customers', loadChildren: () => import('./features/customers/customers.module').then(m => m.CustomersModule) },
      // ... all other features
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
```

### 4.8 State Management

**Pattern:** RxJS `BehaviorSubject` in services — no external state management library (no NgRx, NGXS, etc.)

```typescript
// Example: AuthService reactive state
@Injectable({ providedIn: 'root' })
export class AuthService {
  private authStateSubject = new BehaviorSubject<AuthState>({
    isAuthenticated: false,
    user: null,
    token: null
  });

  authState$ = this.authStateSubject.asObservable();

  get isAuthenticated(): boolean {
    return this.authStateSubject.value.isAuthenticated;
  }
}
```

**Lazy-load table pattern (used across all list components):**

```typescript
// Subject-based trigger → debounce → switchMap → API call
private loadTrigger$ = new Subject<void>();

ngOnInit() {
  this.loadTrigger$.pipe(
    debounceTime(this.firstLoadDone ? 300 : 50),
    switchMap(() => this.service.getData(this.params)),
    catchError(err => { /* handle error */ return of(null); }),
    takeUntilDestroyed(this.destroyRef)
  ).subscribe(result => {
    this.items = result.items;
    this.totalRecords = result.totalCount;
  });
}

onLazyLoad(event: TableLazyLoadEvent) {
  // Extract pagination, sort, filter params from event
  this.loadTrigger$.next();
}
```

### 4.9 Styling Architecture

**Global styles (`styles.scss`):**
- **Font:** Google Fonts `Inter` (300–700) via CSS import
- **Base:** 14px body, `#f1f5f9` background, `#1e293b` text
- **PrimeNG theme:** Aura preset (configured in `AppModule`)
- **DataTable globals:** 1px gridlines `#e2e8f0`, 2px header bottom border
- **Print styles:** Hides layout chrome, supports receipt paper
- **Responsive:** Stacked table layout at ≤768px
- **Utilities:** Copy button, empty message pattern

**Component styles:** Each component has scoped SCSS using Angular's ViewEncapsulation (default).

### 4.10 SSR Infrastructure

SSR is configured but **all routes render client-side** (`RenderMode.Client`). The infrastructure is in place for future SSR:

- `src/main.server.ts` — Re-exports `AppServerModule`
- `src/server.ts` — Express 5 server with `AngularNodeAppEngine`
- `src/app/app.module.server.ts` — Server module with `provideServerRendering`
- `src/app/app.routes.server.ts` — All routes `**` → `RenderMode.Client`

### 4.11 Testing

| Type | Runner | Config |
|---|---|---|
| Unit tests | **Vitest** (via `@angular/build:unit-test`) | `tsconfig.spec.json`, `vitest/globals` types |
| E2E tests | **Playwright** | Separate config, `npx playwright test` |

**Scripts:**
- `npm test` → Playwright E2E
- `npm run test:unit` → Vitest unit tests
- `npm run test:unit:watch` → Vitest watch mode
- `npm run e2e:headed` → Playwright headed mode
- `npm run e2e:ui` → Playwright UI mode

---

## 5. End-to-End Patterns

### 5.1 Authentication Flow

```
1. User enters credentials → POST /api/v1/auth/login
2. Backend validates with BCrypt → Returns { accessToken, refreshToken, user }
3. Frontend stores in localStorage → AuthService.authState$ updated
4. AuthInterceptor adds "Authorization: Bearer <token>" to all requests
5. On 401 → AuthInterceptor calls POST /api/v1/auth/refresh
6. If refresh succeeds → Retry original request with new token
7. If refresh fails → AuthService.logout() → Navigate to /login
```

### 5.2 API Communication

```
Frontend Service          →  Backend Controller  →  Backend Service  →  EF Core  →  SQL Server
(HttpClient + HttpParams)    (attribute routing)     (interface impl)    (LINQ)      (queries)

Response flow:
SQL Server → EF Core → DTO mapping (manual) → Controller → JSON → HttpClient → Component
```

**API URL convention:** `{environment.apiUrl}/{resource}` = `https://localhost:{PORT}/api/v1/{resource}`

### 5.3 Server-Side Pagination

**Backend:**
```csharp
// Controller accepts: page, pageSize, sortField, sortOrder, search, filters
// Service returns: PagedResult<T> { Items, TotalCount, Page, PageSize }

var query = _context.Entities.AsNoTracking();
query = ApplyFilters(query, filters);
var totalCount = await query.CountAsync();
var items = await query
    .OrderBy(sortField, sortOrder)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(e => MapToDto(e))
    .ToListAsync();
return new PagedResult<T>(items, totalCount, page, pageSize);
```

**Frontend:**
```typescript
// PrimeNG p-table with [lazy]="true"
<p-table [lazy]="true" (onLazyLoad)="onLazyLoad($event)" [totalRecords]="totalRecords">

// Component extracts params from TableLazyLoadEvent
onLazyLoad(event: TableLazyLoadEvent) {
  this.currentPage = (event.first! / event.rows!) + 1;
  this.currentPageSize = event.rows!;
  this.currentSortField = event.sortField as string;
  this.currentSortOrder = event.sortOrder!;
  // Extract column filters...
  this.loadTrigger$.next();
}
```

### 5.4 CRUD Feature Pattern

To add a new domain feature (e.g., "Suppliers"), replicate this structure:

**Backend:**
1. `{App}.Domain/Entities/Supplier.cs` — Entity class
2. `{App}.Infrastructure/Data/Configurations/SupplierConfiguration.cs` — EF config
3. `AppDbContext` — Add `DbSet<Supplier>` property
4. `{App}.Application/DTOs/Suppliers/SupplierDtos.cs` — Request/response records
5. `{App}.Application/Interfaces/ISupplierService.cs` — Service contract
6. `{App}.Infrastructure/Services/SupplierService.cs` — Implementation
7. `{App}.Infrastructure/DependencyInjection.cs` — Register service
8. `{App}.Api/Controllers/SuppliersController.cs` — REST endpoints
9. Add EF migration: `dotnet ef migrations add AddSupplier`

**Frontend:**
1. `src/app/core/models/supplier.model.ts` — TypeScript interfaces
2. `src/app/core/services/supplier.service.ts` — HttpClient service
3. `src/app/features/suppliers/` — Feature directory
   - `suppliers.module.ts` — NgModule with child routes
   - `supplier-list/supplier-list.component.*` — List with p-table
   - `supplier-create/supplier-create.component.*` — Form with validation
4. `src/app/app-routing-module.ts` — Add lazy route
5. `src/app/layout/sidebar/sidebar.component.ts` — Add menu item

---

## 6. Data Table Specification

This section defines the standard implementation pattern for server-side paginated, sortable, filterable data tables used across the application. All list views follow this spec.

### 6.1 Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│  PrimeNG p-table [lazy]="true"                                  │
│  ┌──────────┐  ┌──────────┐  ┌───────────┐  ┌───────────────┐  │
│  │ Paginate  │  │  Sort    │  │  Filter   │  │ Empty State   │  │
│  │ + Summary │  │ per col  │  │ per col   │  │ + Loading     │  │
│  └─────┬─────┘  └─────┬────┘  └─────┬─────┘  └───────────────┘  │
│        │              │              │                            │
│        └──────────────┼──────────────┘                            │
│                       ▼                                          │
│              onLazyLoad(event)                                   │
│                       │                                          │
│              toLazyQueryParams(event) ← table.utils.ts           │
│              + extract column filters                            │
│                       │                                          │
│              loadTrigger$.next()                                 │
│                       │                                          │
│        ┌──────────────▼──────────────┐                           │
│        │  RxJS Pipeline              │                           │
│        │  debounce → switchMap →     │                           │
│        │  service.getData() →        │                           │
│        │  catchError →               │                           │
│        │  takeUntilDestroyed         │                           │
│        └──────────────┬──────────────┘                           │
│                       │                                          │
│        ┌──────────────▼──────────────┐                           │
│        │  HttpClient GET             │                           │
│        │  ?page=&pageSize=&sort=&... │                           │
│        └──────────────┬──────────────┘                           │
│                       │                                          │
└───────────────────────┼──────────────────────────────────────────┘
                        ▼
┌─────────────────────────────────────────────────────────────────┐
│  Backend Controller → Service → EF Core                         │
│  .Where(filters).Count() → .OrderBy(sort).Skip().Take()        │
│  → PagedResult<T> { Items, TotalCount, Page, PageSize }         │
└─────────────────────────────────────────────────────────────────┘
```

### 6.2 Backend API Contract

Every paginated endpoint accepts these standard query parameters:

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| `page` | int | Yes | 1 | 1-based page number |
| `pageSize` | int | Yes | 25 | Items per page |
| `sortField` | string | No | varies | Column field name to sort by |
| `sortOrder` | string | No | `"desc"` | `"asc"` or `"desc"` |
| `search` | string | No | — | Global text search |
| *(domain filters)* | string | No | — | Column-specific filters (e.g., `category`, `status`) |

**Multi-value filters** are sent as comma-separated strings: `category=Electronics,Clothing,Food`

**Response format:**

```json
{
  "items": [ { /* entity DTO */ }, ... ],
  "totalCount": 12345,
  "page": 1,
  "pageSize": 25,
  "totalPages": 494,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**Backend implementation pattern:**

```csharp
public async Task<PagedResult<EntityDto>> GetEntitiesAsync(
    int page, int pageSize,
    string? sortField, string? sortOrder,
    string? search, string? status, string? category)
{
    var query = _context.Entities.AsNoTracking();

    // Apply filters
    if (!string.IsNullOrEmpty(search))
        query = query.Where(e => e.Name.Contains(search));
    if (!string.IsNullOrEmpty(category))
    {
        var categories = category.Split(',', StringSplitOptions.RemoveEmptyEntries);
        query = query.Where(e => categories.Contains(e.Category));
    }

    // Count before pagination
    var totalCount = await query.CountAsync();

    // Apply sorting
    query = (sortField, sortOrder) switch
    {
        ("name", "asc") => query.OrderBy(e => e.Name),
        ("name", _)     => query.OrderByDescending(e => e.Name),
        ("date", "asc") => query.OrderBy(e => e.CreatedAt),
        _               => query.OrderByDescending(e => e.CreatedAt)
    };

    // Paginate
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(e => new EntityDto(/* map fields */))
        .ToListAsync();

    return new PagedResult<EntityDto>(items, totalCount, page, pageSize);
}
```

### 6.3 Frontend Table Utilities

**`table.utils.ts`** provides two shared helpers:

```typescript
/** Standard query parameters sent to paginated API endpoints. */
export interface ServerQueryParams {
  page: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'asc' | 'desc';
  globalFilter?: string;
}

/** Convert PrimeNG TableLazyLoadEvent → ServerQueryParams */
export function toLazyQueryParams(event: TableLazyLoadEvent): ServerQueryParams {
  const first = event.first ?? 0;
  const rows = event.rows ?? 25;
  return {
    page: Math.floor(first / rows) + 1,
    pageSize: rows,
    sortField: typeof event.sortField === 'string' ? event.sortField : undefined,
    sortOrder: event.sortOrder === 1 ? 'asc' : 'desc',
    globalFilter: typeof event.globalFilter === 'string' && event.globalFilter.length > 0
      ? event.globalFilter : undefined
  };
}

/** Build HttpParams map, omitting undefined values */
export function toHttpParamsMap(params: ServerQueryParams): { [key: string]: string } {
  const map: { [key: string]: string } = {
    page: params.page.toString(),
    pageSize: params.pageSize.toString()
  };
  if (params.sortField) map['sortField'] = params.sortField;
  if (params.sortOrder) map['sortOrder'] = params.sortOrder;
  if (params.globalFilter) map['search'] = params.globalFilter;
  return map;
}
```

### 6.4 Component TypeScript Pattern

Every list component follows this exact reactive pattern:

```typescript
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EntityListComponent implements OnInit {
  @ViewChild('dt') table!: Table;
  private readonly destroyRef = inject(DestroyRef);
  private readonly loadTrigger$ = new Subject<void>();
  private firstLoadDone = false;

  items: Entity[] = [];
  loading = true;
  totalRecords = 0;

  // Current server query state
  private currentPage = 1;
  private currentPageSize = 25;
  private currentSortField?: string;
  private currentSortOrder?: string;
  // ... domain-specific filter fields

  // Column filter options (loaded once on init)
  categoryOptions: { label: string; value: string }[] = [];

  ngOnInit(): void {
    this.loadFilterOptions();  // Load dropdown options

    // Single reactive pipeline — all loads flow through loadTrigger$
    this.loadTrigger$.pipe(
      debounce(() => timer(this.firstLoadDone ? 300 : 50)),  // Fast first load, debounce subsequent
      switchMap(() => {
        this.loading = true;
        this.cdr.markForCheck();

        return this.entityService.getEntities(
          this.currentPage,
          this.currentPageSize,
          this.currentSortField,
          this.currentSortOrder,
          // ... domain filters
        ).pipe(
          catchError((error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: error.error?.error?.message || 'Failed to load data'
            });
            this.loading = false;
            this.cdr.markForCheck();
            return EMPTY;
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(response => {
      this.firstLoadDone = true;
      this.items = response.items || [];
      this.totalRecords = response.totalCount;
      this.loading = false;
      this.cdr.markForCheck();
    });
  }

  /** PrimeNG lazy load handler — called on page/sort/filter changes */
  onLazyLoad(event: TableLazyLoadEvent): void {
    const params = toLazyQueryParams(event);
    this.currentPage = params.page;
    this.currentPageSize = params.pageSize;
    this.currentSortField = params.sortField;
    this.currentSortOrder = params.sortOrder;

    // Extract column filters from PrimeNG event
    const filters = event.filters ?? {};

    // Text filter example:
    const searchMeta = filters['fieldName'];
    const search = Array.isArray(searchMeta) ? searchMeta[0] : searchMeta;
    this.currentSearch = search?.value || undefined;

    // Multi-select filter example (array → comma-separated):
    const catMeta = filters['category'];
    const cat = Array.isArray(catMeta) ? catMeta[0] : catMeta;
    this.currentCategory = cat?.value
      ? (Array.isArray(cat.value) ? cat.value.join(',') : cat.value)
      : undefined;

    // Dropdown filter example:
    const stMeta = filters['status'];
    const st = Array.isArray(stMeta) ? stMeta[0] : stMeta;
    this.currentStatus = st?.value || undefined;

    this.loadTrigger$.next();
  }

  /** Reset all filters and reload */
  clear(table: Table): void {
    this.currentSearch = undefined;
    this.currentCategory = undefined;
    this.currentStatus = undefined;
    this.currentPage = 1;
    this.currentSortField = undefined;
    this.currentSortOrder = undefined;
    table.clear();
  }
}
```

**Key design decisions:**

| Decision | Rationale |
|---|---|
| `Subject<void>` trigger | Decouples load triggers from API calls; any event can fire a reload |
| `debounce(firstLoad ? 300 : 50)` | Fast initial render (50ms), debounced subsequent for typing filters (300ms) |
| `switchMap` | Cancels in-flight requests when new params arrive — prevents stale data |
| `catchError` inside `switchMap` | Prevents error from killing the outer subscription |
| `takeUntilDestroyed` | Auto-cleanup on component destroy — no manual unsubscribe |
| `ChangeDetectionStrategy.OnPush` | Performance — only re-renders on explicit `markForCheck()` |
| Manual filter extraction | Each column filter is mapped to a typed field for the API call |

### 6.5 Component HTML Template

```html
<div class="data-card">
  <p-table
    #dt
    [value]="items"
    [loading]="loading"
    [lazy]="true"
    [totalRecords]="totalRecords"
    (onLazyLoad)="onLazyLoad($event)"
    dataKey="id"
    [paginator]="true"
    paginatorDropdownAppendTo="body"
    [rows]="25"
    [rowsPerPageOptions]="[25, 50, 100]"
    [showCurrentPageReport]="true"
    currentPageReportTemplate="Showing {first} to {last} of {totalRecords} items"
    responsiveLayout="stack"
    [breakpoint]="'768px'"
    styleClass="p-datatable-sm p-datatable-striped p-datatable-gridlines"
    [sortField]="'createdAt'"
    [sortOrder]="-1">

    <!-- Caption: filter reset button -->
    <ng-template #caption>
      <div class="table-header">
        <div class="filter-group">
          <p-button label="Clear" [outlined]="true" icon="pi pi-filter-slash"
            (click)="clear(dt)" />
        </div>
      </div>
    </ng-template>

    <!-- Header row 1: sortable column headers -->
    <ng-template #header>
      <tr>
        <th pSortableColumn="name">Name <p-sortIcon field="name" /></th>
        <th pSortableColumn="code" style="width: 12%">Code <p-sortIcon field="code" /></th>
        <th pSortableColumn="status" style="width: 10%">Status <p-sortIcon field="status" /></th>
        <th pSortableColumn="createdAt" style="width: 14%">Created <p-sortIcon field="createdAt" /></th>
      </tr>
      <!-- Header row 2: column filters -->
      <tr>
        <!-- Text filter: fires on Enter key press -->
        <th>
          <p-columnFilter type="text" field="name" placeholder="Search..."
            [showMenu]="false" matchMode="contains" filterOn="enter" />
        </th>
        <!-- Multi-select filter -->
        <th>
          <p-columnFilter field="category" matchMode="in" [showMenu]="false">
            <ng-template #filter let-value let-filter="filterCallback">
              <p-multiselect [ngModel]="value" [options]="categoryOptions"
                optionLabel="label" optionValue="value"
                (onChange)="filter($event.value)" placeholder="All"
                [filter]="true" filterPlaceholder="Search..."
                appendTo="body" [maxSelectedLabels]="1"
                selectedItemsLabel="{0} selected"
                style="min-width: 10rem; font-size: 0.8125rem" />
            </ng-template>
          </p-columnFilter>
        </th>
        <!-- Dropdown filter -->
        <th>
          <p-columnFilter field="status" matchMode="equals" [showMenu]="false">
            <ng-template #filter let-value let-filter="filterCallback">
              <p-select [ngModel]="value" [options]="statusOptions"
                optionLabel="label" optionValue="value"
                (onChange)="filter($event.value)" placeholder="All"
                [showClear]="true" appendTo="body" style="min-width: 7rem" />
            </ng-template>
          </p-columnFilter>
        </th>
        <!-- Empty filter slot -->
        <th></th>
      </tr>
    </ng-template>

    <!-- Body rows -->
    <ng-template #body let-item>
      <tr>
        <td>
          <span class="p-column-title">Name</span>
          {{ item.name }}
        </td>
        <td>
          <span class="p-column-title">Code</span>
          {{ item.code }}
        </td>
        <td>
          <span class="p-column-title">Status</span>
          <p-tag [value]="item.status" [severity]="getStatusSeverity(item.status)" />
        </td>
        <td>
          <span class="p-column-title">Created</span>
          {{ item.createdAt | date:'MM/dd/yy h:mm a' }}
        </td>
      </tr>
    </ng-template>

    <!-- Empty state -->
    <ng-template #emptymessage>
      <tr>
        <td [attr.colspan]="4" class="empty-message">
          <i class="pi pi-inbox"></i>
          <p>No items found</p>
        </td>
      </tr>
    </ng-template>

    <!-- Footer summary -->
    <ng-template #summary>
      <div class="table-summary">
        <strong>{{ totalRecords | number }}</strong> items total
      </div>
    </ng-template>
  </p-table>
</div>
```

**Template conventions:**

| Element | Convention |
|---|---|
| `<span class="p-column-title">` | Label shown in stacked mobile layout (PrimeNG responsive) |
| `dataKey="id"` | Unique row identifier for selection/tracking |
| `[rows]="25"` | Default page size |
| `[rowsPerPageOptions]="[25, 50, 100]"` | Standard page size options |
| `responsiveLayout="stack"` | Cards layout on mobile (≤768px) |
| `styleClass` | Always includes `p-datatable-sm p-datatable-striped p-datatable-gridlines` |
| `[sortField]` / `[sortOrder]` | Default sort column (typically newest first, `-1`) |
| `filterOn="enter"` | Text filters fire on Enter key only (prevents per-keystroke API calls) |
| `appendTo="body"` | Dropdowns/multiselects render in body to avoid overflow clipping |

### 6.6 Column Filter Types

| Type | PrimeNG Component | Match Mode | Fires On |
|---|---|---|---|
| **Text search** | `p-columnFilter type="text"` | `contains` or `startsWith` | `filterOn="enter"` |
| **Multi-select** | `p-multiselect` inside `p-columnFilter` | `in` | `onChange` |
| **Single dropdown** | `p-select` inside `p-columnFilter` | `equals` | `onChange` |
| **Date range** | `p-datePicker selectionMode="range"` inside `p-columnFilter` | `between` | `onChange` |

**Multi-select filter extraction** (frontend → API):

```
PrimeNG value: ['Electronics', 'Clothing', 'Food']  →  API param: category=Electronics,Clothing,Food
```

Backend splits with: `category.Split(',', StringSplitOptions.RemoveEmptyEntries)`

### 6.7 Global Table Styles

Applied in `styles.scss` to ensure consistency across all tables:

```scss
// 1px gridlines on all cells
.p-datatable .p-datatable-thead > tr > th,
.p-datatable .p-datatable-tbody > tr > td {
  border: 1px solid #e2e8f0 !important;
}

// 2px bottom border on header for visual separation
.p-datatable .p-datatable-thead > tr > th {
  border-bottom: 2px solid #cbd5e1 !important;
}

// Standard body font-size
.p-datatable .p-datatable-tbody > tr > td {
  font-size: 0.875rem;
}

// Column filter font-size (slightly smaller than body)
.p-datatable p-columnfilterformelement {
  .p-inputtext { font-size: 0.8125rem; }
  .p-select-label { font-size: 0.8125rem; }
}

// Mobile: remove cell borders, show stacked layout
@media screen and (max-width: 768px) {
  .p-datatable .p-datatable-tbody > tr > td {
    border: none !important;
    border-bottom: 1px solid #e2e8f0 !important;
    &:last-child { border-bottom: none !important; }
  }
}

// Empty message pattern
.empty-message {
  text-align: center;
  padding: 3rem 1rem !important;
  color: #94a3b8;

  i {
    font-size: 3rem;
    color: #cbd5e1;
    margin-bottom: 1rem;
    display: block;
  }

  p {
    color: #94a3b8;
    margin: 0;
    font-size: 1rem;
  }
}
```

### 6.8 Service Layer Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class EntityService {
  private apiUrl = `${environment.apiUrl}/entities`;

  constructor(private http: HttpClient) {}

  getEntities(
    page: number, pageSize: number,
    sortField?: string, sortOrder?: string,
    search?: string, category?: string, status?: string
  ): Observable<PagedResult<Entity>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);
    if (sortField) params = params.set('sortField', sortField);
    if (sortOrder) params = params.set('sortOrder', sortOrder);
    if (search) params = params.set('search', search);
    if (category) params = params.set('category', category);
    if (status) params = params.set('status', status);

    return this.http.get<PagedResult<Entity>>(this.apiUrl, { params });
  }
}
```

### 6.9 Checklist for Adding a New Table

| # | Step | Layer |
|---|---|---|
| 1 | Define API endpoint accepting `page`, `pageSize`, `sortField`, `sortOrder`, and domain filters | Backend |
| 2 | Return `PagedResult<T>` with `Items`, `TotalCount`, `Page`, `PageSize` | Backend |
| 3 | Create Angular service method returning `Observable<PagedResult<T>>` | Frontend |
| 4 | Create component with `Subject<void>` + `debounce` + `switchMap` + `takeUntilDestroyed` pipeline | Frontend |
| 5 | Implement `onLazyLoad(event)` using `toLazyQueryParams()` + manual filter extraction | Frontend |
| 6 | Build `<p-table [lazy]="true">` template with header filters, body, empty message, summary | Frontend |
| 7 | Load dropdown/multiselect filter options in `ngOnInit()` | Frontend |
| 8 | Add `ChangeDetectionStrategy.OnPush` and call `cdr.markForCheck()` after data updates | Frontend |
| 9 | Add responsive `<span class="p-column-title">` labels in body cells | Frontend |
| 10 | Test: pagination, sorting, each filter independently, clear filters, empty state, error state | Both |

---

## 7. Development Setup

### Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | 10.0+ |
| Node.js | 20.x+ |
| npm | 10.x+ |
| SQL Server LocalDB | Latest |
| VS Code or Visual Studio | Latest |

### Backend Setup

```bash
cd {AppName}.Backend

# Restore packages
dotnet restore

# Run (auto-migrates and seeds on first start)
dotnet run --project src/{App}.Api

# API available at https://localhost:{PORT}
# Swagger at https://localhost:{PORT}/swagger
```

### Frontend Setup

```bash
cd {AppName}.Frontend

# Install dependencies
npm install

# Start dev server with HMR
npm start

# App available at http://localhost:4200
```

### Default Credentials

| Username | Password | Role |
|---|---|---|
| `admin` | `Admin@123` | Admin |

---

## 8. Folder Structure Reference

```
{AppName}/
├── {AppName}.Backend/
│   ├── {AppName}.slnx
│   ├── Directory.Build.props
│   ├── seed_data.sql
│   └── src/
│       ├── {App}.Api/
│       │   ├── {App}.Api.csproj
│       │   ├── Program.cs
│       │   ├── Dockerfile
│       │   ├── appsettings.json
│       │   ├── appsettings.Development.json
│       │   ├── Properties/
│       │   │   └── launchSettings.json
│       │   └── Controllers/
│       │       ├── AppBaseController.cs
│       │       ├── AuthController.cs
│       │       ├── ProductsController.cs
│       │       ├── OrdersController.cs
│       │       ├── CustomersController.cs
│       │       ├── DashboardController.cs
│       │       ├── ReportsController.cs
│       │       ├── AuditController.cs
│       │       ├── UsersController.cs
│       │       ├── SettingsController.cs
│       │       ├── ProfileController.cs
│       │       └── {Feature}Controller.cs
│       ├── {App}.Application/
│       │   ├── {App}.Application.csproj
│       │   ├── DTOs/
│       │   │   ├── Auth/AuthDtos.cs
│       │   │   ├── Common/CommonDtos.cs
│       │   │   ├── Customers/CustomerDtos.cs
│       │   │   ├── Dashboard/DashboardDtos.cs
│       │   │   ├── Orders/OrderDtos.cs
│       │   │   ├── Products/ProductDtos.cs
│       │   │   ├── Reports/ReportDtos.cs
│       │   │   ├── Settings/SettingsDtos.cs
│       │   │   ├── Users/UserDtos.cs
│       │   │   └── {Feature}/{Feature}Dtos.cs
│       │   └── Interfaces/
│       │       ├── IAuthService.cs
│       │       ├── IUserService.cs
│       │       ├── IProductService.cs
│       │       ├── IOrderService.cs
│       │       ├── IOrderQueryService.cs
│       │       ├── IOrderCommandService.cs
│       │       ├── ICustomerService.cs
│       │       ├── IAuditService.cs
│       │       ├── IAuditLogger.cs
│       │       ├── IAuditReader.cs
│       │       ├── IDashboardService.cs
│       │       ├── IReportService.cs
│       │       ├── IProfileService.cs
│       │       ├── ISystemSettingsService.cs
│       │       └── I{Feature}Service.cs
│       ├── {App}.Domain/
│       │   ├── {App}.Domain.csproj
│       │   ├── StatusConstants.cs
│       │   └── Entities/
│       │       ├── AuditLog.cs
│       │       ├── Category.cs
│       │       ├── Customer.cs
│       │       ├── Order.cs
│       │       ├── OrderItem.cs
│       │       ├── Product.cs
│       │       ├── RefreshToken.cs
│       │       ├── Role.cs
│       │       ├── Status.cs
│       │       ├── SystemSettings.cs
│       │       ├── User.cs
│       │       └── {Entity}.cs
│       └── {App}.Infrastructure/
│           ├── {App}.Infrastructure.csproj
│           ├── DependencyInjection.cs
│           ├── Data/
│           │   ├── AppDbContext.cs
│           │   └── Configurations/
│           │       ├── AuditLogConfiguration.cs
│           │       ├── CategoryConfiguration.cs
│           │       ├── CustomerConfiguration.cs
│           │       ├── OrderConfiguration.cs
│           │       ├── OrderItemConfiguration.cs
│           │       ├── ProductConfiguration.cs
│           │       ├── RefreshTokenConfiguration.cs
│           │       ├── RoleConfiguration.cs
│           │       ├── StatusConfiguration.cs
│           │       ├── SystemSettingsConfiguration.cs
│           │       ├── UserConfiguration.cs
│           │       └── {Entity}Configuration.cs
│           ├── Migrations/
│           │   └── (migration files)
│           └── Services/
│               ├── AuthService.cs
│               ├── AuditService.cs
│               ├── CustomerService.cs
│               ├── DashboardService.cs
│               ├── DatabaseSeeder.cs
│               ├── OrderService.cs
│               ├── ProductService.cs
│               ├── ProfileService.cs
│               ├── ReportService.cs
│               ├── SystemSettingsService.cs
│               ├── UserService.cs
│               └── {Feature}Service.cs
├── {AppName}.Frontend/
│   ├── package.json
│   ├── angular.json
│   ├── tsconfig.json
│   ├── tsconfig.app.json
│   ├── tsconfig.spec.json
│   ├── public/
│   │   └── fonts/
│   └── src/
│       ├── index.html
│       ├── main.ts                    # Client bootstrap (NgModule-based)
│       ├── main.server.ts             # SSR bootstrap
│       ├── server.ts                  # Express SSR server
│       ├── styles.scss                # Global styles
│       ├── primeicons-local.css       # Local PrimeIcons
│       ├── environments/
│       │   ├── environment.ts         # Dev config (apiUrl, etc.)
│       │   └── environment.prod.ts    # Prod config
│       └── app/
│           ├── app-module.ts          # Root NgModule
│           ├── app-routing-module.ts  # Root routing (lazy loads)
│           ├── app.ts                 # Root component
│           ├── app.html
│           ├── app.scss
│           ├── app.module.server.ts   # SSR module
│           ├── app.routes.server.ts   # SSR routes (all client-rendered)
│           ├── core/
│           │   ├── core.module.ts
│           │   ├── guards/
│           │   │   └── auth.guard.ts
│           │   ├── interceptors/
│           │   │   └── auth.interceptor.ts
│           │   ├── models/
│           │   │   ├── index.ts
│           │   │   └── (model files — one per domain concept)
│           │   └── services/
│           │       ├── index.ts
│           │       └── (service files — one per domain concept)
│           ├── shared/
│           │   ├── shared.module.ts
│           │   ├── index.ts
│           │   ├── components/
│           │   │   └── (reusable UI components)
│           │   ├── pipes/
│           │   │   └── (custom pipes)
│           │   └── utils/
│           │       ├── date.utils.ts
│           │       ├── export.utils.ts
│           │       └── table.utils.ts
│           ├── layout/
│           │   ├── layout.module.ts
│           │   ├── main-layout/
│           │   ├── header/
│           │   ├── sidebar/
│           │   └── footer/
│           └── features/
│               ├── auth/
│               ├── dashboard/
│               ├── products/
│               ├── orders/
│               ├── customers/
│               ├── reports/
│               ├── audit/
│               ├── admin/
│               ├── profile/
│               └── {feature}/        ← Add more as needed
```

---

> **Getting Started:** When building a new app from this template, start with the Domain entities, work outward through Application interfaces/DTOs, then Infrastructure services/DbContext, and finally the API controllers. On the frontend, scaffold the Core/Shared/Layout modules first, then add feature modules one at a time. Replace all `{AppName}`, `{App}`, and `{appname}` placeholders with your actual project name.
