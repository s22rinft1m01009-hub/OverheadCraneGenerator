# Validation & Testing Guide

## Pre-Deployment Validation

Run these commands after completing the setup to validate the implementation.

### 1. Solution Build Validation

```bash
cd OverheadCraneGenerator

# Clean and rebuild
dotnet clean
dotnet build -c Release

# Expected output:
# Build succeeded. (X projects built, 0 failed)
# (or minimal rebuild messages if already built)
```

**If build fails:**
- Check embedded font resources are present in `Drawing/Pdf/Fonts/Assets/`
- Verify `.csproj` has `<EmbeddedResource>` entries uncommented
- Ensure .NET 10 SDK is installed: `dotnet --version`

---

### 2. Domain Model Validation

```bash
# Verify CraneType enum has 9 values
dotnet test -t "CraneType" --filter "EnumCount"

# Or manually in C#:
var types = Enum.GetValues(typeof(CraneType)).Cast<CraneType>().ToList();
Assert.AreEqual(9, types.Count); // Pass = all 9 types present
```

Expected crane types:
```
1. SingleGirderEot
2. DoubleGirderEot
3. Gantry
4. JibCrane
5. Monorail
6. WallMountedJib
7. PortableGantry
8. SemiGantry
9. Underslung
```

---

### 3. Drawing Primitives Validation

Check that all 7 drawing element types are JSON-serializable:

```csharp
using System.Text.Json;
using CraneGenerator.Domain.Entities;

// Verify polymorphic JSON works
var line = new DrawingLine { X1 = 0, Y1 = 0, X2 = 100, Y2 = 100 };
var json = JsonSerializer.Serialize<DrawingElement>(line);
Assert.Contains("\"$type\":\"line\"", json);

var arc = new DrawingArc { CenterX = 50, CenterY = 50, Radius = 25, 
						   StartAngleDegrees = 0, EndAngleDegrees = 180 };
json = JsonSerializer.Serialize<DrawingElement>(arc);
Assert.Contains("\"$type\":\"arc\"", json);

// All 7 types should serialize with their discriminator
```

---

### 4. Generator Factory Validation

```bash
# Verify factory creates all 9 generator types
dotnet test -t "CraneDrawingFactory" --filter "AllGeneratorsRegistered"

# Or manually:
var factory = new CraneDrawingFactory(
	new SingleGirderEotGenerator(),
	new DoubleGirderEotGenerator(),
	new GantryGenerator(),
	new JibCraneGenerator(),
	new MonorailGenerator(),
	new WallMountedJibGenerator(),
	new PortableGantryGenerator(),
	new SemiGantryGenerator(),
	new UnderslungGenerator(),
	calculationService
);

var design = new DesignResponse 
{ 
	CraneType = "DOUBLE_GIRDER_EOT",
	// ... all required fields
};

var model = factory.Generate(design);
Assert.IsNotNull(model);
Assert.IsNotNull(model.Elements);
Assert.IsTrue(model.Elements.Count > 0, "Model should contain geometry elements");
```

---

### 5. PDF Renderer Validation

```bash
# Build and run a minimal test
dotnet new console -n PdfTest
cd PdfTest

# Add references and test code:
dotnet add reference ../CraneGenerator.Application
dotnet add reference ../CraneGenerator.Domain

# Main.cs:
using CraneGenerator.Application.Drawing.Pdf;
using CraneGenerator.Application.Drawing.Pdf.Fonts;
using CraneGenerator.Domain.Entities;

CraneFontResolver.Register();

var model = new DrawingModel 
{ 
	Title = "Test",
	DrawingNumber = "TEST-001",
	Scale = "1:50",
	TechnicalData = new DrawingTechnicalData
	{
		DesignName = "Test Design",
		CraneType = "EOT",
		Note = "Test"
	}
};

model.Elements.Add(new DrawingLine { X1 = 0, Y1 = 0, X2 = 100, Y2 = 100 });
model.Views.Add(new DrawingView { Name = "Test", X = 0, Y = 0, Width = 100, Height = 100 });

var renderer = new PdfDrawingRenderer();
var pdf = renderer.Render(model);

File.WriteAllBytes("test_output.pdf", pdf);
Console.WriteLine($"PDF generated: {pdf.Length} bytes");

# Run:
dotnet run

# Expected output:
# PDF generated: 50000+ bytes
# Opens in PDF viewer: Shows test geometry
```

---

### 6. SVG Renderer Validation

```csharp
using CraneGenerator.Application.Drawing;
using CraneGenerator.Domain.Entities;

var model = new DrawingModel { /* same as PDF test */ };
var renderer = new SvgDrawingRenderer();
var svg = renderer.Render(model);

Assert.IsTrue(svg.Contains("<svg"));
Assert.IsTrue(svg.Contains("</svg>"));
Assert.IsTrue(svg.Contains("viewBox="));
Assert.IsTrue(svg.Length > 1000, "SVG should contain substantive geometry");

File.WriteAllText("test_output.svg", svg);
Console.WriteLine("SVG generated successfully");
```

---

### 7. API Integration Test

```bash
# Start the API
dotnet run --project OverheadCraneGenerator &

# Wait 3 seconds for startup
sleep 3

# Test endpoint availability
curl -s http://localhost:5000/api/health || echo "API not ready"

# Create a design
RESPONSE=$(curl -s -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -d '{
	"designName":"Test Gantry",
	"craneType":"GANTRY",
	"capacityTonnes":20,
	"spanMm":8000,
	"liftHeightMm":5000,
	"runwayWidthMm":10000,
	"hoistPositionMm":3000,
	"buildingHeightMm":0
  }')

echo $RESPONSE | grep -q '"id"' && echo "✓ Design created" || echo "✗ Design creation failed"

# Extract ID (adjust parsing as needed)
ID=$(echo $RESPONSE | grep -o '"id":[0-9]*' | head -1 | grep -o '[0-9]*')

if [ -z "$ID" ]; then
  echo "Could not extract design ID, trying alternative..."
  ID=1
fi

# Get PDF
curl -s http://localhost:5000/api/drawings/$ID/pdf -o test_gantry.pdf
[ -f test_gantry.pdf ] && echo "✓ PDF downloaded ($(stat -f%z test_gantry.pdf) bytes)" || echo "✗ PDF download failed"

# Get SVG
curl -s http://localhost:5000/api/drawings/$ID/svg -o test_gantry.svg
[ -f test_gantry.svg ] && echo "✓ SVG downloaded" || echo "✗ SVG download failed"

# Verify PDF is valid
file test_gantry.pdf | grep -q PDF && echo "✓ PDF is valid" || echo "✗ PDF is invalid"

# Verify SVG is valid XML
xmllint --noout test_gantry.svg 2>/dev/null && echo "✓ SVG is valid XML" || echo "✗ SVG XML is invalid"

# Open files for visual inspection
open test_gantry.pdf
open test_gantry.svg
```

---

### 8. Parameter Variation Test

Test that geometry changes with design parameters:

```bash
# Test 1: Small span (narrow building)
curl -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -d '{
	"designName":"Narrow",
	"craneType":"SINGLE_GIRDER_EOT",
	"capacityTonnes":5,
	"spanMm":3000,
	"liftHeightMm":2000,
	"runwayWidthMm":4000,
	"hoistPositionMm":750,
	"buildingHeightMm":2400
  }' > narrow.json

# Test 2: Large span (wide facility)
curl -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -d '{
	"designName":"Wide",
	"craneType":"SINGLE_GIRDER_EOT",
	"capacityTonnes":50,
	"spanMm":15000,
	"liftHeightMm":6000,
	"runwayWidthMm":18000,
	"hoistPositionMm":5000,
	"buildingHeightMm":5000
  }' > wide.json

# Compare PDFs (file sizes should differ)
ls -lh narrow.pdf wide.pdf

# Expected: Both generate valid PDFs with visibly different proportions
```

---

### 9. All Crane Types Quick Test

```bash
#!/bin/bash
types=("SINGLE_GIRDER_EOT" "DOUBLE_GIRDER_EOT" "GANTRY" "JIB_CRANE" "MONORAIL" "WALL_MOUNTED_JIB" "PORTABLE_GANTRY" "SEMI_GANTRY" "UNDERSLUNG")

for type in "${types[@]}"; do
  echo "Testing $type..."

  curl -X POST http://localhost:5000/api/designs \
	-H "Content-Type: application/json" \
	-d "{
	  \"designName\":\"Test $type\",
	  \"craneType\":\"$type\",
	  \"capacityTonnes\":10,
	  \"spanMm\":6000,
	  \"liftHeightMm\":4000,
	  \"runwayWidthMm\":8000,
	  \"hoistPositionMm\":2000,
	  \"buildingHeightMm\":3500
	}" > /tmp/design_$type.json

  if grep -q '"id"' /tmp/design_$type.json; then
	echo "✓ $type design created"
  else
	echo "✗ $type design failed"
  fi
done

# Expected output: Each type shows ✓
```

---

### 10. Dimension Accuracy Test

Open generated PDFs and verify dimensions match input:

```csharp
// Input parameters
var design = new DesignResponse 
{
	SpanMm = 6000,
	LiftHeightMm = 4000,
	RunwayWidthMm = 8000,
	HoistPositionMm = 2000,
	BuildingHeightMm = 3500
};

// After generating PDF, manually verify in Adobe:
// - Span dimension label shows "6000 mm" ✓
// - Lift height shows "4000 mm" ✓
// - Runway width shows "8000 mm" ✓
// - Hoist position shows "Hoist 2000 mm" ✓
// - Building height shows "3500 mm" (if > 0) ✓
```

---

### 11. Rendering Correctness Test

For each crane type, verify:

| Element | Expected | Validate |
|---------|----------|----------|
| Girders | Colored rectangles #f4c542 (gold) | Visual inspection in PDF |
| Columns | Colored rectangles #d3d3d3 (gray) | Visual inspection |
| Trolley | Blue circular marker #4f81bd | SVG circle, PDF ellipse |
| Centerlines | Dashed lines | SVG stroke-dasharray, PDF style |
| Steel structure | Light gray #eeeeee | Observe beams/legs |
| Dimensions | Arrows + labels | Check readability, spacing |
| Title block | Right side panel | Verify text + borders |
| Technical data | Right upper panel | Verify all 8 fields |

---

## Test Data Sets

### Test Set 1: Standard EOT (Double Girder)
```json
{
  "designName": "Standard Industrial EOT",
  "craneType": "DOUBLE_GIRDER_EOT",
  "capacityTonnes": 10,
  "spanMm": 6000,
  "liftHeightMm": 4000,
  "runwayWidthMm": 8000,
  "hoistPositionMm": 2000,
  "buildingHeightMm": 3500
}
```
**Expected PDF**: Standard A3 layout, 2 girders visible in elevation, trolley centered in plan.

### Test Set 2: Large Span Gantry
```json
{
  "designName": "Heavy Duty Outdoor Gantry",
  "craneType": "GANTRY",
  "capacityTonnes": 50,
  "spanMm": 12000,
  "liftHeightMm": 6000,
  "runwayWidthMm": 15000,
  "hoistPositionMm": 4000,
  "buildingHeightMm": 0
}
```
**Expected PDF**: Angled legs visible in elevation, no building columns, wide footprint in plan.

### Test Set 3: Compact Jib
```json
{
  "designName": "Compact Workshop Jib",
  "craneType": "JIB_CRANE",
  "capacityTonnes": 2,
  "spanMm": 2000,
  "liftHeightMm": 2000,
  "runwayWidthMm": 2000,
  "hoistPositionMm": 500,
  "buildingHeightMm": 2400
}
```
**Expected PDF**: Vertical column, horizontal boom, 180° arc in elevation, full 360° circle in plan.

---

## Success Criteria

✅ **Implementation is successful if:**

1. All 9 crane types generate PDFs without errors
2. All PDFs open in standard PDF viewers
3. All PDFs show 3 distinct views (elevation, plan, section)
4. Dimensions are present, readable, and approximately correct
5. Colors match the engineering palette
6. No geometry overlaps or hidden behind text
7. SVG output validates as well-formed XML
8. SVG displays correctly in web browser
9. Fonts render without fallback warnings
10. Existing CRUD API routes continue to function
11. Parameter changes result in visibly different geometry
12. No runtime exceptions during normal operation

---

## Rollback Instructions

If validation fails:

```bash
# Restore original Windows fonts approach (temporary fix):
git checkout CraneGenerator.Application/Drawing/Pdf/Fonts/CraneFontResolver.cs

# Clear build cache
dotnet clean

# Rebuild
dotnet build

# Then implement embedded fonts properly as per QUICK_START.md
```

---

**For any issues, check IMPLEMENTATION_REPORT.md Troubleshooting section.**
