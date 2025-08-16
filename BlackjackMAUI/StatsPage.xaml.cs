using BlackjackLogic;
using Microcharts;
using SkiaSharp;
using System.Linq;

namespace MyBlackjackMAUI;

public partial class StatsPage : ContentPage
{
    public StatsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
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
        lblBiggestLoss.Text = $"${stats.CurrentRun.BiggestLoss}";

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

        // Create the chart
        if (stats.CurrentRun.MoneyHistory != null && stats.CurrentRun.MoneyHistory.Count > 1)
        {
            var entries = new List<ChartEntry>();
            for (int i = 0; i < stats.CurrentRun.MoneyHistory.Count; i++)
            {
                entries.Add(new ChartEntry(stats.CurrentRun.MoneyHistory[i])
                {
                    Label = (i + 1).ToString(),
                    ValueLabel = stats.CurrentRun.MoneyHistory[i].ToString(),
                    Color = SKColor.Parse("#FFD700")
                });
            }

            chartView.Chart = new LineChart
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.Transparent,
                AnimationDuration = TimeSpan.Zero,
                IsAnimated = false
            };
        }
        else
        {
            chartView.IsVisible = false;
        }


        // Historical data is now on its own page.
    }

    private async void BackButton_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
