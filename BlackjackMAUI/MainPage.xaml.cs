using BlackjackLogic;
using Microsoft.Maui.Layouts;
using Plugin.Maui.Audio;

namespace MyBlackjackMAUI;

[QueryProperty(nameof(LoadedGame), "LoadedGame")]
public partial class MainPage : ContentPage
{
    private BlackjackGameLogic _game;
    private readonly IAudioManager _audioManager;

    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        _audioManager = audioManager;
        _game = new BlackjackGameLogic();
        UpdateUI();
    }

    public BlackjackGameLogic LoadedGame
    {
        set
        {
            if (value != null)
            {
                _game = value;
                UpdateUI();
            }
        }
    }

    private async void PlaySound(string fileName)
    {
        try
        {
            var player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(fileName));
            player.Play();
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            Console.WriteLine($"Error playing sound: {ex.Message}");
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
                PlaySound("deal.wav");
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
        PlaySound("deal.wav");
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
        lblPlayerScore.Text = $"Player Score: {_game.Player.CalculateScore()}"; // Shows score of active hand
        lblDealerScore.Text = $"Dealer Score: {(_game.CurrentState == GameState.PlayerTurn || _game.CurrentState == GameState.AwaitingInsurance ? "?" : _game.Dealer.CalculateScore(0).ToString())}";

        // Money
        lblPlayerMoney.Text = $"Player Money: ${_game.Player.Money}";

        // Player's Hands
        pnlPlayerHand.Clear();
        for (int i = 0; i < _game.Player.Hands.Count; i++)
        {
            var handContainer = new VerticalStackLayout { Spacing = 5, Margin = new Thickness(10) };
            if (i == _game.Player.ActiveHandIndex && _game.CurrentState == GameState.PlayerTurn)
            {
                handContainer.BackgroundColor = Colors.DarkGoldenrod; // Highlight active hand
            }

            var handLabel = new Label { Text = $"Hand {i + 1} Score: {_game.Player.CalculateScore(i)}", FontAttributes = FontAttributes.Bold };
            var cardFlexLayout = new FlexLayout { JustifyContent = FlexJustify.Center, Wrap = FlexWrap.Wrap };

            foreach (var card in _game.Player.Hands[i])
            {
                var cardView = CreateCardView(card);
                cardFlexLayout.Children.Add(cardView);
                // AnimateCard(cardView);
            }

            handContainer.Children.Add(handLabel);
            handContainer.Children.Add(cardFlexLayout);
            pnlPlayerHand.Children.Add(handContainer);
        }

        // Dealer's Hand
        pnlDealerHand.Clear();
        bool hideFirstCard = _game.CurrentState == GameState.PlayerTurn || _game.CurrentState == GameState.AwaitingInsurance;

        if (_game.Dealer.Hands != null && _game.Dealer.Hands.Count > 0)
        {
            foreach (var card in _game.Dealer.Hands[0])
            {
                var isHidden = hideFirstCard && card == _game.Dealer.Hands[0].First();
                var cardView = CreateCardView(card, isHidden);
                pnlDealerHand.Children.Add(cardView);
                if (!isHidden)
                {
                    // AnimateCard(cardView);
                }
            }
        }


        // Button states
        bool handInProgress = _game.CurrentState == GameState.PlayerTurn;
        bool awaitingInsurance = _game.CurrentState == GameState.AwaitingInsurance;

        ActionControls.IsVisible = handInProgress;
        BettingPanel.IsVisible = !handInProgress && !awaitingInsurance;
        InsuranceControls.IsVisible = awaitingInsurance;

        // Button visibility
        bool canDoubleDown = handInProgress && _game.Player.CurrentHand.Count == 2 && _game.Player.Money >= _game.CurrentBet;
        bool canSplit = handInProgress && _game.Player.CurrentHand.Count == 2 && _game.Player.CurrentHand[0].Value == _game.Player.CurrentHand[1].Value && _game.Player.Money >= _game.CurrentBet;

        btnDoubleDown.IsVisible = canDoubleDown;
        btnSplit.IsVisible = canSplit;

        if (awaitingInsurance)
        {
            lblStatus.Text = "Insurance?";
        }
    }

    private void EndHand()
    {
        List<HandResultInfo> results = _game.DetermineHandResult();
        lblStatus.Text = GetResultMessage(results);

        // Determine overall win/loss for sound effect
        bool playerWon = results.Any(r => r.MainHandResult == HandResult.Win || r.MainHandResult == HandResult.Blackjack || r.InsuranceResult == HandResult.InsuranceWin);
        bool playerLost = results.Any(r => r.MainHandResult == HandResult.Loss);

        if (playerWon && !playerLost) // Play win sound only if there's a win and no loss (e.g. split hands)
        {
            PlaySound("win.wav");
        }
        else if (playerLost) // Play loss sound if there's any loss
        {
            PlaySound("lose.wav");
        }


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
            BettingPanel.IsVisible = true;

            // Clear the previous bet amount
            txtBet.Text = "";
        }
    }

    private string GetResultMessage(List<HandResultInfo> results)
    {
        var message = new System.Text.StringBuilder();

        if (results.Count > 0 && results[0].InsuranceResult.HasValue)
        {
            if (results[0].InsuranceResult == HandResult.InsuranceWin)
            {
                message.AppendLine($"Insurance wins! You get ${_game.InsuranceBet * 2}.");
            }
            else
            {
                message.AppendLine("Insurance loses.");
            }
        }

        for (int i = 0; i < results.Count; i++)
        {
            string handIdentifier = results.Count > 1 ? $"Hand {i + 1}: " : "";
            message.Append(handIdentifier);
            message.Append(results[i].MainHandResult switch
            {
                HandResult.Win => $"Win (${_game.Bets[i]})",
                HandResult.Loss => "Loss",
                HandResult.Push => "Push",
                HandResult.Blackjack => $"Blackjack! Win (${_game.Bets[i] * 1.5})",
                _ => ""
            });
            if (i < results.Count - 1) message.Append(" | ");
        }

        return message.ToString();
    }

    private async void AnimateCard(View cardView)
    {
        cardView.Opacity = 0;
        cardView.TranslationY = -50;
        await cardView.FadeTo(1, 250, Easing.SinIn);
        await cardView.TranslateTo(0, 0, 250, Easing.SinOut);
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
        PlaySound("game_over.wav");
        // Hide all other controls
        ActionControls.IsVisible = false;
        BettingPanel.IsVisible = false;

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

    private void btnDoubleDown_Click(object sender, EventArgs e)
    {
        try
        {
            _game.PlayerDoublesDown();
            UpdateUI();
            EndHand();
        }
        catch (InvalidOperationException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void btnInsuranceYes_Click(object sender, EventArgs e)
    {
        try
        {
            _game.ResolveInsurance(true);
            // If dealer has blackjack, hand might be over
            if (_game.CurrentState == GameState.HandOver)
            {
                EndHand();
            }
            UpdateUI();
        }
        catch (InvalidOperationException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void btnInsuranceNo_Click(object sender, EventArgs e)
    {
        _game.ResolveInsurance(false);
        // If dealer has blackjack, hand might be over
        if (_game.CurrentState == GameState.HandOver)
        {
            EndHand();
        }
        UpdateUI();
    }

    private void btnSplit_Click(object sender, EventArgs e)
    {
        try
        {
            _game.PlayerSplits();
            UpdateUI();
        }
        catch (InvalidOperationException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void ChipButton_Click(object sender, EventArgs e)
    {
        if (sender is Button button && int.TryParse(button.CommandParameter?.ToString(), out int chipValue))
        {
            int.TryParse(txtBet.Text, out int currentBet); // Defaults to 0 if parsing fails

            int newBet = currentBet + chipValue;

            if (newBet <= _game.Player.Money)
            {
                txtBet.Text = newBet.ToString();
                PlaySound("chip.wav");
            }
        }
    }

    private void ClearBetButton_Click(object sender, EventArgs e)
    {
        txtBet.Text = "";
    }
}
