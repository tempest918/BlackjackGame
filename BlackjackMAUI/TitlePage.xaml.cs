using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class TitlePage : ContentPage
{
    private readonly MainPage _mainPage;

    public TitlePage(MainPage mainPage)
	{
		InitializeComponent();
        _mainPage = mainPage;
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
