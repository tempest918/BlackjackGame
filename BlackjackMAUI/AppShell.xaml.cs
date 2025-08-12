using Plugin.Maui.Audio;

namespace MyBlackjackMAUI
{
    public partial class AppShell : Shell
    {
        public static IAudioPlayer BgmPlayer { get; private set; }

        public AppShell(IAudioManager audioManager)
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));

            // Start BGM
            _ = InitializeBgm(audioManager);
        }

        private async Task InitializeBgm(IAudioManager audioManager)
        {
            if (BgmPlayer is not null) return;

            BgmPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("bgm.mp3"));
            BgmPlayer.Loop = true;
            BgmPlayer.Volume = Settings.BgmVolume;
            BgmPlayer.Play();
        }
    }
}
