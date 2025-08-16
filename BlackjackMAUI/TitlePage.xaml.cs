using BlackjackLogic;
using System.Threading.Tasks;

namespace MyBlackjackMAUI;

public partial class TitlePage : ContentPage
{
    private readonly MainPage _mainPage;
    private CancellationTokenSource _animationCts;

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

    private async void btnHistory_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationCts = new CancellationTokenSource();
        _ = AnimateLogo(_animationCts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _animationCts?.Cancel();
    }

    private async Task AnimateLogo(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await logoImage.ScaleTo(1.1, 1000, Easing.SinInOut);
                token.ThrowIfCancellationRequested();
                await logoImage.ScaleTo(1.0, 1000, Easing.SinInOut);
            }
        }
        catch (OperationCanceledException)
        {
            // This is expected when the page disappears.
            // Reset the scale to its original state.
            logoImage.Scale = 1.0;
        }
    }
}
