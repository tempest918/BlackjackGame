using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class HistoryPage : ContentPage
{
    private readonly BlackjackGameLogic _game;

	public HistoryPage(BlackjackGameLogic game)
	{
		InitializeComponent();
        _game = game;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        HistoryListView.ItemsSource = _game.Stats.History.OrderByDescending(run => run.EndTime).ToList();
    }

    private async void BackButton_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
