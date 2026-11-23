# Backend Database

## Provider and connection string

The API uses EF Core SQLite. In `OverheadCraneGenerator/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=crane-generator.db"
}
```

`OverheadCraneGenerator/Program.cs` reads this key and calls `UseSqlite`. The relative file path is resolved in the host's runtime/content-root context. The repository currently contains `OverheadCraneGenerator/crane-generator.db`; preserve it when changing or applying migrations.

## DbContext and entity mapping

`CraneGenerator.Infrastructure.Data.CraneDbContext` exposes `DbSet<CraneDesign> CraneDesigns` and maps the `CraneDesigns` table.

The `CraneDesign` columns are:

| Field | SQLite type | Meaning/configuration |
|---|---|---|
| `Id` | INTEGER | Primary key; SQLite auto-increment in initial migration |
| `DesignName` | TEXT | Required, maximum 200 characters |
| `CraneType` | TEXT | Required string, maximum 50 characters |
| `CapacityTonnes` | REAL | Required crane capacity input in tonnes |
| `SpanMm` | REAL | Required span input in millimetres |
| `LiftHeightMm` | REAL | Required lift-height input in millimetres |
| `RunwayWidthMm` | REAL | Required runway-width input in millimetres |
| `HoistPositionMm` | REAL | Required hoist position input in millimetres |
| `BuildingHeightMm` | REAL | Required building-height input in millimetres |
| `CreatedAt` | TEXT | Required creation timestamp |
| `UpdatedAt` | TEXT | Required update timestamp |

`CraneType` is stored as text rather than as the enum's integer representation. `DesignService` writes normalized values for new/updated designs and normalizes values when mapping saved entities to responses. Supported canonical values are `SINGLE_GIRDER_EOT`, `DOUBLE_GIRDER_EOT`, and `GANTRY`; recognized legacy values are mapped by `CraneTypeNormalizer`.

No relationships, unique constraints, indexes, or database-level numeric check constraints are defined in the current mapping/migration. Numeric validation is in the application layer.

## Repository and persistence behavior

`IDesignRepository` declares `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, and `ExistsAsync`. `DesignRepository` uses EF Core queries and `SaveChangesAsync` for writes. `GetAllAsync` orders by `CreatedAt` descending. `ExistsAsync` is implemented but is not used by current `DesignService` methods.

`DesignService` persists only input parameters and timestamps. Generated `DrawingModel`, SVG, and PDF files are not stored as source-of-truth data. Drawings are generated from saved design values on request.

## Migrations and database creation

The infrastructure project contains one migration, `20261122141611_InitialCreate`, plus its designer and `CraneDbContextModelSnapshot`. The migration creates `CraneDesigns`; its `Down` operation drops that table. There are no later migrations in the current project.

`Program.cs` does not call `Database.Migrate()` or `EnsureCreated()` during startup. To apply the checked-in migration (requires EF CLI tooling and a valid startup configuration), run from the repository root:

```powershell
dotnet ef database update --project .\CraneGenerator.Infrastructure\CraneGenerator.Infrastructure.csproj --startup-project .\OverheadCraneGenerator\OverheadCraneGenerator.csproj
```

Before applying schema changes to a database with valuable records, make a backup. Do not delete the existing database to resolve migration problems.

## Persistence across restarts

SQLite writes are committed through EF Core `SaveChangesAsync`; the file database is intended to retain records across API restarts. This behavior follows the configured file-backed connection string. The documentation does not claim a restart/persistence test was run; automated persistence tests are not implemented.
