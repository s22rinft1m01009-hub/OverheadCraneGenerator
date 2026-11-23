# Backend Drawing Engine

## Entry flow

A saved design is read through `IDesignService` and passed to `ICraneDrawingService.Generate(DesignResponse)`. `CraneDrawingService` delegates to `CraneDrawingFactory`. The factory parses the normalized `CraneType`, asks `ICraneCalculationService` for drawing-oriented derived values, selects an `ICraneGeometryGenerator`, and returns a `DrawingModel`.

```text
DesignResponse
	-> CraneDrawingService
	-> CraneDrawingFactory + CraneCalculationService
	-> SingleGirderEotGenerator | DoubleGirderEotGenerator | GantryGenerator
	-> DrawingModel
	-> SvgDrawingRenderer -> SVG
```

PDF generation is not implemented.

## Drawing model types

`DrawingModel` contains:

- `DrawingNumber`, `Title`, and `Scale`
- `Views` (`List<DrawingView>`)
- `Elements` (`List<DrawingElement>`)
- `Dimensions` (`List<DrawingDimension>`)
- `TechnicalData` (`DrawingTechnicalData`)

`DrawingView` defines name and canvas-space position/size. Every geometry generator creates three named projections: `MAIN ELEVATION`, `PLAN VIEW`, and `SECTION A-A`.

`DrawingElement` is the base type with stroke and stroke width. Derived types are:

- `DrawingLine`: `X1`, `Y1`, `X2`, `Y2`
- `DrawingRectangle`: `X`, `Y`, `Width`, `Height`, `Fill`
- `DrawingCircle`: `CenterX`, `CenterY`, `Radius`, `Fill`

System.Text.Json attributes on `DrawingElement` register `line`, `rectangle`, and `circle` discriminators. There is no `DrawingText` element; labels are rendered by the SVG renderer. `DrawingDimension` is a separate model with endpoints, `Label`, and `Offset`, not a `DrawingElement` subtype.

`DrawingTechnicalData` holds design name/type, drawing number, revision/note defaults, capacity, span, lift height, runway width, hoist position, and building height.

## Crane generators and topology

### `SingleGirderEotGenerator`

- Elevation: two vertical building-column rectangles, ground line, one main girder rectangle, trolley/hoist rectangle, rope line, and hook circle.
- Plan: two runway/reference lines, one girder rectangle, and one hoist rectangle.
- Section: one girder rectangle, hoist rectangle, and a vertical line.

### `DoubleGirderEotGenerator`

- Elevation: two building columns, ground line, and two separately drawn horizontal girder rectangles; a wider/taller trolley rectangle spans the arrangement, with rope and hook.
- Plan: two separated longitudinal lines, a trolley rectangle across their separation, and additional outer reference lines.
- Section: two separate horizontal girder rectangles and a trolley rectangle.

### `GantryGenerator`

- Elevation: top girder, two sloped free-standing legs, two base/foot rectangles, ground line, trolley, rope, and hook. It does not draw EOT building columns.
- Plan: footprint rectangle, end-side lines, and circular hoist symbol.
- Section: top girder, two sloped legs, and two base rectangles.

No Jib, Monorail, Wall-Mounted Jib, Portable Gantry, Semi-Gantry, or Underslung generator exists in the current Application project.

## Calculations and parameter behavior

`CraneCalculationService` returns a `CraneCalculationResult` containing the hoist position ratio, fixed drawing width, a derived drawing X value, building-height drawing value, rope length, runway drawing depth, and a scale string. Its display rules are in [ENGINEERING_NOTES.md](ENGINEERING_NOTES.md); the repository does not establish an engineering source for them.

The generators use the hoist ratio to position trolley/hoist within their respective view bounds. Common dimension labels use actual `DesignResponse` inputs: span, lift height, building height, runway width, and hoist position. Values are formatted with `N0` (or `0` in SVG technical data) and `mm` labels.

The drawing coordinates are view/canvas units, not direct millimetres. However, the current generator geometry mostly uses fixed view-space widths/positions; input changes do not uniformly scale every structural member. In particular, span changes dimension text but the fixed span-view width remains fixed. Capacity affects technical data, not generated geometry. Building height affects EOT elevation bounds through the calculation service; Gantry elevation uses fixed top/leg coordinates. Runway width affects some plan/section display dimensions through clamped prototype rules.

## Drawing number, scale, and title block

The base generator creates a stable drawing number `CRN-{id:D3}` and title `PARAMETRIC OVERHEAD CRANE DRAWING`. It sets `Scale` to the string from `CraneCalculationService`.

`SvgDrawingRenderer` emits a 1200×800 SVG, border, view borders/labels, geometry, dimensions, a technical-data area, and a title block. The title block displays drawing number, scale, revision A, current date, and `CONCEPTUAL DRAWING - NOT FOR FABRICATION`.

`DrawingTechnicalData` has `Revision` and `Note` fields, but the renderer currently hardcodes revision A and the conceptual note rather than reading those properties. The renderer shows the design name/type and numeric technical data in its technical-data panel.

## SVG serialization

`SvgDrawingRenderer.Render(DrawingModel)` converts the shared drawing model to SVG. It handles lines, rectangles, circles, dimension lines/extension lines/arrowheads/labels, view borders, text labels, technical data, and title block. The API endpoint returns the rendered string as `image/svg+xml`.

There is no PDF renderer, PDF package, or PDF API endpoint. There are also no SVG/drawing automated tests in the current test project.
