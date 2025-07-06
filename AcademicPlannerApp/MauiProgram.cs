using Microsoft.Extensions.Logging;
using AcademicPlannerApp.ViewModels; // Tendrás que crear estos más tarde
using AcademicPlannerApp.Services;   // Tendrás que crear estos más tarde (DialogService)
using AcademicPlannerApp.Data.Services; // Para DataAccessService
using Microsoft.Maui.Storage; // Para FileSystem
using System.IO; // Para Path

namespace AcademicPlannerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // --- Configuración clave para SQLite directo ---
        // Ruta de la base de datos para SQLite
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AcademicPlanner.db");
        // Registra tu DataAccessService como un singleton, pasándole la ruta de la DB
        builder.Services.AddSingleton<DataAccessService>(s => new DataAccessService(dbPath));

        // --- Registro de Servicios de UI y ViewModels (crearemos estos en los siguientes pasos) ---
        // Estos son placeholders por ahora
        builder.Services.AddSingleton<IDialogService, DialogService>(); // Para popups, etc.
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>(); // Tu página principal

        // Opcional: para depuración (se activa solo en modo Debug)
#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }
}