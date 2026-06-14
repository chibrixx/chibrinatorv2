using System.Windows;
using Chibinator.Core.Services;
using Chibinator.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chibinator.UI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Pre-load settings and profiles before the window opens
        var appSettings = Services.GetRequiredService<AppSettingsService>();
        await appSettings.LoadAsync();

        var profileService = Services.GetRequiredService<ProfileService>();
        await profileService.LoadAllAsync();

        var mainVm = Services.GetRequiredService<MainViewModel>();
        await mainVm.InitializeAsync();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        // Logging
        services.AddLogging(b =>
        {
            b.AddConsole();
            b.SetMinimumLevel(LogLevel.Debug);
        });

        // Core services
        services.AddSingleton<AppSettingsService>();
        services.AddSingleton<RobloxDetectionService>();
        services.AddSingleton<FastFlagService>();
        services.AddSingleton<ProfileService>();
        services.AddSingleton<HardwareDetectionService>();
        services.AddSingleton<LaunchService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<ProfilesViewModel>();
        services.AddTransient<FastFlagsViewModel>();
        services.AddTransient<SettingsViewModel>();
    }
}
