# Build Verification Script - Face Recognition Attendance System
# Checks that the build meets all commercial-grade requirements

param(
    [Parameter(Mandatory=$false)]
    [string]$ExePath = ".\publish\FaceRecognitionAttendance.exe"
)

$ErrorActionPreference = "Continue"
$failures = 0

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Verification Script" -ForegroundColor Cyan
Write-Host "Face Recognition Attendance System" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check 1: EXE exists
Write-Host "[1/8] Checking if EXE exists..." -ForegroundColor Yellow
if (Test-Path $ExePath) {
    Write-Host "  ✓ EXE found: $ExePath" -ForegroundColor Green
} else {
    Write-Host "  ✗ EXE not found: $ExePath" -ForegroundColor Red
    $failures++
}

# Check 2: File size
Write-Host "[2/8] Checking file size..." -ForegroundColor Yellow
if (Test-Path $ExePath) {
    $fileSize = (Get-Item $ExePath).Length / 1MB
    Write-Host "  → Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor White
    
    if ($fileSize -gt 0 -and $fileSize -lt 300) {
        Write-Host "  ✓ File size is acceptable" -ForegroundColor Green
    } elseif ($fileSize -ge 300) {
        Write-Host "  ⚠ Warning: File size is large (>300MB)" -ForegroundColor Yellow
    } else {
        Write-Host "  ✗ Invalid file size" -ForegroundColor Red
        $failures++
    }
}

# Check 3: Digital signature (optional but recommended)
Write-Host "[3/8] Checking digital signature..." -ForegroundColor Yellow
if (Test-Path $ExePath) {
    try {
        $signature = Get-AuthenticodeSignature -FilePath $ExePath
        if ($signature.Status -eq "Valid") {
            Write-Host "  ✓ File is digitally signed" -ForegroundColor Green
        } else {
            Write-Host "  ⚠ File is not signed (optional for testing)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ⚠ Could not verify signature" -ForegroundColor Yellow
    }
}

# Check 4: .NET version embedded
Write-Host "[4/8] Checking .NET runtime embedding..." -ForegroundColor Yellow
if (Test-Path $ExePath) {
    # Single-file exes should be >100MB due to embedded runtime
    if ($fileSize -gt 100) {
        Write-Host "  ✓ Likely contains embedded .NET runtime" -ForegroundColor Green
    } else {
        Write-Host "  ✗ File may not be self-contained" -ForegroundColor Red
        $failures++
    }
}

# Check 5: AI Models directory
Write-Host "[5/8] Checking AI models..." -ForegroundColor Yellow
$modelsPath = "..\src\Resources\Models"
$deployProto = Join-Path $modelsPath "deploy.prototxt"
$caffeModel = Join-Path $modelsPath "res10_300x300_ssd_iter_140000.caffemodel"

$modelsFound = 0
if (Test-Path $deployProto) {
    Write-Host "  ✓ deploy.prototxt found" -ForegroundColor Green
    $modelsFound++
} else {
    Write-Host "  ⚠ deploy.prototxt not found (download required)" -ForegroundColor Yellow
}

if (Test-Path $caffeModel) {
    Write-Host "  ✓ caffemodel found" -ForegroundColor Green
    $modelsFound++
} else {
    Write-Host "  ⚠ caffemodel not found (download required)" -ForegroundColor Yellow
}

if ($modelsFound -eq 0) {
    Write-Host "  ⚠ No AI models found - face detection may not work" -ForegroundColor Yellow
}

# Check 6: Project file configuration
Write-Host "[6/8] Checking project configuration..." -ForegroundColor Yellow
$projectFile = "..\src\FaceRecognitionAttendance.csproj"
if (Test-Path $projectFile) {
    $content = Get-Content $projectFile -Raw
    
    $checks = @{
        "net8.0-windows" = $content -match "net8\.0-windows"
        "PublishSingleFile" = $content -match "PublishSingleFile.*true"
        "SelfContained" = $content -match "SelfContained.*true"
        "PublishTrimmed" = $content -match "PublishTrimmed.*true"
    }
    
    $allPassed = $true
    foreach ($check in $checks.GetEnumerator()) {
        if ($check.Value) {
            Write-Host "  ✓ $($check.Key) configured" -ForegroundColor Green
        } else {
            Write-Host "  ✗ $($check.Key) not configured" -ForegroundColor Red
            $failures++
            $allPassed = $false
        }
    }
} else {
    Write-Host "  ✗ Project file not found" -ForegroundColor Red
    $failures++
}

# Check 7: Documentation files
Write-Host "[7/8] Checking documentation..." -ForegroundColor Yellow
$docs = @(
    "..\README.md",
    "..\DEPLOYMENT_GUIDE.md",
    "..\QUICK_START.md",
    "..\UPGRADE_NOTES.md"
)

$docsFound = 0
foreach ($doc in $docs) {
    if (Test-Path $doc) {
        $docsFound++
    }
}

if ($docsFound -eq $docs.Count) {
    Write-Host "  ✓ All documentation files present ($docsFound/$($docs.Count))" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Some documentation missing ($docsFound/$($docs.Count))" -ForegroundColor Yellow
}

# Check 8: Build outputs directory structure
Write-Host "[8/8] Checking build output structure..." -ForegroundColor Yellow
$publishDir = ".\publish"
if (Test-Path $publishDir) {
    $fileCount = (Get-ChildItem $publishDir -File).Count
    
    if ($fileCount -eq 1) {
        Write-Host "  ✓ Clean single-file output (1 file)" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Multiple files in output ($fileCount files)" -ForegroundColor Yellow
        Write-Host "    Note: Single-file may extract temp files at runtime" -ForegroundColor Gray
    }
} else {
    Write-Host "  ✗ Publish directory not found" -ForegroundColor Red
    $failures++
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verification Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($failures -eq 0) {
    Write-Host "✓ All critical checks passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Build is ready for deployment." -ForegroundColor Green
    Write-Host "Recommended next steps:" -ForegroundColor Cyan
    Write-Host "  1. Test on a clean Windows VM" -ForegroundColor White
    Write-Host "  2. Verify camera access works" -ForegroundColor White
    Write-Host "  3. Test face recognition functionality" -ForegroundColor White
    Write-Host "  4. Create release package" -ForegroundColor White
    exit 0
} else {
    Write-Host "✗ $failures critical issue(s) found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please fix the issues above before deployment." -ForegroundColor Red
    exit 1
}
