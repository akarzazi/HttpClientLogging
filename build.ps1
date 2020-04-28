[CmdletBinding(PositionalBinding = $false)]
param(
    [switch] $CreatePackages,
    [switch] $RunTest
)

Write-Host "Run Parameters:" -ForegroundColor Cyan
Write-Host "  CreatePackages: $CreatePackages"
Write-Host "  RunTests: $RunTests"
Write-Host "  dotnet --version:" (dotnet --version)

$packageOutputFolder = "$PSScriptRoot\.nupkgs"

Write-Host "Restoring all projects..." -ForegroundColor "Magenta"
dotnet restore
Write-Host "Done restoring." -ForegroundColor "Green"

Write-Host "Building all projects..." -ForegroundColor "Magenta"
dotnet build -c Release --no-restore
Write-Host "Done building." -ForegroundColor "Green"

# ----------TESTS------------
if ($RunTest) {
    Write-Host "Running tests: " -ForegroundColor "Magenta"
    dotnet test -c Release 
    if ($LastExitCode -ne 0) {
        Write-Host "Error with tests, aborting build." -Foreground "Red"
        Exit 1
    }
    Write-Host "Tests passed!" -ForegroundColor "Green"
}

# ----------PACKING------------
if ($CreatePackages) {
    mkdir -Force $packageOutputFolder | Out-Null
    Write-Host "Clearing existing $packageOutputFolder..." -NoNewline
    Get-ChildItem $packageOutputFolder | Remove-Item
    Write-Host "done." -ForegroundColor "Green"

    Write-Host "Packing ..." -ForegroundColor "Magenta"
    dotnet pack ".\src\HttpClientLogging\HttpClientLogging.csproj" --no-build -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -c Release /p:PackageOutputPath=$packageOutputFolder
}
Write-Host "Build Complete." -ForegroundColor "Green"
