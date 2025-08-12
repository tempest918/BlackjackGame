using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class TitlePage : ContentPage
{
	public TitlePage()
	{
		InitializeComponent();
		btnContinue.IsEnabled = GameSaves.SaveFileExists();
	}

    private async void btnNewGame_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    private async void btnContinue_Click(object sender, EventArgs e)
    {
        var game = GameSaves.LoadGame();
        await Shell.Current.GoToAsync(nameof(MainPage), new Dictionary<string, object>
        {
            { "LoadedGame", game }
        });
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
