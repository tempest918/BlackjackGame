using MyBlackjackMAUI.Services;
using Plugin.Maui.Audio;

namespace MyBlackjackMAUI
{
    public partial class AppShell : Shell
    {
        public AppShell(BgmManagerService bgmManager)
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));

            // Start BGM
            _ = StartBgm(bgmManager);
        }

        private async Task StartBgm(BgmManagerService bgmManager)
        {
            await bgmManager.InitializeAsync();
            bgmManager.SetVolume(Settings.BgmVolume);
            bgmManager.Play();
        }
    }
}
