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

        lblHandsPlayed.Text = stats.HandsPlayed.ToString();
        lblWins.Text = stats.Wins.ToString();
        lblLosses.Text = stats.Losses.ToString();
        lblPushes.Text = stats.Pushes.ToString();
        lblBlackjacks.Text = stats.Blackjacks.ToString();
        lblLargestPotWon.Text = $"${stats.LargestPotWon}";

        if (stats.HandsPlayed > 0)
        {
            double winPercentage = (double)stats.Wins / stats.HandsPlayed * 100;
            double lossPercentage = (double)stats.Losses / stats.HandsPlayed * 100;
            lblWinLossPercentage.Text = $"{winPercentage:F2}% / {lossPercentage:F2}%";
        }
        else
        {
            lblWinLossPercentage.Text = "N/A";
        }
    }

    private async void BackButton_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
