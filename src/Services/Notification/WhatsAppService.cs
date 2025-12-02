using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;
using FaceRecognitionAttendance.Models;

namespace FaceRecognitionAttendance.Services.Notification
{
    /// <summary>
    /// WhatsApp notification service via web. whatsapp.com
    /// </summary>
    public class WhatsAppService : IWhatsAppService
    {
        public void SendAbsenteeAlert(User user, DateTime date)
        {
            try
            {
                var message = $"Dear {user.Name},\n\n" +
                             $"You were marked absent on {date:yyyy-MM-dd}.\n\n" +
                             $"Department: {user.Department}\n" +
                             $"If this is incorrect, please contact the administration immediately.\n\n" +
                             $"Thank you. ";

                var encodedMessage = HttpUtility. UrlEncode(message);
                var phone = user.Phone. Replace("+", ""). Replace(" ", ""). Replace("-", "");
                var url = $"https://web.whatsapp.com/send?phone={phone}&text={encodedMessage}";

                // Open in default browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                Console.WriteLine($"WhatsApp alert sent to {user.Name} ({user.Phone})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending WhatsApp alert: {ex.Message}");
            }
        }

        public void SendBatchAbsenteeAlerts(List<User> users, DateTime date, int delaySeconds = 2)
        {
            foreach (var user in users)
            {
                SendAbsenteeAlert(user, date);
                
                // Delay between messages to avoid overwhelming the system
                if (users.IndexOf(user) < users.Count - 1)
                {
                    Thread.Sleep(delaySeconds * 1000);
                }
            }
        }
    }
}