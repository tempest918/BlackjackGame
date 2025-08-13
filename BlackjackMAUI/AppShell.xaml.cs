using CommunityToolkit.Maui.Views;

namespace MyBlackjackMAUI
{
    public partial class AppShell : Shell
    {
        public static MediaElement? GlobalBgmPlayer { get; private set; }
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));

            if (Application.Current.Resources.TryGetValue("BgmPlayer", out object? player) && player is MediaElement bgmPlayer)
            {
                GlobalBgmPlayer = bgmPlayer;
                GlobalBgmPlayer.Volume = Settings.BgmVolume;
                GlobalBgmPlayer.Play();
            }
        }
    }
}
