using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace MyBlackjackMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
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
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<BlackjackLogic.BlackjackGameLogic>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<StatsPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<TitlePage>();

            return builder.Build();
        }
    }
}
