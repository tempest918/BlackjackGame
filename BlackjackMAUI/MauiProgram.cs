using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MyBlackjackMAUI.Services;
using Plugin.Maui.Audio;
using Microcharts.Maui;

namespace MyBlackjackMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialSymbolsOutlined-Regular.ttf", "MaterialIcons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<BgmManagerService>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<BlackjackLogic.BlackjackGameLogic>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<StatsPage>();
            builder.Services.AddTransient(serviceProvider =>
            {
                var bgmManager = serviceProvider.GetRequiredService<BgmManagerService>();
#if ANDROID
                var deviceInfoService = serviceProvider.GetRequiredService<IDeviceInfoService>();
#else
                IDeviceInfoService deviceInfoService = null;
#endif
                return new SettingsPage(bgmManager, deviceInfoService);
            });
            builder.Services.AddTransient<TitlePage>();
            builder.Services.AddTransient<HistoryPage>();

#if ANDROID
            builder.Services.AddSingleton<IDeviceInfoService, Platforms.Android.DeviceInfoService>();
#endif

            return builder.Build();
        }
    }
}
