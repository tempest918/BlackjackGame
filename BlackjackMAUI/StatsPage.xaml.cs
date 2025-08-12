using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class StatsPage : ContentPage
{
    public StatsPage()
    {
        InitializeComponent();
        LoadStats();
    }

    private void LoadStats()
    {
        var stats = PersistenceService.LoadStats();

        // Display current run stats
        lblHandsPlayed.Text = stats.CurrentRun.HandsPlayed.ToString();
        lblWins.Text = stats.CurrentRun.Wins.ToString();
        lblLosses.Text = stats.CurrentRun.Losses.ToString();
        lblPushes.Text = stats.CurrentRun.Pushes.ToString();
        lblBlackjacks.Text = stats.CurrentRun.Blackjacks.ToString();
        lblLargestPotWon.Text = $"${stats.CurrentRun.LargestPotWon}";

        if (stats.CurrentRun.HandsPlayed > 0)
        {
            double winPercentage = (double)stats.CurrentRun.Wins / stats.CurrentRun.HandsPlayed * 100;
            double lossPercentage = (double)stats.CurrentRun.Losses / stats.CurrentRun.HandsPlayed * 100;
            lblWinLossPercentage.Text = $"{winPercentage:F2}% / {lossPercentage:F2}%";
        }
        else
        {
            lblWinLossPercentage.Text = "N/A";
        }

        // Bind historical data, ordered by most recent first
        HistoryListView.ItemsSource = stats.History.OrderByDescending(h => h.EndTime).ToList();
    }

    private async void BackButton_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
