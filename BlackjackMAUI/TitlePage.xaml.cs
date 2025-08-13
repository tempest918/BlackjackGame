using BlackjackLogic;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace MyBlackjackMAUI;

public partial class TitlePage : ContentPage
{
    private readonly MainPage _mainPage;
    private static MediaElement? _bgmPlayer;

    public TitlePage(MainPage mainPage)
	{
		InitializeComponent();
        _mainPage = mainPage;

        if (_bgmPlayer is null)
        {
            _bgmPlayer = new MediaElement
            {
                Source = MediaSource.FromResource("bgm.mp3"),
                ShouldLoopPlayback = true,
                IsVisible = false,
                Volume = Settings.BgmVolume
            };
            mainGrid.Children.Add(_bgmPlayer);
            _bgmPlayer.Play();
        }
    }

    private async void btnNewGame_Click(object sender, EventArgs e)
    {
        if (!_mainPage.GameInProgress)
        {
            _mainPage.StartOrResetGame(true);
        }
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    private void btnQuit_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private async void btnSettings_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}
