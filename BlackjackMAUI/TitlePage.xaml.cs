using BlackjackLogic;
using System.Threading.Tasks;

namespace MyBlackjackMAUI;

public partial class TitlePage : ContentPage
{
    private readonly MainPage _mainPage;
    private bool _isQuitting;

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

    private async void btnQuit_Click(object sender, EventArgs e)
    {
        _isQuitting = true;
        await Task.Delay(50);
        Application.Current.Quit();
    }

    private async void btnSettings_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isQuitting = false;
        _ = AnimateLogo();
    }

    private async Task AnimateLogo()
    {
        while (!_isQuitting)
        {
            if (!IsVisible) break;
            await logoImage.ScaleTo(1.1, 1000, Easing.SinInOut);
            if (!IsVisible || _isQuitting) break;
            await logoImage.ScaleTo(1.0, 1000, Easing.SinInOut);
        }
    }
}
