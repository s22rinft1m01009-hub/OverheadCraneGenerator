# Backend Testing

## Test project and framework

`CraneGenerator.Tests/CraneGenerator.Tests.csproj` targets `net10.0`. It declares AutoMapper and EF Core-related packages but does not declare `Microsoft.NET.Test.Sdk`, xUnit, NUnit, MSTest, or a test adapter. It also has no project reference to `CraneGenerator.Application`.

Two source files exist:

- `CraneCalculationTests.cs` — empty `CraneCalculationTests` class.
- `DesignValidationTests.cs` — empty `DesignValidationTests` class.

There are no test methods or assertions in these files.

## Coverage status

| Test area | Status | Repository evidence |
|---|---|---|
| Unit tests | Not Implemented | No test methods/assertions; no test framework configured |
| Service tests | Not Implemented | No service test cases |
| Validation tests | Not Implemented | File named `DesignValidationTests.cs` contains only an empty class |
| API/integration tests | Not Implemented | No test host or API test cases |
| CRUD tests | Not Implemented | No CRUD assertions |
| Database/persistence tests | Not Implemented | No database tests or restart-persistence test |
| Calculation tests | Not Implemented | File named `CraneCalculationTests.cs` contains only an empty class |
| Drawing topology tests | Not Implemented | No generator/drawing assertions |
| SVG tests | Not Implemented | No renderer tests |
| PDF tests | Not Implemented | PDF renderer/endpoint and tests are absent |
| Error-handling tests | Not Implemented | No middleware/controller error tests |

## Commands

Restore/build commands supported by the solution setup:

```powershell
dotnet restore .\OverheadCraneGenerator.slnx
dotnet build .\OverheadCraneGenerator.slnx
```

The conventional test command is:

```powershell
dotnet test .\OverheadCraneGenerator.slnx
```

However, the current test project has no test SDK/framework/adapter and contains no tests, so the repository does not currently provide meaningful automated test discovery or coverage. No test result is claimed here.
