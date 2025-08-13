using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core.Primitives;

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

            GlobalBgmPlayer = new MediaElement
            {
                Source = MediaSource.FromResource("bgm.mp3"),
                ShouldLoopPlayback = true,
                IsVisible = false,
                Volume = Settings.BgmVolume,
                ShouldAutoPlay = true
            };

            playerHolder.Children.Add(GlobalBgmPlayer);
        }
    }
}
