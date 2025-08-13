using CommunityToolkit.Maui.MediaElement;

namespace MyBlackjackMAUI
{
    public partial class AppShell : Shell
    {
        public static CommunityToolkit.Maui.MediaElement.MediaElement? GlobalBgmPlayer { get; private set; }
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));

            if(this.Resources.TryGetValue("BgmPlayer", out object player))
            {
                GlobalBgmPlayer = player as MediaElement;
                if(GlobalBgmPlayer is not null)
                {
                    GlobalBgmPlayer.Volume = Settings.BgmVolume;
                }
            }
        }
    }
}
