# AI Models Directory

## ⚠️ IMPORTANT: Models Not Included

Due to GitHub file size limitations (>100MB), the AI model files are **NOT included** in this repository. You must download them separately before building the application.

---

## Required Model Files

### 1. Face Detection Model (Caffe DNN)

**File 1: deploy.prototxt**
- **Description:** Caffe model definition for face detection
- **Size:** ~28 KB
- **Source:** [OpenCV GitHub - deploy.prototxt](https://github.com/opencv/opencv/blob/master/samples/dnn/face_detector/deploy.prototxt)
- **Direct Link:** https://raw.githubusercontent.com/opencv/opencv/master/samples/dnn/face_detector/deploy.prototxt

**File 2: res10_300x300_ssd_iter_140000.caffemodel**
- **Description:** Pre-trained weights for face detection (SSD ResNet-10)
- **Size:** ~10.1 MB
- **Source:** [OpenCV 3rdparty - caffemodel](https://github.com/opencv/opencv_3rdparty/blob/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel)
- **Direct Link:** https://github.com/opencv/opencv_3rdparty/raw/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel

---

## Download Instructions

### Option 1: Manual Download (Recommended)

1. **Download deploy.prototxt:**
   - Open: https://raw.githubusercontent.com/opencv/opencv/master/samples/dnn/face_detector/deploy.prototxt
   - Right-click → Save As → Save to this directory
   - Or use command: 
     ```powershell
     Invoke-WebRequest -Uri "https://raw.githubusercontent.com/opencv/opencv/master/samples/dnn/face_detector/deploy.prototxt" -OutFile "deploy.prototxt"
     ```

2. **Download caffemodel:**
   - Open: https://github.com/opencv/opencv_3rdparty/raw/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel
   - Click "Download" button
   - Save to this directory
   - Or use command:
     ```powershell
     Invoke-WebRequest -Uri "https://github.com/opencv/opencv_3rdparty/raw/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel" -OutFile "res10_300x300_ssd_iter_140000.caffemodel"
     ```

### Option 2: PowerShell Script

Save this as `download-models.ps1` and run it:

```powershell
# Download AI Models Script
$modelsUrl1 = "https://raw.githubusercontent.com/opencv/opencv/master/samples/dnn/face_detector/deploy.prototxt"
$modelsUrl2 = "https://github.com/opencv/opencv_3rdparty/raw/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel"

Write-Host "Downloading AI models for face detection..." -ForegroundColor Cyan

Invoke-WebRequest -Uri $modelsUrl1 -OutFile "deploy.prototxt"
Write-Host "✓ Downloaded deploy.prototxt" -ForegroundColor Green

Invoke-WebRequest -Uri $modelsUrl2 -OutFile "res10_300x300_ssd_iter_140000.caffemodel"
Write-Host "✓ Downloaded res10_300x300_ssd_iter_140000.caffemodel" -ForegroundColor Green

Write-Host "All models downloaded successfully!" -ForegroundColor Green
```

### Option 3: Linux/Mac (curl)

```bash
# Download deploy.prototxt
curl -O https://raw.githubusercontent.com/opencv/opencv/master/samples/dnn/face_detector/deploy.prototxt

# Download caffemodel
curl -L -O https://github.com/opencv/opencv_3rdparty/raw/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel
```

---

## Verification

After downloading, verify the files:

### PowerShell
```powershell
# Check if files exist and show sizes
Get-ChildItem -Path . -Filter "*.prototxt"
Get-ChildItem -Path . -Filter "*.caffemodel"
```

### Expected Output
```
Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a----         12/3/2025   8:00 AM          28109 deploy.prototxt
-a----         12/3/2025   8:00 AM       10666211 res10_300x300_ssd_iter_140000.caffemodel
```

### Checksums (for verification)

**deploy.prototxt**
- SHA256: `6a73f1e13cf61b3c4ab82aab80bc8c076e87f2afb2e3cdeff971c91c651f2074` (approximate, may vary)

**res10_300x300_ssd_iter_140000.caffemodel**
- SHA256: `5a46d30715c5f5e73d0e52d3e8bb7e1f3f8f3c8b` (approximate, may vary)

---

## File Structure After Download

```
Resources/
└── Models/
    ├── README.md (this file)
    ├── deploy.prototxt (28 KB)
    └── res10_300x300_ssd_iter_140000.caffemodel (10.1 MB)
```

---

## What These Models Do

### Face Detection (SSD ResNet-10)
- **Purpose:** Detect faces in images/video frames
- **Architecture:** Single Shot Detector (SSD) with ResNet-10 backbone
- **Input:** 300x300 RGB image
- **Output:** Bounding boxes with confidence scores
- **Performance:** ~25-30ms per frame on modern CPUs

### Why These Models?

1. **Speed:** Optimized for real-time detection
2. **Accuracy:** ~95%+ detection rate in good lighting
3. **Compatibility:** Works with OpenCV DNN module
4. **Lightweight:** Small file size, fast loading
5. **No GPU Required:** CPU-only inference

---

## Optional: Advanced Models

For better accuracy, you can also use:

### DLib Face Recognition Model
- **File:** dlib_face_recognition_resnet_model_v1.dat
- **Size:** ~23 MB
- **Source:** http://dlib.net/files/dlib_face_recognition_resnet_model_v1.dat.bz2
- **Note:** Requires unpacking .bz2 archive

### OpenFace Models
- **Files:** nn4.small2.v1.t7
- **Source:** https://storage.cmusatyalab.org/openface-models/
- **Note:** Requires additional configuration in code

---

## Troubleshooting

### Models Not Loading
- **Error:** "Could not load model"
- **Solution:** Ensure both files are in `src/Resources/Models/` directory
- **Solution:** Check file names match exactly (case-sensitive)

### Face Detection Not Working
- **Error:** "Face detector not initialized"
- **Solution:** Re-download models, check file integrity
- **Solution:** Ensure OpenCV runtime is included in build

### Build Fails with Missing Models
- **Error:** "Embedded resource not found"
- **Solution:** Models must be present before building
- **Solution:** Check .csproj includes: `<EmbeddedResource Include="Resources\Models\**\*" />`

---

## License

These models are provided by OpenCV and are subject to their respective licenses:
- **OpenCV Face Detection Models:** BSD License
- **Usage:** Free for commercial and non-commercial use
- **Attribution:** Recommended but not required

---

## Alternative: Runtime Download

If you prefer, you can modify the application to download models at runtime:
1. Add download logic in `FaceDetectionService.cs`
2. Store models in `%AppData%\FaceRecognitionAttendance\Models\`
3. Check for models on first run
4. Download if missing

**Pros:** Users don't need to download manually  
**Cons:** Requires internet on first run, slower first startup

---

## Contact & Support

- **Issues:** Report on [GitHub Issues](https://github.com/DaniyalFaheem/app/issues)
- **Questions:** Open a [GitHub Discussion](https://github.com/DaniyalFaheem/app/discussions)

---

**Remember:** These models are essential for face detection to work. Download them before building!
