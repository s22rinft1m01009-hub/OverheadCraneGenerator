# Backend API

The API is hosted by `OverheadCraneGenerator`. The routes below are taken from controller attributes in the current source. No authentication handler/configuration is registered; `Program.cs` declares an OpenAPI Bearer scheme but does not configure JWT authentication.

## Design endpoints

### Create design

- **Method/route:** `POST /api/designs`
- **Purpose:** Validate and persist a new `CraneDesign`.
- **Path/query parameters:** None.
- **Request body:** `CreateDesignRequest` JSON.
- **Success:** HTTP 200; `ApiResponse<DesignResponse>` envelope, with new record under `data`.
- **Validation:** See [BACKEND_VALIDATION.md](BACKEND_VALIDATION.md).
- **Errors:** HTTP 400 for request validation or model-binding errors; HTTP 500 for unexpected exceptions. No duplicate/409 policy exists.

Example request:

```json
{
  "designName": "Workshop Crane 01",
  "craneType": "SINGLE_GIRDER_EOT",
  "capacityTonnes": 10,
  "spanMm": 25000,
  "liftHeightMm": 8000,
  "runwayWidthMm": 12500,
  "hoistPositionMm": 12500,
  "buildingHeightMm": 9000
}
```

Example success body (timestamps/ID are generated at runtime):

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Data Add succesfully",
  "data": {
	"id": 1,
	"designName": "Workshop Crane 01",
	"craneType": "SINGLE_GIRDER_EOT",
	"capacityTonnes": 10,
	"spanMm": 25000,
	"liftHeightMm": 8000,
	"runwayWidthMm": 12500,
	"hoistPositionMm": 12500,
	"buildingHeightMm": 9000,
	"createdAt": "<UTC timestamp>",
	"updatedAt": "<UTC timestamp>"
  }
}
```

The controller currently returns HTTP 200, not HTTP 201.

### List designs

- **Method/route:** `GET /api/designs/getall`
- **Purpose:** Retrieve all designs, ordered newest-created first by `DesignRepository`.
- **Request/path/query:** None.
- **Success:** HTTP 200 `ApiResponse<List<DesignResponse>>`.
- **Errors:** Unexpected errors are handled as HTTP 500.

Example:

```http
GET /api/designs/getall
```

A collection route `GET /api/designs` is **not** implemented.

### Get design by ID

- **Method/route:** `GET /api/designs/{id:int}`
- **Purpose:** Retrieve a single design.
- **Path parameter:** `id` (integer).
- **Request/query:** No body or query parameters.
- **Success:** HTTP 200 `ApiResponse<DesignResponse>`.
- **Errors:** HTTP 404 if no design exists; HTTP 500 for unexpected errors.

Example:

```http
GET /api/designs/2
```

### Update design

- **Method/route:** `PUT /api/designs/{id:int}`
- **Purpose:** Validate and replace the saved design input fields.
- **Path parameter:** `id` (integer).
- **Request body:** `UpdateDesignRequest`, with the same design fields as create.
- **Success:** HTTP 200 `ApiResponse<bool>` with `data: true`.
- **Validation:** Same rules as create.
- **Errors:** HTTP 404 when the ID is absent; HTTP 400 for validation/model-binding errors; HTTP 500 for unexpected errors. No 409 behavior is implemented.

Example:

```http
PUT /api/designs/2
Content-Type: application/json
```

Use the create request fields as the complete update payload.

### Delete design

- **Method/route:** `DELETE /api/designs/{id:int}`
- **Purpose:** Delete a saved design.
- **Path parameter:** `id` (integer).
- **Request/query:** No body or query parameters.
- **Success:** HTTP 200 `ApiResponse<string>` with empty data.
- **Errors:** HTTP 404 when the ID is absent; HTTP 500 for unexpected errors.

Example:

```http
DELETE /api/designs/2
```

## Drawing endpoints

### Generate drawing model

- **Method/route:** `POST /api/designs/{id:int}/generate`
- **Purpose:** Load a saved design and generate a crane-specific `DrawingModel`.
- **Path parameter:** `id` (integer).
- **Request/query:** No body or query parameters.
- **Success:** HTTP 200 with the drawing model directly (not the CRUD envelope).
- **Errors:** HTTP 404 if the design is absent; HTTP 500 on unexpected generation errors.

Example:

```http
POST /api/designs/2/generate
```

The `Elements` list includes the `$type` discriminator and derived line/rectangle/circle properties through `DrawingElement` JSON polymorphism metadata.

### Generate SVG

- **Method/route:** `GET /api/designs/{id:int}/drawing/svg`
- **Purpose:** Load a saved design, generate the same drawing model used by the generation endpoint, render it, and return SVG.
- **Path parameter:** `id` (integer).
- **Request/query:** No body or query parameters.
- **Success:** HTTP 200, content type `image/svg+xml`.
- **Errors:** HTTP 404 if the design is absent; HTTP 500 for unexpected errors.

Example:

```http
GET /api/designs/2/drawing/svg
```

### PDF drawing

**Not implemented.** There is no PDF renderer, PDF package, or `/api/designs/{id}/drawing/pdf` action in the current backend.

## Health and template endpoint

### Health

- **Method/routes:** `GET /health` and `GET /api/health`
- **Purpose:** Return a simple API health payload.
- **Success:** HTTP 200 JSON with `status`, `application`, `version`, and runtime `timestamp`.
- **Errors:** No custom error behavior is defined by this action.

Example shape:

```json
{
  "status": "ok",
  "application": "Overhead Crane Generator",
  "version": "1.0.0",
  "timestamp": "<runtime timestamp>"
}
```

### Weather forecast (template endpoint)

- **Method/route:** `GET /WeatherForecast`
- **Purpose:** Template sample endpoint; returns five generated forecasts. Temperatures and summaries use `Random.Shared`, so output varies.
- **Request:** None.
- **Success:** HTTP 200 array of `WeatherForecast` values.
- **Errors:** No endpoint-specific errors are defined.

`DrawingsController` has no actions and therefore exposes no drawing routes.

## Error response behavior

- `DesignValidationException` and `ArgumentException` are converted by `ExceptionHandlingMiddleware` to HTTP 400 `application/problem+json` with an `errors` extension.
- Unexpected exceptions are logged and returned as generic HTTP 500 ProblemDetails; stack traces are not sent to clients.
- Missing design IDs are handled by controller branches and return HTTP 404 (some are empty bodies; update/delete supply a string message).
- No HTTP 409 conflict handling is implemented.
- ASP.NET Core `[ApiController]` model-binding errors can return HTTP 400 before service validation.

The `ApiResponse<T>` format is used only by actions that call `AppBaseController.Success`; drawing generation, SVG, and health return direct action results.
