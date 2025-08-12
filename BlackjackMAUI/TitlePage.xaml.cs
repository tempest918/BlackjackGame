using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class TitlePage : ContentPage
{
	public TitlePage()
	{
		InitializeComponent();
	}

    private async void btnNewGame_Click(object sender, EventArgs e)
    {
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
