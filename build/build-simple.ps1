# Simple Build Script - Face Recognition Attendance System
# Quick build without extra checks - for experienced developers

param(
    [switch]$SkipRestore,
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

# Configuration
$ProjectPath = "..\src\FaceRecognitionAttendance.csproj"
$OutputPath = ".\publish"

Write-Host "Building Face Recognition Attendance System (.NET 8.0)..." -ForegroundColor Cyan

# Clean
if (Test-Path $OutputPath) {
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Restore (unless skipped)
if (-not $SkipRestore) {
    Write-Host "Restoring packages..." -ForegroundColor Yellow
    dotnet restore $ProjectPath
    if ($LASTEXITCODE -ne 0) { exit 1 }
}

# Publish
Write-Host "Publishing single-file EXE..." -ForegroundColor Yellow
$verbosityLevel = if ($Verbose) { "detailed" } else { "minimal" }

dotnet publish $ProjectPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -p:TrimMode=partial `
    -p:PublishReadyToRun=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o $OutputPath `
    -v $verbosityLevel

if ($LASTEXITCODE -eq 0) {
    $ExePath = Join-Path $OutputPath "FaceRecognitionAttendance.exe"
    $FileSize = [math]::Round((Get-Item $ExePath).Length / 1MB, 2)
    Write-Host ""
    Write-Host "✓ Build successful!" -ForegroundColor Green
    Write-Host "  Location: $ExePath" -ForegroundColor White
    Write-Host "  Size: $FileSize MB" -ForegroundColor White
} else {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}
