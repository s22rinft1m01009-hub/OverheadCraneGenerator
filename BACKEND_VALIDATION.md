# Backend Validation

Validation is implemented in `CraneGenerator.Application.Validation.DesignValidator`. `DesignService.CreateAsync` and `UpdateAsync` call `ValidateAndNormalize` before reading/writing the design. Failures are collected into `DesignValidationException`; `ExceptionHandlingMiddleware` maps that exception to HTTP 400.

## Rules implemented

| Field/rule | Current behavior | Implementation |
|---|---|---|
| `DesignName` | Required; whitespace-only rejected; trimmed length must be at most 200 characters | `DesignValidator` |
| `CraneType` | Must normalize to one of `SINGLE_GIRDER_EOT`, `DOUBLE_GIRDER_EOT`, or `GANTRY`; recognized legacy aliases are normalized | `DesignValidator` and `CraneTypeNormalizer` |
| `CapacityTonnes` | Finite and greater than zero | `RequireFinitePositive` |
| `SpanMm` | Finite and greater than zero | `RequireFinitePositive` |
| `LiftHeightMm` | Finite and greater than zero | `RequireFinitePositive` |
| `RunwayWidthMm` | Finite and greater than or equal to zero; must be strictly greater than zero for single-/double-girder EOT | `RequireFiniteNonNegative` and crane-type conditional |
| `HoistPositionMm` | Finite and greater than or equal to zero | `RequireFiniteNonNegative` |
| Hoist/span relation | If span is positive, hoist position must not exceed span | Cross-field check in `DesignValidator` |
| `BuildingHeightMm` | Finite and strictly greater than zero for all supported types | `RequireFinitePositive` |

Although Gantry geometry does not use building height and zero runway is accepted for Gantry, the current validator still requires positive building height for Gantry. There is no upper bound for numeric inputs, no capacity/span engineering range, and no maximum request-specific numeric value beyond finite-double checks.

The `CraneDbContext` requires fields and limits name/type lengths at the persistence mapping level, but numeric/business constraints are not database check constraints.

## Crane type normalization

`CraneTypeNormalizer.Normalize` trims and uppercases input. It accepts:

- `SINGLE_GIRDER_EOT`, `SINGLE GIRDER EOT`, `EOT CRANE`, `EOT` → `SINGLE_GIRDER_EOT`
- `DOUBLE_GIRDER_EOT`, `DOUBLE GIRDER EOT` → `DOUBLE_GIRDER_EOT`
- `GANTRY`, `GANTRY CRANE` → `GANTRY`

Other values produce an `ArgumentException` in the normalizer; `DesignValidator` converts unsupported type into a validation error before create/update.

## HTTP behavior

- **400 Bad Request:** `DesignValidationException` and `ArgumentException` are converted by `ExceptionHandlingMiddleware` to `application/problem+json`. Validation details are included in an `errors` extension. `[ApiController]` can also return framework-generated 400 responses for request binding/model-state failures before the service is invoked.
- **404 Not Found:** Controller actions return 404 when a requested design ID is not found. Some responses have no body, while update/delete include a plain message string.
- **409 Conflict:** Not implemented. No uniqueness/conflict rule or conflict middleware/action response is present.
- **500 Internal Server Error:** Unexpected exceptions are logged server-side and returned as generic ProblemDetails; stack traces are not sent to clients.

Validation is applied to create/update service operations. A design loaded from the database for generation is not revalidated by `DesignValidator` at read time.
