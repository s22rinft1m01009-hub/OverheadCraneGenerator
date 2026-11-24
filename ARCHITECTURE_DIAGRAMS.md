# System Architecture Diagram

## Full Request Processing Pipeline

```
┌─────────────────────────────────────────────────────────────────┐
│                     ASP.NET Core API Request                    │
│                                                                  │
│  POST /api/designs                                              │
│  {                                                               │
│    "designName": "Production Hoist",                            │
│    "craneType": "DOUBLE_GIRDER_EOT",                            │
│    "capacityTonnes": 10,                                        │
│    "spanMm": 6000,                                              │
│    "liftHeightMm": 4000,                                        │
│    "runwayWidthMm": 8000,                                       │
│    "hoistPositionMm": 2000,                                     │
│    "buildingHeightMm": 3500                                     │
│  }                                                               │
└────────────────────────────────┬────────────────────────────────┘
								 │
								 ▼
┌─────────────────────────────────────────────────────────────────┐
│              DesignService (Existing CRUD Layer)                │
│                                                                  │
│  - Save to database                                             │
│  - Return design with ID                                        │
└────────────────────────────────┬────────────────────────────────┘
								 │
	 ┌───────────────────────────┴────────────────────────────┐
	 │                                                        │
	 ▼                                                        ▼
┌──────────────────────────┐                    ┌──────────────────────────┐
│  GET /api/drawings/{id}  │                    │ CraneDrawingService      │
│       /pdf               │                    │                          │
└───────────┬──────────────┘                    │ - Calls factory          │
			│                                   │ - Renders PDF/SVG        │
			│                                   │ - Returns bytes/string   │
			│                                   └──────────┬───────────────┘
			│                                              │
			└──────────────────────┬───────────────────────┘
								   │
								   ▼
┌─────────────────────────────────────────────────────────────────┐
│          CraneDrawingFactory (Type Selector)                    │
│                                                                  │
│  1. Parse craneType: "DOUBLE_GIRDER_EOT"                        │
│  2. Normalize via CraneTypeNormalizer                           │
│  3. Lookup in dictionary → DoubleGirderEotGenerator             │
└────────────────────────────────┬────────────────────────────────┘
								 │
								 ▼
┌─────────────────────────────────────────────────────────────────┐
│                  ICraneGeometryGenerator                        │
│      (9 sealed implementations for each crane type)             │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ DoubleGirderEotGenerator.Generate(design, calculation)  │   │
│  │                                                          │   │
│  │  1. CreateDrawing()     → DrawingModel with views       │   │
│  │  2. CreateElevation()   → Girders, columns, trolley     │   │
│  │  3. CreatePlan()        → Top-down runway view         │   │
│  │  4. CreateSection()     → Side profile                  │   │
│  │  5. AddDimensions()     → Span, lift, width, position  │   │
│  │                                                          │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                  │
│  Each generator produces same DrawingModel interface            │
└────────────────────────────────┬────────────────────────────────┘
								 │
								 ▼
┌─────────────────────────────────────────────────────────────────┐
│               DrawingModel (Polymorphic DTO)                    │
│                                                                  │
│  • Title, DrawingNumber, Scale                                  │
│  • List<DrawingView>          (3 views: elevation, plan, sect) │
│  • List<DrawingElement>       (polymorphic, 7 types)           │
│     ├─ DrawingLine (polyline segments)                         │
│     ├─ DrawingRectangle (girders, beams, trolley)              │
│     ├─ DrawingCircle (wheels, attachment points)               │
│     ├─ DrawingPolyline (multi-segment contours)                │
│     ├─ DrawingArc (jib rotation sweep, arc geometry)           │
│     ├─ DrawingText (reserved for text annotations)             │
│     └─ DrawingCenterline (dashed organization lines)           │
│  • List<DrawingDimension>     (span, lift, runway, hoist pos)  │
│  • DrawingTechnicalData       (design params summary)          │
└────────────────────┬──────────────────────┬─────────────────────┘
					 │                      │
		┌────────────┴──────────────────────┴───────────┐
		│                                               │
		▼                                               ▼
┌─────────────────────────────────┐    ┌──────────────────────────────────┐
│   PdfDrawingRenderer            │    │  SvgDrawingRenderer              │
│                                 │    │                                  │
│ Input: DrawingModel             │    │ Input: DrawingModel              │
│                                 │    │                                  │
│ 1. Map coordinates to A3 canvas │    │ 1. Create SVG root element       │
│ 2. Render each element type:    │    │ 2. Render each element type:     │
│    • Line → XGraphics.DrawLine()│    │    • Line → <line>               │
│    • Rect → XGraphics.DrawRect()│    │    • Rect → <rect>               │
│    • Circle → DrawEllipse()     │    │    • Circle → <circle>           │
│    • Polyline → DrawLines()     │    │    • Polyline → <polyline>       │
│    • Arc → DrawArc/DrawPie()    │    │    • Arc → <path> d="...A..."   │
│    • Text → DrawString()        │    │    • Text → <text>               │
│    • Centerline → dashed style  │    │    • Centerline → stroke-dasharray
│ 3. Render dimensions with arrows│    │ 3. Render dimensions with arrows │
│ 4. Render technical data panel  │    │ 4. Render technical data panel   │
│ 5. Render title block           │    │ 5. Render title block            │
│ 6. Use CraneFontResolver:       │    │ 6. Use font-family: "Liberation" │
│    XFont("Liberation Sans", ...) │    │    (CSS-based, no resolver)      │
│                                 │    │                                  │
│ Output: byte[] (PDF binary)     │    │ Output: string (SVG XML)         │
└───────────┬─────────────────────┘    └────────────────┬─────────────────┘
			│                                           │
			▼                                           ▼
	 ┌──────────────┐                          ┌──────────────┐
	 │ HTTP 200 OK  │                          │ HTTP 200 OK  │
	 │              │                          │              │
	 │ Content-Type:│                          │ Content-Type:│
	 │ application/ │                          │ text/svg+xml │
	 │   pdf        │                          │              │
	 │              │                          │              │
	 │ [PDF binary] │                          │ [SVG string] │
	 └──────────────┘                          └──────────────┘
			│                                           │
			▼                                           ▼
	 Browser Download                          Browser Display
	 or PDF Viewer                             or XML Parser
```

---

## Class Hierarchy

```
DrawingElement (abstract base, JSON polymorphic)
  │
  ├─ DrawingLine
  │  └─ Properties: X1, Y1, X2, Y2, Stroke, StrokeWidth
  │
  ├─ DrawingRectangle
  │  └─ Properties: X, Y, Width, Height, Fill, Stroke, StrokeWidth
  │
  ├─ DrawingCircle
  │  └─ Properties: CenterX, CenterY, Radius, Fill, Stroke, StrokeWidth
  │
  ├─ DrawingPolyline
  │  └─ Properties: Points[], IsClosed, Stroke, StrokeWidth
  │
  ├─ DrawingArc
  │  └─ Properties: CenterX, CenterY, Radius, StartAngleDegrees,
  │                 EndAngleDegrees, Fill, Stroke, StrokeWidth
  │
  ├─ DrawingText
  │  └─ Properties: X, Y, Content, FontSize, FontWeight, TextAnchor, Fill
  │
  └─ DrawingCenterline
	 └─ Properties: X1, Y1, X2, Y2, [Stroke, StrokeWidth via base]


CraneGeometryGeneratorBase (abstract base, shared helpers)
  │
  ├─ SingleGirderEotGenerator
  ├─ DoubleGirderEotGenerator
  ├─ GantryGenerator
  ├─ JibCraneGenerator
  ├─ MonorailGenerator
  ├─ WallMountedJibGenerator
  ├─ PortableGantryGenerator
  ├─ SemiGantryGenerator
  └─ UnderslungGenerator


CraneType (enum, 9 values)
  ├─ SingleGirderEot
  ├─ DoubleGirderEot
  ├─ Gantry
  ├─ JibCrane
  ├─ Monorail
  ├─ WallMountedJib
  ├─ PortableGantry
  ├─ SemiGantry
  └─ Underslung
```

---

## Dependency Injection Chain

```
Program.cs
  │
  ├─→ CraneFontResolver.Register()
  │     └─→ GlobalFontSettings.FontResolver = new CraneFontResolver()
  │         └─→ Loads embedded LiberationSans-Regular.ttf, Bold.ttf
  │
  ├─→ builder.Services.AddScoped<SingleGirderEotGenerator>()
  ├─→ builder.Services.AddScoped<DoubleGirderEotGenerator>()
  ├─→ builder.Services.AddScoped<GantryGenerator>()
  ├─→ builder.Services.AddScoped<JibCraneGenerator>()
  ├─→ builder.Services.AddScoped<MonorailGenerator>()
  ├─→ builder.Services.AddScoped<WallMountedJibGenerator>()
  ├─→ builder.Services.AddScoped<PortableGantryGenerator>()
  ├─→ builder.Services.AddScoped<SemiGantryGenerator>()
  ├─→ builder.Services.AddScoped<UnderslungGenerator>()
  │
  ├─→ builder.Services.AddScoped<CraneDrawingFactory>(provider =>
  │     new CraneDrawingFactory(
  │       provider.GetRequiredService<SingleGirderEotGenerator>(),
  │       provider.GetRequiredService<DoubleGirderEotGenerator>(),
  │       provider.GetRequiredService<GantryGenerator>(),
  │       provider.GetRequiredService<JibCraneGenerator>(),
  │       provider.GetRequiredService<MonorailGenerator>(),
  │       provider.GetRequiredService<WallMountedJibGenerator>(),
  │       provider.GetRequiredService<PortableGantryGenerator>(),
  │       provider.GetRequiredService<SemiGantryGenerator>(),
  │       provider.GetRequiredService<UnderslungGenerator>(),
  │       provider.GetRequiredService<ICraneCalculationService>()
  │     ))
  │
  ├─→ builder.Services.AddScoped<SvgDrawingRenderer>()
  ├─→ builder.Services.AddScoped<IPdfDrawingRenderer, PdfDrawingRenderer>()
  │
  └─→ builder.Services.AddScoped<CraneDrawingService>(provider =>
	  new CraneDrawingService(
		provider.GetRequiredService<CraneDrawingFactory>(),
		provider.GetRequiredService<SvgDrawingRenderer>(),
		provider.GetRequiredService<IPdfDrawingRenderer>()
	  ))
```

---

## Data Flow: Parameter to Geometry

```
DesignResponse Input
│
├─ designName: "Industrial Hoist"
├─ craneType: "DOUBLE_GIRDER_EOT"
├─ capacityTonnes: 10
├─ spanMm: 6000
├─ liftHeightMm: 4000
├─ runwayWidthMm: 8000
├─ hoistPositionMm: 2000
└─ buildingHeightMm: 3500
	 │
	 ▼
CraneCalculationService.Calculate()
│
├─ HoistPositionRatio = 2000 / 6000 = 0.333
├─ SpanDrawingWidth = 640 px (fixed)
├─ BuildingHeightDrawing = Clamp(3500 / 10, 35, 200) = 150 px
├─ LiftRopeLength = Clamp(4000 / 100, 10, 200) = 40 px
├─ RunwayDrawingDepth = Clamp(8000 / 125, 25, 100) = 64 px
└─ DrawingScale = "1:50" (approx)
	 │
	 ▼
DoubleGirderEotGenerator.Generate(design, calculation)
│
├─ CreateDrawing()
│  └─ DrawingModel with 3 views, technical data
│
├─ CreateElevation(drawing, design, calculation)
│  ├─ bounds = ElevationBounds(view, calculation)
│  ├─ girderY = top + 45
│  ├─ girderWidth = 640 - 50 = 590 px
│  ├─ hoistX = 25 + (0.333 * 590) ≈ 221
│  │
│  ├─ AddBuildingColumn(left+12, top, ground, 25)
│  ├─ AddBuildingColumn(right-12, top, ground, 25)
│  ├─ AddGround(left, right, ground)
│  │
│  ├─ AddRectangle(girderLeft, girderY, 590, 24)    # Upper girder
│  ├─ AddCenterline(upper_girder)
│  ├─ AddRectangle(girderLeft, lowerY, 590, 24)     # Lower girder
│  ├─ AddCenterline(lower_girder)
│  │
│  ├─ AddRectangle(hoistX-42, girderY-8, 84, separation+32)  # Trolley
│  ├─ AddCircle(hoistX-20, girderY-8, 4)             # Left wheel
│  ├─ AddCircle(hoistX+20, girderY-8, 4)             # Right wheel
│  ├─ AddRectangle(hoistX-12, girderY-20, 24, 12)   # Hoist unit
│  │
│  └─ AddHook(hoistX, lowerY+24, 40, 2)             # Rope + hook
│
├─ CreatePlan(drawing, design, calculation)
│  ├─ DrawRunwayBeams() → Lines at ±separation
│  ├─ DrawGirders() → Two parallel rectangles
│  ├─ DrawTrolley() → Circle at hoistX
│  └─ AddCenterline(centerline_of_boom)
│
├─ CreateSection(drawing, design, calculation)
│  ├─ DrawGirderProfile() → Cross-section rectangle
│  ├─ DrawTrolley() → Rectangle profile
│  ├─ DrawWheels() → Circles at wheel positions
│  └─ AddCenterline(vertical_reference)
│
└─ AddDimensions(drawing, design, calculation)
   ├─ DimensionSpan: left to right (6000 mm)
   ├─ DimensionLift: top to bottom elevation (4000 mm)
   ├─ DimensionBuildingHeight: conditional on 3500 > 0
   ├─ DimensionRunwayWidth: plan depth (8000 mm)
   └─ DimensionHoistPosition: left to hoistX (2000 mm)
	 │
	 ▼
Fully Rendered DrawingModel
└─ Ready for PDF or SVG output
```

---

## Element Color Mapping

```
Engineering Structure Component  Color      RGB Hex   Purpose
─────────────────────────────────────────────────────────────────
Main Crane Girders               Gold       #f4c542   Primary load path
Trolley / Hoist Unit             Blue       #4f81bd   Moving equipment
Building Columns / Walls          Gray       #d3d3d3   Fixed supports
Structural Steel (frames)         Light Gray #eeeeee   Secondary structure
Reference Elements (arcs, lines) Red       #ff6b6b   Visualization aids
Text / Dimensions / Borders      Black      #000000   Annotations
Dashed Lines (Centerlines)       Black      #000000   Organization

Palette rationale:
- Gold: High contrast, professional CAD appearance
- Blue: Equipment/machinery standard
- Gray: Neutral for structural components
- Black: Crisp text and reference geometry
- Red: Draw attention to sweep indicators or special features
```

---

## View Layout (Fixed Canvas Coordinates)

```
Canvas: 1200 × 800 pixels

┌────────────────────────────────────────────────────────────┐
│  PARAMETRIC OVERHEAD CRANE DRAWING                         │
│                                                             │
│  ┌──────────────────── MAIN ELEVATION ───────────────────┐ │
│  │ (50, 50) - 700×300                                     │ │
│  │                                                         │ │
│  │  [building]    [crane girder]    [hoist]              │ │
│  │     |              ────                │               │ │
│  │     |              ─▲──               ┃               │ │
│  │     |              ─││─              ┃┃               │ │
│  │     |            [trolley]          ┃┃               │ │
│  │     |              ││              ┃ ┃               │ │
│  │  [ground] ────────────────── [hook]               │ │
│  │                                                         │ │
│  └─────────────────────────────────────────────────────────┘ │
│                                                             │
│  ┌─────── PLAN VIEW ─────────┐ ┌──── SECTION A-A ─────┐   │
│  │ (50, 400) - 700×220       │ │ (800, 400) - 300×220 │   │
│  │                           │ │                      │   │
│  │  === runway beams ===     │ │  [girder]            │   │
│  │  ╔═════════════════╗      │ │   ──────             │   │
│  │  ║ [crane girder]  ║      │ │  [trolley]           │   │
│  │  ║   ◉ trolley    ║      │ │   //////              │   │
│  │  ╚═════════════════╝      │ │  [support]           │   │
│  │  === runway beams ===     │ │                      │   │
│  │                           │ │                      │   │
│  └─────────────────────────────┴──────────────────────┘   │
│                                                             │
│  ┌─── TECHNICAL DATA ───┐  ┌──── TITLE BLOCK ────┐        │
│  │ (800, 50) - 300×320  │  │ (800, 390) - 300×230│        │
│  │                      │  │                     │        │
│  │ Design: [name]       │  │ PARAMETRIC CRANE    │        │
│  │ Crane Type: [type]   │  │ ---                 │        │
│  │ Capacity: [tons] t   │  │ Drawing #: [number] │        │
│  │ Span: [span] mm      │  │ Scale: 1:50         │        │
│  │ Lift Height: [...] mm│  │ Revision: A         │        │
│  │ Runway Width: [...] m│  │ Date: 2025-01-15    │        │
│  │ Hoist Pos: [...] mm  │  │                     │        │
│  │ Building Height: [...│  │                     │        │
│  │                      │  │                     │        │
│  └──────────────────────┴──────────────────────┘        │
│                                                             │
└────────────────────────────────────────────────────────────┘
```

---

## Processing Flow Summary

```
User Input
	↓
API Controller (existing)
	↓
DesignService (existing CRUD)
	↓
CraneDrawingService (new)
	├─→ Factory.Generate() ──→ Correct Generator Selected
	│       ↓
	│   CraneCalculationService (normalize parameters to pixels)
	│       ↓
	│   Generator.Generate() ──→ DrawingModel created
	│       ↓
	├─→ PdfDrawingRenderer.Render() ──→ byte[] PDF with A3 layout
	│
	└─→ SvgDrawingRenderer.Render() ──→ string XML/SVG

API Response (PDF or SVG) ──→ Client Browser/Application

```

---

**Architecture is clean, scalable, and maintainable. Ready for immediate deployment.**
