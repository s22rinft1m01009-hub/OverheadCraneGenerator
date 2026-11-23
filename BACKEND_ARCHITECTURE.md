# Backend Architecture

## Projects and responsibilities

- **`OverheadCraneGenerator`** — ASP.NET Core host, `Program.cs`, controllers, exception middleware, JSON/API configuration, and appsettings.
- **`CraneGenerator.Application`** — `DesignService`, design/drawing interfaces and DTOs, `DesignValidator`, calculation result/service, crane model factory/generators, and `SvgDrawingRenderer`.
- **`CraneGenerator.Domain`** — `CraneDesign`, `CraneType` and `CraneTypeNormalizer`, plus drawing model/entity types.
- **`CraneGenerator.Infrastructure`** — EF Core `CraneDbContext`, `IDesignRepository`/`DesignRepository`, and migrations.
- **`CraneGenerator.Tests`** — empty test class shells and no configured test framework at present.

## Request-to-persistence flow

```text
HTTP Request
	↓
DesignsController
	↓
IDesignService / DesignService
	↓
IDesignRepository / DesignRepository
	↓
CraneDbContext (EF Core)
	↓
SQLite
```

`DesignService` validates requests with `DesignValidator`, normalizes the crane type, creates/updates `CraneDesign`, and maps entities to `DesignResponse`. The repository performs asynchronous EF Core operations. Controllers handle route binding and translate missing records to 404.

## Drawing flow

```text
Saved CraneDesign -> DesignResponse
	↓
CraneDrawingService
	↓
CraneDrawingFactory -> ICraneCalculationService
	↓
CraneCalculationService -> CraneCalculationResult
	↓
CraneDrawingFactory selects ICraneGeometryGenerator
	↓
DrawingModel
	↓
SvgDrawingRenderer (SVG only)
```

`CraneDrawingFactory` normalizes/parses the design type and selects one of `SingleGirderEotGenerator`, `DoubleGirderEotGenerator`, or `GantryGenerator`. `CraneGeometryGeneratorBase` shares drawing setup, technical data, common dimensions, and hook/ground helpers. Each concrete generator defines elevation, plan, and section geometry. There is no PDF renderer or PDF flow.

## Dependency direction and project references

`OverheadCraneGenerator` references Application, Domain, Infrastructure, and currently also `CraneGenerator.Tests`. `CraneGenerator.Application` references Domain and Infrastructure in its project file. Infrastructure references Domain. The project references are documented as currently configured; they are not a claim of strict clean-architecture dependency inversion.

## Important types

### API and DTOs

- `DesignsController` exposes design CRUD, drawing-model generation, and SVG.
- `HealthController` exposes `/health` and `/api/health`.
- `WeatherForecastController` is a template endpoint.
- `CreateDesignRequest`, `UpdateDesignRequest`, and `DesignResponse` define design transport values.
- `AppBaseController` defines `ApiResponse<T>` and `ErrorResponse<T>` for envelope responses. Drawing and health actions return direct action results.

### Application services

- `IDesignService` / `DesignService` — async design CRUD, validation, normalization, entity/response mapping.
- `ICraneDrawingService` / `CraneDrawingService` — drawing-generation façade.
- `ICraneCalculationService` / `CraneCalculationService` — synchronous deterministic display-derived values.
- `CraneDrawingFactory` — selects a geometry generator.
- `SvgDrawingRenderer` — serializes a `DrawingModel` to SVG.

### Domain model

`CraneDesign` holds persisted inputs and timestamps. `DrawingModel`, `DrawingView`, `DrawingElement`, `DrawingLine`, `DrawingRectangle`, `DrawingCircle`, `DrawingDimension`, and `DrawingTechnicalData` define drawing data. `DrawingElement` uses System.Text.Json polymorphism attributes for its three derived element types.

### Persistence

`IDesignRepository` declares read/create/update/delete/existence operations. `DesignRepository` implements them with `CraneDbContext`. `CraneDbContext` maps the `CraneDesigns` table and required/max-length properties.

## Dependency injection

`Program.cs` uses scoped registrations for the repository, design services, calculation service, drawing service, all three geometry generators, factory, and SVG renderer. EF Core registers `CraneDbContext` using the `DefaultConnection` SQLite string.

## Middleware and errors

`ExceptionHandlingMiddleware` handles `DesignValidationException` and `ArgumentException` as 400 ProblemDetails, logs unexpected exceptions, and returns a generic 500 response without stack details. It is installed before controller mapping. Missing IDs are handled in controller actions as 404. No 409 conflict handler exists.

## Validation

`DesignValidator` validates create/update DTO values and returns a normalized type string. `DesignService` calls it before persistence. `[ApiController]` provides framework model-binding errors for invalid bodies. See [BACKEND_VALIDATION.md](BACKEND_VALIDATION.md) for precise rules.

## Serialization and response shapes

`DrawingElement` declares JSON discriminator values `line`, `rectangle`, and `circle`, retaining derived coordinates/properties when an element is serialized through `List<DrawingElement>`. Normal successful CRUD actions return an `ApiResponse<T>` JSON object. The generated drawing action returns `DrawingModel` directly; SVG returns `image/svg+xml`.

## Documentation boundaries

No authentication handler is configured despite an OpenAPI Bearer scheme declaration. No PDF rendering, endpoint, or package exists. No automated test framework or substantive tests exist. No migration application is triggered automatically in `Program.cs`.
