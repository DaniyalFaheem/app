using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FaceRecognitionAttendance.Data;
using FaceRecognitionAttendance.Data.Repositories;
using FaceRecognitionAttendance.Services.Authentication;
using FaceRecognitionAttendance.Services.Attendance;
using FaceRecognitionAttendance.Services.Camera;
using FaceRecognitionAttendance.Services.FaceRecognition;
using FaceRecognitionAttendance.Services.Notification;
using FaceRecognitionAttendance.Services.Salary;

namespace FaceRecognitionAttendance
{
    /// <summary>
    /// Application entry point with dependency injection setup
    /// Commercial-grade error handling and reliability improvements for .NET 8.0
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private static string? _logFilePath;

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Setup global exception handlers for maximum reliability
            SetupExceptionHandlers();

            try
            {
                base.OnStartup(e);

                // Setup application paths
                SetupApplicationPaths();

                // Configure services
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                _serviceProvider = serviceCollection.BuildServiceProvider();

                // Initialize database
                await InitializeDatabaseAsync();

                // Log successful startup
                LogMessage("Application started successfully");

                // Show main window (will be LoginWindow as per StartupUri)
            }
            catch (Exception ex)
            {
                HandleStartupException(ex);
            }
        }

        /// <summary>
        /// Setup global exception handlers for unhandled exceptions
        /// Ensures application doesn't crash silently and provides error reporting
        /// </summary>
        private void SetupExceptionHandlers()
        {
            // Handle UI thread exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Handle non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Handle task exceptions
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException("UI Thread Exception", e.Exception);
            
            MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\nThe error has been logged. Please contact support if this persists.",
                "Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true; // Prevent application crash
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException("Domain Unhandled Exception", ex);
                
                MessageBox.Show(
                    $"A critical error occurred:\n\n{ex.Message}\n\nThe application will now close.",
                    "Critical Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            LogException("Task Exception", e.Exception);
            e.SetObserved(); // Prevent process termination
        }

        private void HandleStartupException(Exception ex)
        {
            LogException("Startup Exception", ex);
            
            MessageBox.Show(
                $"Failed to start the application:\n\n{ex.Message}\n\nPlease check the log file for details.",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            Shutdown(1);
        }

        private void SetupApplicationPaths()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "FaceRecognitionAttendance");
            
            Directory.CreateDirectory(appFolder);
            Directory.CreateDirectory(Path.Combine(appFolder, "FaceImages"));
            Directory.CreateDirectory(Path.Combine(appFolder, "Exports"));
            Directory.CreateDirectory(Path.Combine(appFolder, "Logs"));

            // Initialize log file path
            _logFilePath = Path.Combine(appFolder, "Logs", $"app_{DateTime.Now:yyyyMMdd}.log");
        }

        /// <summary>
        /// Log informational messages for debugging and monitoring
        /// </summary>
        private static void LogMessage(string message)
        {
            try
            {
                if (_logFilePath == null) return;

                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}{Environment.NewLine}";
                File.AppendAllText(_logFilePath, logEntry);
            }
            catch (Exception)
            {
                // Silently fail if logging fails (don't crash due to logging)
            }
        }

        /// <summary>
        /// Log exceptions with full stack trace for troubleshooting
        /// </summary>
        private static void LogException(string context, Exception ex)
        {
            try
            {
                if (_logFilePath == null)
                {
                    var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    var logFolder = Path.Combine(appDataPath, "FaceRecognitionAttendance", "Logs");
                    Directory.CreateDirectory(logFolder);
                    _logFilePath = Path.Combine(logFolder, $"app_{DateTime.Now:yyyyMMdd}.log");
                }

                var sb = new StringBuilder();
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {context}");
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine($"Type: {ex.GetType().FullName}");
                sb.AppendLine($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
                    sb.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }
                
                sb.AppendLine(new string('-', 80));

                File.AppendAllText(_logFilePath, sb.ToString());
            }
            catch (Exception)
            {
                // Silently fail if logging fails
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbPath = Path.Combine(appDataPath, "FaceRecognitionAttendance", "attendance.db");
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();

            // Services
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
            services.AddScoped<IFaceRecognitionService, FaceRecognitionService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<ICsvExportService, CsvExportService>();
            services.AddScoped<ISalaryCalculator, SalaryCalculator>();
            services.AddScoped<IWhatsAppService, WhatsAppService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<AbsenteeDetector>();

            // Database initializer
            services.AddScoped<DatabaseInitializer>();
        }

        private async System.Threading.Tasks.Task InitializeDatabaseAsync()
        {
            using var scope = _serviceProvider?.CreateScope();
            var initializer = scope?.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            
            if (initializer != null)
            {
                await initializer.InitializeAsync();
            }
        }

        public static ServiceProvider? ServiceProvider => ((App)Current)._serviceProvider;

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                LogMessage("Application shutting down");
                _serviceProvider?.Dispose();
            }
            catch (Exception ex)
            {
                LogException("Shutdown Exception", ex);
            }
            finally
            {
                base.OnExit(e);
            }
        }
    }
}