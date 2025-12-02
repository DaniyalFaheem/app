# Face Recognition Attendance System - Build Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Face Recognition Attendance System" -ForegroundColor Cyan
Write-Host "Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ProjectPath = ". .\src\FaceRecognitionAttendance. csproj"
$OutputPath = ".\publish"
$Configuration = "Release"
$Runtime = "win-x64"

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputPath) {
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore $ProjectPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to restore packages!" -ForegroundColor Red
    exit 1
}

# Build in Release mode
Write-Host "Building in Release mode..." -ForegroundColor Yellow
dotnet build $ProjectPath -c $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Publish as single-file executable
Write-Host "Publishing as single-file executable..." -ForegroundColor Yellow
dotnet publish $ProjectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -p:PublishReadyToRun=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o $OutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

# Get file size
$ExePath = Join-Path $OutputPath "FaceRecognitionAttendance.exe"
if (Test-Path $ExePath) {
    $FileSize = (Get-Item $ExePath).Length / 1MB
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Executable location: $ExePath" -ForegroundColor Cyan
    Write-Host "File size: $([math]::Round($FileSize, 2)) MB" -ForegroundColor Cyan
    Write-Host ""
    
    if ($FileSize -lt 150) {
        Write-Host "✓ File size is within the 150MB requirement" -ForegroundColor Green
    } else {
        Write-Host "⚠ Warning: File size exceeds 150MB requirement" -ForegroundColor Yellow
    }
} else {
    Write-Host "Executable not found!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1.  Test the executable on a clean Windows installation" -ForegroundColor White
Write-Host "2. Download AI model files (see Resources/Models/README.md)" -ForegroundColor White
Write-Host "3.  Create installer using NSIS (installer. nsi)" -ForegroundColor White
Write-Host ""