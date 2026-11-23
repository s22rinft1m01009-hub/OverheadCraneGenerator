# Parametric Overhead Crane Generator — Backend

## Backend overview

The backend is the .NET 10 ASP.NET Core API in `OverheadCraneGenerator`. It stores crane design input in SQLite, validates create/update requests, calculates a small set of drawing-oriented derived values, builds crane-specific `DrawingModel` geometry, and can return drawing JSON or SVG.

This is a conceptual parametric drawing generator. It is not a structural analysis, engineering approval, or fabrication system.

## Technology stack

| Technology | Actual backend use |
|---|---|
| .NET 10 / ASP.NET Core | HTTP API, controllers, dependency injection, middleware, OpenAPI mapping |
| Entity Framework Core 10.0.15 | ORM and migrations |
| SQLite (`Microsoft.EntityFrameworkCore.Sqlite`) | Persisted `CraneDesign` records in `crane-generator.db` |
| System.Text.Json | HTTP JSON serialization; `DrawingElement` uses polymorphism metadata |
| OpenAPI / Scalar | OpenAPI mapping in Development and Scalar API reference |
| C# | Domain, application, repository, API, and rendering code |

Project files also declare AutoMapper, Mapster, and Swashbuckle packages. The current design service maps entities to response DTOs manually; the API uses `AddOpenApi` and Scalar. No PDF library/package is present.

## Backend architecture

```text
HTTP request
	-> OverheadCraneGenerator.Controllers
	-> CraneGenerator.Application.Services
	-> CraneGenerator.Infrastructure.Repositories
	-> CraneDbContext (EF Core)
	-> SQLite
```

Drawing requests use `CraneDrawingService -> CraneDrawingFactory -> ICraneCalculationService + selected ICraneGeometryGenerator -> DrawingModel`. `SvgDrawingRenderer` consumes that model to produce SVG. PDF rendering is not implemented.

## Project structure

- `OverheadCraneGenerator` — ASP.NET Core host, controllers, middleware, configuration, and API entry point.
- `CraneGenerator.Application` — design/drawing services, request/response DTOs, validation, calculations, crane generators, and SVG renderer.
- `CraneGenerator.Domain` — `CraneDesign`, drawing model types, and crane type enum/normalizer.
- `CraneGenerator.Infrastructure` — `CraneDbContext`, `DesignRepository`, repository interface, and EF Core migration files.
- `CraneGenerator.Tests` — currently contains two empty test class shells; executable tests are not configured.

## API and controllers

`DesignsController` provides design CRUD, drawing-model generation, and SVG generation. Its collection route is `GET /api/designs/getall` (not `GET /api/designs`). `HealthController` is available at both `/health` and `/api/health`. A template `WeatherForecastController` also exposes `GET /WeatherForecast`. `DrawingsController` exists but contains no actions.

Successful CRUD results use the `ApiResponse<T>` envelope (`success`, `statusCode`, `message`, `data`). Generated drawing JSON is returned directly. SVG is returned as `image/svg+xml`.

See [BACKEND_API.md](BACKEND_API.md) for routes, examples, and response behavior.

## Services and repositories

- `DesignService` performs CRUD orchestration, invokes `DesignValidator` on create/update, normalizes crane type values, and maps stored entities to `DesignResponse`.
- `CraneDrawingService` delegates drawing generation to `CraneDrawingFactory`.
- `CraneCalculationService` returns deterministic drawing-oriented derived values such as hoist ratio, drawing envelope/rope/runway dimensions, and a scale string.
- `DesignRepository` performs EF Core reads/writes through `CraneDbContext`.

Services and generators are registered as scoped dependencies in `OverheadCraneGenerator/Program.cs`.

## Domain and supported crane types

`CraneDesign` stores the input values and timestamps. Its `CraneType` field remains a string for database/API compatibility. `CraneTypeNormalizer` canonicalizes supported types and legacy values:

- `SINGLE_GIRDER_EOT` (also legacy `EOT Crane`, `EOT`, and `SINGLE GIRDER EOT`)
- `DOUBLE_GIRDER_EOT` (also `DOUBLE GIRDER EOT`)
- `GANTRY` (also `GANTRY CRANE`)

The implemented geometry classes are `SingleGirderEotGenerator`, `DoubleGirderEotGenerator`, and `GantryGenerator`. Other crane types are not implemented.

## SQLite database and migrations

`OverheadCraneGenerator/appsettings.json` defines:

```json
"DefaultConnection": "Data Source=crane-generator.db"
```

The API configures SQLite in `Program.cs`. One migration exists: `20261122141611_InitialCreate`. The API does not automatically call `Database.Migrate()` at startup. See [BACKEND_DATABASE.md](BACKEND_DATABASE.md) for schema and migration commands.

## Validation and error handling

`DesignValidator` is used by `DesignService` create/update operations. It validates non-empty names (maximum 200 characters), supported crane type, finite numeric values, positive capacity/span/lift/building height, non-negative runway/hoist values, positive runway for EOT types, and hoist position no greater than span. Building height must be positive for every type, including Gantry.

`ExceptionHandlingMiddleware` maps `DesignValidationException` and `ArgumentException` to 400 ProblemDetails responses and unexpected exceptions to a safe generic 500 response. Missing designs return 404 from controller actions. No 409 conflict handling is implemented.

## Drawing generation and SVG

`DrawingModel` contains drawing number, title, scale, views, polymorphic elements, dimensions, and technical data. Each generator creates `MAIN ELEVATION`, `PLAN VIEW`, and `SECTION A-A` with model-specific geometry. Labels use design input values. Hoist location and several drawing dimensions use values from `CraneCalculationService`.

`SvgDrawingRenderer` renders lines, rectangles, circles, dimensions, view borders, technical data, and a title block. The title block includes the conceptual notice `CONCEPTUAL DRAWING - NOT FOR FABRICATION`. The renderer operates on `DrawingModel`; it does not select crane geometry. SVG is available through `GET /api/designs/{id}/drawing/svg`.

A PDF renderer, PDF package, and PDF API endpoint are not present.

## Running locally

From the repository root in PowerShell:

```powershell
dotnet restore .\OverheadCraneGenerator.slnx
dotnet build .\OverheadCraneGenerator.slnx
dotnet run --project .\OverheadCraneGenerator\OverheadCraneGenerator.csproj
```

The launch profiles use HTTP `http://localhost:5041` and HTTPS `https://localhost:7025` / `http://localhost:5041`; the app redirects HTTP to HTTPS when that profile is configured accordingly. In Development, OpenAPI and Scalar are mapped.

## Database setup

The checked-in initial migration can be applied with EF tooling (ensure `dotnet-ef` is installed):

```powershell
dotnet ef database update --project .\CraneGenerator.Infrastructure\CraneGenerator.Infrastructure.csproj --startup-project .\OverheadCraneGenerator\OverheadCraneGenerator.csproj
```

The relative SQLite file path is resolved from the API host's working/content directory. The API does not automatically apply migrations at startup. Preserve the existing database file when updating the project.

## Tests

`CraneGenerator.Tests` has `CraneCalculationTests` and `DesignValidationTests` files, but both classes are empty. Its project file does not configure a test SDK or test framework. No automated unit/API/persistence/drawing/PDF tests are currently implemented. `dotnet test .\OverheadCraneGenerator.slnx` is the command to try after test infrastructure is added; the current repository does not provide meaningful test discovery or coverage.

## Example API workflow

Create a design (the current controller returns HTTP 200 in the standard success envelope):

```powershell
$body = @{
  designName = 'Workshop Crane 01'
  craneType = 'SINGLE_GIRDER_EOT'
  capacityTonnes = 10
  spanMm = 25000
  liftHeightMm = 8000
  runwayWidthMm = 12500
  hoistPositionMm = 12500
  buildingHeightMm = 9000
} | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5041/api/designs -ContentType 'application/json' -Body $body
```

Use the returned `data.id` for:

```text
GET  /api/designs/{id}
POST /api/designs/{id}/generate
GET  /api/designs/{id}/drawing/svg
```

The exact CRUD paths and response shapes are in [BACKEND_API.md](BACKEND_API.md). There is no PDF download route.

## Known backend limitations

- PDF export is absent.
- Automated tests are absent; current test classes are empty and test infrastructure is not configured.
- Only single-girder EOT, double-girder EOT, and gantry geometry are implemented.
- Drawing calculations use prototype display-space rules; scale/geometry are conceptual, not certified dimensional engineering.
- Capacity is validated and displayed as technical data; it is not used for structural sizing.
- The current `DesignValidator` requires `BuildingHeightMm > 0` for Gantry, although the geometry itself does not use this field for gantry elevation.
- CRUD collection retrieval uses `/api/designs/getall`, not `/api/designs`.
- There is no conflict/409 policy, authentication setup, or explicit database migration-on-startup behavior.

## Engineering scope and honesty

The backend stores source design inputs, calculates drawing placement/display values, and generates conceptual geometry. It does not calculate structural strength, deflection, wheel loads, motor/gearbox/brake sizing, weld design, fatigue, building adequacy, or fabrication approval. It does not provide engineering certification. See [ENGINEERING_NOTES.md](ENGINEERING_NOTES.md).
