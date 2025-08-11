using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class MainPage : ContentPage
{
    private BlackjackGameLogic _game;

    public MainPage()
    {
        InitializeComponent();
        _game = new BlackjackGameLogic();
        UpdateUI();
    }

    public BlackjackGameLogic LoadedGame
    {
        set
        {
            _game = value;
            UpdateUI();
        }
    }

    private void btnBet_Click(object sender, EventArgs e)
    {
        if (int.TryParse(txtBet.Text, out int betAmount))
        {
            try
            {
                _game.StartNewHand(betAmount);
                lblStatus.Text = "Player's Turn";
                UpdateUI();
            }
            catch (ArgumentException ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        else
        {
            DisplayAlert("Error", "Please enter a valid bet amount.", "OK");
        }
    }

    private void btnHit_Click(object sender, EventArgs e)
    {
        _game.PlayerHits();
        UpdateUI();
        if (_game.CurrentState == GameState.HandOver)
        {
            EndHand();
        }
    }

    private void btnStay_Click(object sender, EventArgs e)
    {
        _game.PlayerStays();
        UpdateUI();
        EndHand();
    }

    private void UpdateUI()
    {
        // Scores
        lblPlayerScore.Text = $"Player Score: {_game.Player.CalculateScore()}";
        lblDealerScore.Text = $"Dealer Score: {(_game.CurrentState == GameState.PlayerTurn ? "?" : _game.Dealer.CalculateScore().ToString())}";

        // Money
        lblPlayerMoney.Text = $"Player Money: ${_game.Player.Money}";

        // Hands
        pnlPlayerHand.Clear();
        foreach (var card in _game.Player.Hand)
        {
            pnlPlayerHand.Children.Add(CreateCardView(card));
        }

        pnlDealerHand.Clear();
        bool hideFirstCard = _game.CurrentState == GameState.PlayerTurn;
        if (_game.Dealer.Hand != null)
        {
            foreach (var card in _game.Dealer.Hand)
            {
                pnlDealerHand.Children.Add(CreateCardView(card, hideFirstCard));
                hideFirstCard = false; // only hide the first one
            }
        }

        // Button states
        bool handInProgress = _game.CurrentState == GameState.PlayerTurn;
        ActionControls.IsVisible = handInProgress;
        BettingControls.IsVisible = !handInProgress;
    }

    private void EndHand()
    {
        HandResult result = _game.DetermineHandResult();
        lblStatus.Text = GetResultMessage(result);

        UpdateUI(); // Final update to show dealer's full hand and final scores

        if (_game.Player.Money <= 0)
        {
            GameOver();
        }
        else
        {
            // Hide the Hit/Stay buttons
            ActionControls.IsVisible = false;

            // Show the betting controls for the next hand
            BettingControls.IsVisible = true;

            // Clear the previous bet amount
            txtBet.Text = "";
        }
    }

    private string GetResultMessage(HandResult result)
    {
        return result switch
        {
            HandResult.Win => $"You win ${_game.CurrentBet}!",
            HandResult.Loss => "You lose!",
            HandResult.Push => "Push (Tie)!",
            HandResult.Blackjack => $"Blackjack! You win ${_game.CurrentBet * 1.5}!",
            _ => ""
        };
    }

    private View CreateCardView(Card card, bool isHidden = false)
    {
        var border = new Border
        {
            Stroke = Colors.Black,
            StrokeThickness = 2,
            BackgroundColor = Colors.White,
            HeightRequest = 120,
            WidthRequest = 80,
            Margin = new Thickness(5)
        };

        if (isHidden)
        {
            border.BackgroundColor = (Color)Application.Current.Resources["FeltGreenDark"];
        }
        else
        {
            var grid = new Grid { Padding = 5 };
            var suitColor = (card.Suit == "Hearts" || card.Suit == "Diamonds") ? Colors.Red : Colors.Black;

            grid.Children.Add(new Label { Text = card.Face, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = suitColor, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start });
            grid.Children.Add(new Label { Text = card.GetSuitSymbol(), FontSize = 24, TextColor = suitColor, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center });
            grid.Children.Add(new Label { Text = card.Face, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = suitColor, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End, Rotation = 180 });

            border.Content = grid;
        }

        return border;
    }

    private void GameOver()
    {
        // Hide all other controls
        ActionControls.IsVisible = false;
        BettingControls.IsVisible = false;

        // Show the new Game Over panel
        GameOverControls.IsVisible = true;
    }

    private void btnNewGame_Click(object sender, EventArgs e)
    {
        // Create a fresh instance of the game logic to reset everything
        _game = new BlackjackGameLogic();

        // Reset the UI
        GameOverControls.IsVisible = false;
        pnlPlayerHand.Clear();
        pnlDealerHand.Clear();
        txtBet.Text = "";

        // Update all labels and button visibility for a fresh start
        UpdateUI();
    }

    private void btnQuit_Click(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private async void btnSaveQuit_Click(object sender, EventArgs e)
    {
        GameSaves.SaveGame(_game);
        await Shell.Current.GoToAsync("..");
    }
}
