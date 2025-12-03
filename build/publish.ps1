# Face Recognition Attendance System - Build Script (.NET 8.0)
# This script builds a commercial-grade, self-contained single EXE file

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Face Recognition Attendance System" -ForegroundColor Cyan
Write-Host ".NET 8.0 Commercial Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ProjectPath = "..\src\FaceRecognitionAttendance.csproj"
$OutputPath = ".\publish"
$Configuration = "Release"
$Runtime = "win-x64"

# Verify .NET 8.0 SDK is installed
Write-Host "Checking .NET version..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
Write-Host "Detected .NET SDK: $dotnetVersion" -ForegroundColor Green

if (-not ($dotnetVersion -like "8.*")) {
    Write-Host "Warning: .NET 8.0 SDK not detected. Build may fail." -ForegroundColor Yellow
    Write-Host "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne "y") {
        exit 1
    }
}
Write-Host ""

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

# Publish as single-file executable with .NET 8.0 optimizations
Write-Host "Publishing as single-file executable with maximum optimizations..." -ForegroundColor Yellow
Write-Host "Configuration: Self-contained, trimmed, compressed, ReadyToRun" -ForegroundColor Gray
Write-Host ""

dotnet publish $ProjectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -p:TrimMode=partial `
    -p:PublishReadyToRun=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=embedded `
    -p:DebugSymbols=true `
    -p:IlcOptimizationPreference=Speed `
    -p:Deterministic=true `
    -o $OutputPath `
    -v minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    Write-Host "Please check the error messages above and ensure all dependencies are available." -ForegroundColor Red
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
    
    if ($FileSize -lt 200) {
        Write-Host "✓ File size is within acceptable limits (<200MB)" -ForegroundColor Green
    } else {
        Write-Host "⚠ Warning: File size exceeds 200MB - Consider additional trimming" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "Build Details:" -ForegroundColor Cyan
    Write-Host "  - .NET Version: 8.0" -ForegroundColor White
    Write-Host "  - Runtime: Self-contained win-x64" -ForegroundColor White
    Write-Host "  - Optimizations: ReadyToRun, Trimmed, Compressed" -ForegroundColor White
    Write-Host "  - Single File: Yes (all dependencies embedded)" -ForegroundColor White
    Write-Host "  - Debug Symbols: Embedded" -ForegroundColor White
    
} else {
    Write-Host "Executable not found!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Test the executable on a clean Windows 10/11 installation" -ForegroundColor White
Write-Host "2. Verify camera access and face recognition functionality" -ForegroundColor White
Write-Host "3. Ensure AI model files are embedded or in Resources\Models\" -ForegroundColor White
Write-Host "4. Test on multiple machines without .NET runtime installed" -ForegroundColor White
Write-Host ""
Write-Host "The EXE is 100% self-contained and requires NO pre-installation!" -ForegroundColor Green
Write-Host ""