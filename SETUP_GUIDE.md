# 🚀 Complete Setup Guide - Face Recognition Attendance System

## System Requirements

### Essential Requirements
- ✅ **Windows 10 or 11** (64-bit) - WPF only works on Windows
- ✅ **.NET 6 SDK** - Required to build and run the application
- ✅ **Webcam** - Required for face detection and recognition
- ✅ **8 GB RAM** (minimum) - For OpenCV and face recognition
- ✅ **500 MB free disk space** - For application and database

### Optional (for development)
- Visual Studio 2022 or later
- Git for version control

---

## Step 1: Install .NET 6 SDK

### Check if .NET 6 is already installed:
```powershell
# Open PowerShell or Command Prompt and run:
dotnet --version
```

If you see version `6.0.x`, you're good! If not:

### Install .NET 6 SDK:

1. **Download** from Microsoft:
   - Go to: https://dotnet.microsoft.com/download/dotnet/6.0
   - Click **"Download .NET SDK x64"** (Windows)
   - File name: `dotnet-sdk-6.0.xxx-win-x64.exe`

2. **Install**:
   - Double-click the downloaded `.exe` file
   - Follow the installation wizard (default settings are fine)
   - Installation takes 2-3 minutes

3. **Verify**:
   ```powershell
   # Close and reopen PowerShell/Command Prompt
   dotnet --version
   # Should show: 6.0.xxx
   ```

---

## Step 2: Clone the Repository

### Option A: Using Git (Recommended)

1. **Install Git** (if not installed):
   - Download from: https://git-scm.com/download/win
   - Run installer with default settings

2. **Clone the repository**:
   ```powershell
   # Open PowerShell
   # Navigate to where you want the project
   cd C:\Projects
   
   # Clone the repository
   git clone https://github.com/DaniyalFaheem/app.git
   
   # Enter the project directory
   cd app
   ```

### Option B: Download ZIP (Alternative)

1. Go to: https://github.com/DaniyalFaheem/app
2. Click green **"Code"** button → **"Download ZIP"**
3. Extract the ZIP file to `C:\Projects\app` (or your preferred location)
4. Open PowerShell and navigate:
   ```powershell
   cd C:\Projects\app
   ```

---

## Step 3: Download AI Model Files (IMPORTANT!)

The AI models are **NOT** included in the repository due to their size. You must download them separately.

### Required Model Files:

1. **Create the Models directory**:
   ```powershell
   # From the project root (C:\Projects\app)
   mkdir src\Resources\Models
   ```

2. **Download these two files**:
   
   **File 1: deploy.prototxt**
   - URL: https://github.com/opencv/opencv/blob/master/samples/dnn/face_detector/deploy.prototxt
   - Click "Raw" button, then Ctrl+S to save
   - Save as: `deploy.prototxt`
   
   **File 2: res10_300x300_ssd_iter_140000.caffemodel**
   - URL: https://github.com/opencv/opencv_3rdparty/raw/dnn_samples_face_detector_20170830/res10_300x300_ssd_iter_140000.caffemodel
   - Click link to download (file is ~10 MB)
   - Save as: `res10_300x300_ssd_iter_140000.caffemodel`

3. **Place both files** in:
   ```
   C:\Projects\app\src\Resources\Models\
   ```

4. **Verify**:
   ```powershell
   dir src\Resources\Models
   # You should see both .prototxt and .caffemodel files
   ```

---

## Step 4: Build the Application

### Method 1: Using PowerShell (Simple)

1. **Open PowerShell** in the project directory:
   ```powershell
   cd C:\Projects\app
   ```

2. **Restore NuGet packages**:
   ```powershell
   dotnet restore src/FaceRecognitionAttendance.csproj
   ```
   This downloads all required libraries (takes 1-2 minutes first time)

3. **Build the application**:
   ```powershell
   dotnet build src/FaceRecognitionAttendance.csproj -c Release
   ```
   This compiles the code (takes 30-60 seconds)

4. **Check for success**:
   - Look for: `Build succeeded. 0 Warning(s). 0 Error(s)`
   - If you see errors, see Troubleshooting section below

### Method 2: Using the Build Script (Automated)

```powershell
cd C:\Projects\app\build
.\publish.ps1
```

This will:
- Restore packages
- Build the application
- Create a single-file executable in `publish\` folder

---

## Step 5: Run the Application

### Method 1: Run from source (for development)

```powershell
cd C:\Projects\app
dotnet run --project src/FaceRecognitionAttendance.csproj
```

The application window should open in a few seconds.

### Method 2: Run the executable (after using build script)

```powershell
cd C:\Projects\app\publish
.\FaceRecognitionAttendance.exe
```

### Method 3: Using Visual Studio (if installed)

1. Open Visual Studio 2022
2. File → Open → Project/Solution
3. Navigate to `C:\Projects\app\src\`
4. Select `FaceRecognitionAttendance.csproj`
5. Press **F5** to run

---

## Step 6: First-Time Login

When the application starts:

1. **Default Login Credentials**:
   - Username: `admin`
   - Password: `admin123`

2. **After login**:
   - Change the default password (recommended for security)
   - Make sure your webcam is connected and working

3. **Grant Camera Permissions**:
   - Windows may ask for camera permissions
   - Click "Allow" to enable face detection

---

## 📁 Expected Directory Structure

After setup, your project should look like this:

```
C:\Projects\app\
├── src/
│   ├── Resources/
│   │   ├── Images/
│   │   │   ├── background.jpg      ✅ From this PR
│   │   │   └── send_button.png     ✅ From this PR
│   │   └── Models/
│   │       ├── deploy.prototxt                        ✅ Downloaded
│   │       └── res10_300x300_ssd_iter_140000.caffemodel  ✅ Downloaded
│   ├── Views/
│   ├── Models/
│   ├── Services/
│   ├── App.xaml
│   └── FaceRecognitionAttendance.csproj
├── build/
│   └── publish.ps1
└── README.md
```

---

## 🐛 Troubleshooting

### Error: "The target framework 'net6.0-windows' requires platform 'Windows'"

**Solution**: This is normal on non-Windows systems. The application **MUST** be built and run on Windows.

### Error: "Could not find face detection model"

**Solution**: Make sure you downloaded the AI model files (Step 3) and placed them in `src/Resources/Models/`

### Error: "Camera not found" or "Cannot access camera"

**Solutions**:
1. Make sure a webcam is connected
2. Check Windows camera permissions:
   - Settings → Privacy → Camera
   - Enable "Allow apps to access your camera"
3. Close other apps using the camera (Zoom, Teams, etc.)

### Error: "dotnet: The term 'dotnet' is not recognized"

**Solution**: 
1. Restart PowerShell after installing .NET SDK
2. If still not working, reinstall .NET SDK and reboot your computer

### Build errors about missing packages

**Solution**:
```powershell
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore src/FaceRecognitionAttendance.csproj --force
dotnet build src/FaceRecognitionAttendance.csproj -c Release
```

### Application crashes on startup

**Solutions**:
1. Check that AI model files are in the correct location
2. Make sure .NET 6 SDK is fully installed
3. Try running as Administrator:
   - Right-click PowerShell → "Run as Administrator"
   - Navigate to project and run again

---

## 🎨 What's New in This PR?

The images you uploaded are now integrated:

1. **Background Image**: 
   - Futuristic tech-themed background visible in User Management and Edit User windows
   - Applied with subtle opacity (12-15%) so text remains readable

2. **Send Button**: 
   - Custom green "SEND" button style available throughout the app
   - Can be used for WhatsApp alerts and other send actions
   - Has interactive states (hover, press, disabled)

---

## 📝 Quick Command Reference

```powershell
# Navigate to project
cd C:\Projects\app

# Restore packages
dotnet restore src/FaceRecognitionAttendance.csproj

# Build
dotnet build src/FaceRecognitionAttendance.csproj -c Release

# Run
dotnet run --project src/FaceRecognitionAttendance.csproj

# Build single-file executable
cd build
.\publish.ps1
```

---

## 🆘 Need More Help?

1. **Check the main README**: `README.md` has detailed feature documentation
2. **Image usage guide**: `IMAGES_USAGE.md` shows how to use the new images
3. **Report issues**: https://github.com/DaniyalFaheem/app/issues

---

## ✅ Checklist for First-Time Setup

- [ ] Windows 10/11 installed
- [ ] .NET 6 SDK installed and verified (`dotnet --version`)
- [ ] Repository cloned or downloaded
- [ ] AI model files downloaded to `src/Resources/Models/`
- [ ] Packages restored (`dotnet restore`)
- [ ] Application built successfully (`dotnet build`)
- [ ] Application runs without errors
- [ ] Webcam connected and permissions granted
- [ ] Logged in with default credentials
- [ ] Background images visible in UI ✨

---

**Congratulations! 🎉** Your Face Recognition Attendance System is now ready to use!
