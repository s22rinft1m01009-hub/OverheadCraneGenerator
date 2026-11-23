# Backend Engineering Notes

## Scope

The implemented backend creates conceptual parametric crane drawings from stored design inputs. It does not provide structural engineering analysis, certify a crane/building, or approve fabrication. No engineering code, external standard, or verified source for the display-space rules was found in the repository.

## SOURCE — user-supplied/stored inputs

`CraneDesign` and the create/update request DTOs contain:

- Design name
- Crane type string
- Capacity in tonnes
- Span, lift height, runway width, hoist position, and building height in millimetres

`DesignService` stores these input values (and timestamps). These values are not calculated by the backend.

## DERIVED — calculation outputs

`CraneCalculationService.Calculate(DesignResponse)` returns drawing-oriented values:

- `HoistPositionRatio`: hoist position divided by span, clamped to 0–1 (or 0 if span is non-positive).
- `SpanDrawingWidth`: fixed at 640 drawing coordinate units.
- `HoistDrawingX`: `100 + ratio * 640`; this result is present in `CraneCalculationResult`, though the current generators position the hoist using the ratio and view-specific left/right bounds instead.
- `BuildingHeightDrawing`: building height converted through the implementation's linear expression and clamped to 90–170 coordinate units.
- `LiftRopeLength`: lift height divided by 100 and clamped to 35–120 coordinate units.
- `RunwayDrawingDepth`: runway width divided by 1000 and clamped to 25–70 coordinate units.
- `DrawingScale`: `1:<ceil(span / 640)>`, with a minimum denominator of 1.

These are display geometry rules, not engineering calculations. `CraneGeometryGeneratorBase` formats dimension labels from the design inputs rather than deriving the displayed measurements from pixel coordinates.

## ESTIMATE / PROTOTYPE RULE — geometry

The repository implements fixed view coordinates, component offsets, widths, heights, and clamped display values in `CraneGeometryGeneratorBase`, `SingleGirderEotGenerator`, `DoubleGirderEotGenerator`, and `GantryGenerator`. `CraneCalculationService` contains the numeric conversion/clamp rules listed above.

**Source not established in repository; treated as a prototype/assumption.** No supporting engineering references or engineering standard are identified in the code. These display rules must not be interpreted as physical member sizing, clearances, or safe operating dimensions.

## Deliberately unsupported calculations

No implementation was found for:

- Structural strength/stress analysis or deflection
- Wheel loads or runway/building adequacy
- Motor, gearbox, or brake sizing
- Weld design or fatigue calculations
- Fabrication details/approval or engineering certification

Capacity is checked as a positive finite input and displayed in technical data; it is not used by the calculation service to size crane components.

## Validation-related engineering limitation

`DesignValidator` currently requires `BuildingHeightMm` to be positive for every crane type. Therefore, a Gantry request with zero building height is rejected, even though gantry elevation geometry does not use a building envelope. EOT runway width must be positive; Gantry runway width may be zero under the current validator.

## Required interpretation

Treat generated geometry and dimensions as a visual concept only. Do not use the output as a structural calculation, construction document, fabrication instruction, or certification.
