# 🚀 Quick Start Guide - Face Recognition Attendance System

## For End Users (No Technical Knowledge Required)

### Step 1: Download
1. Download `FaceRecognitionAttendance.exe` from the releases page
2. Save it to any folder on your Windows PC (e.g., Desktop, Documents)

### Step 2: Run
1. **Double-click** the `FaceRecognitionAttendance.exe` file
2. Wait 3-5 seconds for the application to start
3. **No installation needed!** The EXE runs directly

### Step 3: First Login
1. Use the default admin credentials:
   - **Username:** `admin`
   - **Password:** `admin123`
2. ✅ **Important:** Change the password immediately after first login

### Step 4: Grant Camera Permission
1. Windows will ask for camera permission
2. Click **"Allow"** to enable face recognition
3. This is required for the face detection features

---

## Common Questions

### Q: Do I need to install anything?
**A: NO!** This is a 100% self-contained application. No .NET runtime, no Visual Studio, no dependencies needed.

### Q: What if I don't have .NET installed?
**A: That's fine!** Everything is included in the EXE file.

### Q: Will it work on a fresh Windows installation?
**A: Yes!** Works on clean Windows 10 (version 1809+) and Windows 11 systems.

### Q: Do I need Administrator rights?
**A: No.** Runs as a normal user. Admin rights are NOT required.

### Q: Why is the file so large (~150MB)?
**A: Complete self-containment.** Includes .NET runtime, OpenCV libraries, and all dependencies.

### Q: Is internet required?
**A: No.** Works completely offline (except for WhatsApp alerts feature).

### Q: Where is my data stored?
**A:** 
- Database: `C:\Users\YourName\AppData\Roaming\FaceRecognitionAttendance\`
- Face images: Same location in `FaceImages` folder
- CSV exports: Same location in `Exports` folder

### Q: Can I move the EXE to a USB drive?
**A: Yes!** Copy it anywhere. Data stays in AppData regardless.

---

## System Requirements

### Minimum
- Windows 10 version 1809 or higher
- 4 GB RAM
- 500 MB free disk space
- Webcam (USB or built-in)

### Recommended
- Windows 11
- 8 GB RAM
- 2 GB free disk space
- 720p or better webcam

---

## First-Time Setup

### 1. Change Admin Password
- Login with default credentials
- Go to Settings → Change Password
- Choose a strong password

### 2. Register Your First User
- Click "Register New User"
- Fill in details (Name, Phone, Email, Department)
- Follow face capture instructions
- System captures 80 images automatically

### 3. Start Attendance Tracking
- Click "Start Attendance"
- Face camera
- System automatically recognizes and logs attendance

---

## Troubleshooting

### Camera Not Working
1. Check camera permissions: Windows Settings → Privacy → Camera
2. Allow access for desktop apps
3. Restart the application

### Face Not Recognized
1. Ensure good lighting (front-lit, not backlit)
2. Face the camera directly
3. Remove glasses if accuracy is low
4. Try re-registering with better quality images

### Application Won't Start
1. Verify Windows version (must be 1809 or higher)
2. Check antivirus isn't blocking it
3. Right-click EXE → Properties → Unblock (if present)

### Database Errors
1. Ensure you have write permissions in AppData folder
2. Check free disk space (need at least 100MB)
3. Delete database to start fresh (will lose all data)

---

## Features Overview

### For Admins

**User Management**
- Register students and faculty
- Edit user information
- View/export user lists
- Deactivate users

**Attendance**
- Real-time face recognition
- Automatic attendance marking
- 5-minute cooldown prevents duplicates
- Export to CSV

**Reporting**
- View attendance by date/department
- Detect absentees
- Salary calculations (faculty)
- CSV exports for Excel

**Notifications**
- WhatsApp alerts for absentees
- Bulk messaging support

### For Regular Users
- View own attendance record
- Check attendance history
- Export personal attendance data

---

## Security Features

✅ Password protection with BCrypt hashing  
✅ Session timeouts (30 minutes inactive)  
✅ Audit logging for all changes  
✅ Role-based access control  
✅ Encrypted database storage  

---

## Support

**Documentation**
- Full README: See README.md
- Developer Guide: See DEPLOYMENT_GUIDE.md
- Setup Instructions: See SETUP_GUIDE.md

**Issues**
- Report bugs: [GitHub Issues](https://github.com/DaniyalFaheem/app/issues)
- Feature requests: [GitHub Discussions](https://github.com/DaniyalFaheem/app/discussions)

**Contact**
- Developer: Daniyal Faheem
- Repository: [github.com/DaniyalFaheem/app](https://github.com/DaniyalFaheem/app)

---

## Version Information

**Current Version:** 2.0.0  
**Release Date:** 2025-12-03  
**Framework:** .NET 8.0  
**License:** MIT  

---

**Enjoy using Face Recognition Attendance System! 🎓**
