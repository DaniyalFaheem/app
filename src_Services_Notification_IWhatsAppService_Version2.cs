using FaceRecognitionAttendance. Models;
using System;
using System.Collections.Generic;

namespace FaceRecognitionAttendance.Services.Notification
{
    /// <summary>
    /// WhatsApp notification service interface
    /// </summary>
    public interface IWhatsAppService
    {
        /// <summary>
        /// Send absentee alert to user
        /// </summary>
        void SendAbsenteeAlert(User user, DateTime date);

        /// <summary>
        /// Send absentee alerts to multiple users
        /// </summary>
        void SendBatchAbsenteeAlerts(List<User> users, DateTime date, int delaySeconds = 2);
    }
}