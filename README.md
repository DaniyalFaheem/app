# Face Recognition Attendance Management System

A production-ready, standalone desktop application for automated attendance management using real-time face recognition technology.

## 🌟 Features

### Core Functionality
- ✅ **Real-time Face Recognition** using webcam with OpenCV DNN
- ✅ **User Registration** with 80-image face capture and quality checks
- ✅ **Student & Faculty Management** with separate profiles
- ✅ **Automatic Attendance Logging** to CSV files with 5-minute cooldown
- ✅ **Absentee Detection** with date/department filtering
- ✅ **WhatsApp Alert System** via web. whatsapp.com integration
- ✅ **Salary Calculator** with 3 types (Monthly, Fixed, Per-Day)
- ✅ **Role-Based Access Control** (Admin vs User panels)
- ✅ **Modern GUI** with live video feed and color-coded detection
- ✅ **Edit User Feature** (NEW) - Complete user information editing with audit trail

### Technical Highlights
- **Single Executable**: ~140MB standalone . exe, no dependencies required
- **AI Models Embedded**: OpenCV DNN with face detection/recognition models
- **Performance**: Face detection <30ms, Recognition <2 seconds
- **Security**: Encrypted SQLite database, BCrypt password hashing, session management
- **Deployment**: Works on clean Windows 10/11 out-of-the-box

## 🎯 Technology Stack

| Component | Technology |
|-----------|------------|
| **Language** | C# 10 with . NET 6 |
| **Framework** | WPF with ModernWPF theme |
| **Face Recognition** | OpenCvSharp4 + DNN module |
| **Database** | SQLite with Entity Framework Core |
| **Security** | BCrypt. Net for password hashing |
| **CSV Export** | CsvHelper library |
| **Camera** | OpenCV VideoCapture |

## 📁 Project Structure
