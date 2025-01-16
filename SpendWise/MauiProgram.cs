using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SpendWise.Model;
using SpendWise.Services;
using SpendWise.Services.Interface;

namespace SpendWise
{
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
                });
            builder.Services.AddMauiBlazorWebView();
            // Registers services as a scoped dependency.
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IDebtService, DebtService>();
            builder.Services.AddScoped<IBalanceService, BalanceService>();
            builder.Services.AddScoped<ITagService, TagService>();
            // Adds MudBlazor services to the dependency injection container.
            builder.Services.AddMudServices();
            // Timing configuration of Mud Snack Bar
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.ShowTransitionDuration = 300; // Opening animation duration (ms)
                config.SnackbarConfiguration.HideTransitionDuration = 300; // Closing animation duration (ms)
                config.SnackbarConfiguration.VisibleStateDuration = 3000; // Display duration: 3 seconds
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
