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
            Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));

            GlobalBgmPlayer = this.BgmPlayer;
            if (GlobalBgmPlayer is not null)
            {
                GlobalBgmPlayer.Volume = Settings.BgmVolume;
            }
        }
    }
}
