# Quick Start Checklist

## Pre-Build

- [ ] Download Liberation Sans fonts:
  - [ ] LiberationSans-Regular.ttf
  - [ ] LiberationSans-Bold.ttf
  - [ ] From: https://github.com/liberationfonts/liberation-fonts/releases

- [ ] Add fonts to project:
  - [ ] Create folder: `CraneGenerator.Application/Drawing/Pdf/Fonts/Assets/`
  - [ ] Copy both TTF files into this folder
  - [ ] In Visual Studio: Right-click each file → Properties → Build Action = **EmbeddedResource**

  **OR** uncomment in `CraneGenerator.Application/CraneGenerator.Application.csproj`:
  ```xml
  <ItemGroup>
	<EmbeddedResource Include="Drawing/Pdf/Fonts/Assets/LiberationSans-Regular.ttf" />
	<EmbeddedResource Include="Drawing/Pdf/Fonts/Assets/LiberationSans-Bold.ttf" />
  </ItemGroup>
  ```

## Build

```bash
cd OverheadCraneGenerator
dotnet clean
dotnet build
```

Expected build output:
- ✅ CraneGenerator.Domain
- ✅ CraneGenerator.Infrastructure
- ✅ CraneGenerator.Application
- ✅ OverheadCraneGenerator (API/Web)
- ✅ CraneGenerator.Tests (if present)

## Verify

```bash
# Run unit tests (if implemented)
dotnet test CraneGenerator.Tests

# Expected: All tests pass or "no tests found" if not implemented
```

## Test API (If Running)

```bash
# Start the API
dotnet run --project OverheadCraneGenerator

# In another terminal, create a design
curl -X POST http://localhost:5000/api/designs \
  -H "Content-Type: application/json" \
  -d '{
	"designName":"SingleGirder Test",
	"craneType":"SINGLE_GIRDER_EOT",
	"capacityTonnes":5,
	"spanMm":5000,
	"liftHeightMm":3000,
	"runwayWidthMm":6000,
	"hoistPositionMm":1500,
	"buildingHeightMm":3000
  }'

# Note the returned design ID (e.g., 1)

# Get the PDF
curl -X GET http://localhost:5000/api/drawings/1/pdf \
  -o test_single_girder.pdf

# Open in your PDF viewer
open test_single_girder.pdf
```

## Expected Results

- [ ] PDF generates without errors
- [ ] PDF is approximately 50-200 KB
- [ ] PDF opens in Adobe Reader or browser
- [ ] PDF shows 3 views (Elevation, Plan, Section A-A)
- [ ] Dimensions are readable
- [ ] Colors match crane type
- [ ] No overlapping geometry
- [ ] Font renders correctly

## If Build Fails

1. **"Font resource not found"**
   - Verify fonts are in `Drawing/Pdf/Fonts/Assets/` folder
   - Check Build Action is set to **EmbeddedResource**
   - Run: `dotnet clean && dotnet build`

2. **"Unknown font face"**
   - Ensure `CraneFontResolver.Register()` is called in `Program.cs` line 1
   - Rebuild solution

3. **"A different PDFsharp font resolver has already been configured"**
   - Remove any other font resolver registrations
   - Check for duplicate `CraneFontResolver.Register()` calls

4. **PDF renders but shows no geometry**
   - Check PDFsharp version is 6.2.2
   - Verify XGraphics API compatibility
   - Test with a simpler crane type (SingleGirderEot)

## All 9 Crane Types Available

Test with each type (in `craneType` field):

1. `SINGLE_GIRDER_EOT` - single main beam
2. `DOUBLE_GIRDER_EOT` - two parallel beams
3. `GANTRY` - freestanding mobile
4. `JIB_CRANE` - rotating boom on column
5. `MONORAIL` - single rail, suspended hoist
6. `WALL_MOUNTED_JIB` - wall-anchored jib
7. `PORTABLE_GANTRY` - mobile gantry with casters
8. `SEMI_GANTRY` - building column + free leg
9. `UNDERSLUNG` - overhead runway, hoist below

## Support

See `IMPLEMENTATION_REPORT.md` for:
- Full architecture documentation
- API usage examples
- Troubleshooting guide
- Enhancement roadmap
