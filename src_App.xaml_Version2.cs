using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft. Extensions.DependencyInjection;
using FaceRecognitionAttendance. Data;
using FaceRecognitionAttendance.Data. Repositories;
using FaceRecognitionAttendance.Services.Authentication;
using FaceRecognitionAttendance.Services. Attendance;
using FaceRecognitionAttendance.Services.Camera;
using FaceRecognitionAttendance.Services.FaceRecognition;
using FaceRecognitionAttendance.Services.Notification;
using FaceRecognitionAttendance.Services.Salary;

namespace FaceRecognitionAttendance
{
    /// <summary>
    /// Application entry point with dependency injection setup
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
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

            // Show main window (will be LoginWindow as per StartupUri)
        }

        private void SetupApplicationPaths()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "FaceRecognitionAttendance");
            
            Directory.CreateDirectory(appFolder);
            Directory.CreateDirectory(Path.Combine(appFolder, "FaceImages"));
            Directory.CreateDirectory(Path.Combine(appFolder, "Exports"));
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
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}