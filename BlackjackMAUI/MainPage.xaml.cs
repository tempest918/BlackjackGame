using BlackjackLogic;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Plugin.Maui.Audio;

namespace MyBlackjackMAUI;

public partial class MainPage : ContentPage
{
    private BlackjackGameLogic _game;
    private readonly IAudioManager _audioManager;
    private readonly Random _random = new();

    public bool GameInProgress { get; set; }

    public MainPage(IAudioManager audioManager, BlackjackGameLogic game)
    {
        InitializeComponent();
        _audioManager = audioManager;
        _game = game;

        ApplySettings();
        UpdateUI();
        DrawHands(false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateUI();
        DrawHands(false);
        ApplySettings();
    }

    private async void PlaySound(string fileName)
    {
        if (!Settings.SoundEffectsEnabled) return;

        try
        {
            var player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(fileName));
            player.Volume = Settings.SoundEffectsVolume;
            player.Play();
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            Console.WriteLine($"Error playing sound: {ex.Message}");
        }
    }

    private void PlayDealSound()
    {
        int soundNumber = _random.Next(1, 7); // 1 to 6
        PlaySound($"deal_{soundNumber}.wav");
    }

    private void ApplySettings()
    {
        // Felt Color
        this.BackgroundColor = Color.FromArgb(Settings.FeltColor switch
        {
            "Blue" => "#054a8a",
            "Red" => "#5b0506",
            _ => "#006a4e" // Green
        });

        UpdateUI(); // Re-draw cards with new backs
        DrawHands(false);
    }

    private async void btnBet_Click(object sender, EventArgs e)
    {
        if (int.TryParse(txtBet.Text, out int betAmount))
        {
            try
            {
                _game.StartNewHand(betAmount, Settings.NumberOfDecks);
                lblStatus.Text = "Player's Turn";
                await DealCardsWithAnimation();
                DrawHands(false); // Redraw to apply active hand highlight, which animation doesn't do.

                if (_game.CurrentState == GameState.HandOver)
                {
                    // Player has Blackjack, or some other immediate hand-over condition.
                    // We need to reveal the dealer's card and then end the hand.
                    var revealedCard = _game.Dealer.Hands[0][0];
                    var cardView = CreateCardView(revealedCard);
                    pnlDealerHand.Children.RemoveAt(0);
                    pnlDealerHand.Children.Insert(0, cardView);
                    AnimateCard(cardView);
                    await Task.Delay(350);

                    EndHand();
                }
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

    private async void btnHit_Click(object sender, EventArgs e)
    {
        int handIndexBeforeHit = _game.Player.ActiveHandIndex;
        _game.PlayerHits();

        var handThatWasHit = _game.Player.Hands[handIndexBeforeHit];
        var newCard = handThatWasHit.Last();

        var playerHandContainer = (VerticalStackLayout)pnlPlayerHand.Children[handIndexBeforeHit];
        var cardFlexLayout = (FlexLayout)playerHandContainer.Children[1];

        await AddSingleCardToUI(cardFlexLayout, newCard);

        var handLabel = (Label)playerHandContainer.Children[0];
        handLabel.Text = $"Hand {handIndexBeforeHit + 1} Score: {_game.Player.CalculateScore(handIndexBeforeHit)}";

        UpdateUI();

        if (_game.Player.ActiveHandIndex != handIndexBeforeHit)
        {
            DrawHands(false);
        }

        if (_game.CurrentState == GameState.HandOver)
        {
            EndHand();
        }
    }

    private async Task DealCardsWithAnimation()
    {
        // 1. Clear everything from previous hands
        pnlPlayerHand.Clear();
        pnlDealerHand.Clear();

        // 2. Set up player hand container since UpdateUI won't be doing it initially
        var playerHandContainer = new VerticalStackLayout { Spacing = 5, Margin = new Thickness(10) };
        var handLabel = new Label { FontAttributes = FontAttributes.Bold };
        var cardFlexLayout = new FlexLayout { JustifyContent = FlexJustify.Center, Wrap = FlexWrap.Wrap };
        playerHandContainer.Children.Add(handLabel);
        playerHandContainer.Children.Add(cardFlexLayout);
        pnlPlayerHand.Children.Add(playerHandContainer);

        // 3. Deal cards one by one with delays and sounds
        await AddSingleCardToUI(cardFlexLayout, _game.Player.Hands[0][0]);
        handLabel.Text = $"Hand 1 Score: {_game.Player.CalculateScore(0)}";

        await AddSingleCardToUI(pnlDealerHand, _game.Dealer.Hands[0][0], true);

        await AddSingleCardToUI(cardFlexLayout, _game.Player.Hands[0][1]);
        handLabel.Text = $"Hand 1 Score: {_game.Player.CalculateScore(0)}";

        await AddSingleCardToUI(pnlDealerHand, _game.Dealer.Hands[0][1]);

        // 4. Final UI update to refresh scores and button states
        UpdateUI();
    }

    private async Task AddSingleCardToUI(Layout parentLayout, Card card, bool isHidden = false)
    {
        var cardView = CreateCardView(card, isHidden);

        if (parentLayout is FlexLayout flex)
        {
            flex.Children.Add(cardView);
        }
        else if (parentLayout is StackLayout stack)
        {
            stack.Children.Add(cardView);
        }

        PlayDealSound();
        AnimateCard(cardView);
        await Task.Delay(350); // Animation delay
    }

    private async Task AnimateDealerTurn()
    {
        // Reveal dealer's first card
        var revealedCard = _game.Dealer.Hands[0][0];
        var cardView = CreateCardView(revealedCard);
        pnlDealerHand.Children.RemoveAt(0);
        pnlDealerHand.Children.Insert(0, cardView);
        AnimateCard(cardView);
        UpdateUI();
        await Task.Delay(350);

        // Dealer draws cards
        while (_game.Dealer.ShouldHit())
        {
            _game.DealerHits();
            var newCard = _game.Dealer.Hands[0].Last();
            await AddSingleCardToUI(pnlDealerHand, newCard);
            lblDealerScore.Text = $"Dealer Score: {_game.Dealer.CalculateScore(0)}";
            await Task.Delay(350); // Wait a bit after each card
        }

        _game.CurrentState = GameState.HandOver; // Manually update state after loop
    }

    private async void btnStay_Click(object sender, EventArgs e)
    {
        _game.PlayerStays();

        // After staying, check if the turn is over or if we moved to a new hand.
        if (_game.CurrentState == GameState.DealerTurn)
        {
            // Player's turn is over, start dealer's turn.
            await AnimateDealerTurn();
            EndHand();
        }
        else
        {
            // Still player's turn, but on a different hand. Update UI to reflect this.
            UpdateUI();
            DrawHands(false); // No animation needed for just switching focus
            ScrollToActiveHand();
        }
    }

    private void UpdateUI()
    {
        // Scores
        lblPlayerScore.Text = $"Player Score: {_game.Player.CalculateScore()}"; // Shows score of active hand
        lblDealerScore.Text = $"Dealer Score: {(_game.CurrentState == GameState.PlayerTurn || _game.CurrentState == GameState.AwaitingInsurance ? "?" : _game.Dealer.CalculateScore(0).ToString())}";

        // Money
        lblPlayerMoney.Text = $"Player Money: ${_game.Player.Money}";

        // Button states
        bool handInProgress = _game.CurrentState == GameState.PlayerTurn;
        bool awaitingInsurance = _game.CurrentState == GameState.AwaitingInsurance;

        ActionControls.IsVisible = handInProgress;
        BettingPanel.IsVisible = !handInProgress && !awaitingInsurance;
        InsuranceControls.IsVisible = awaitingInsurance;

        // Button visibility
        bool canDoubleDown = handInProgress && _game.Player.CurrentHand.Count == 2 && _game.Player.Money >= _game.CurrentBet;
        bool canSplit = handInProgress && _game.Player.CurrentHand.Count == 2 && _game.Player.CurrentHand[0].Value == _game.Player.CurrentHand[1].Value && _game.Player.Money >= _game.CurrentBet && _game.Player.Hands.Count < 4;

        btnDoubleDown.IsVisible = canDoubleDown;
        btnSplit.IsVisible = canSplit;

        if (awaitingInsurance)
        {
            lblStatus.Text = "Insurance?";
        }
    }

    private void DrawHands(bool animate = true)
    {
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
                if (animate)
                {
                    AnimateCard(cardView);
                }
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
                if (!isHidden && animate)
                {
                    AnimateCard(cardView);
                }
            }
        }
    }

    private void EndHand()
    {
        PlayerHandsScrollView.ScrollToAsync(lblStatus, ScrollToPosition.Start, true);

        List<HandResultInfo> results = _game.DetermineHandResult();
        lblStatus.Text = GetResultMessage(results);

        _game.UpdateStatsAndSave(results);

        // Determine overall win/loss for sound effect
        bool playerWon = results.Any(r => r.MainHandResult == HandResult.Win || r.MainHandResult == HandResult.Blackjack || r.InsuranceResult == HandResult.InsuranceWin);
        bool playerLost = results.Any(r => r.MainHandResult == HandResult.Loss);

        if (results.Any(r => r.MainHandResult == HandResult.Blackjack))
        {
            PlaySound("win.wav"); // Still play win sound, but also do special effect
            DisplaySpecialMessage(lblStatus.Text, Colors.Gold);
        }
        else if (playerWon && !playerLost) // Play win sound only if there's a win and no loss (e.g. split hands)
        {
            PlaySound("win.wav");
        }
        else if (playerLost) // Play loss sound if there's any loss
        {
            PlaySound("lose.wav");
        }


        UpdateUI(); // Final update to show dealer's full hand and final scores
        DrawHands(false);

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
                HandResult.Blackjack => $"Blackjack! Win (${(int)(_game.Bets[i] * 1.5)})",
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

    private async void DisplaySpecialMessage(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.TextColor = color;
        lblStatus.FontSize = 24;
        lblStatus.FontAttributes = FontAttributes.Bold;

        await lblStatus.ScaleTo(1.2, 500, Easing.CubicOut);
        await Task.Delay(1500);
        await lblStatus.ScaleTo(1.0, 500, Easing.CubicIn);

        // Reset to default style
        lblStatus.ClearValue(Label.TextColorProperty);
        lblStatus.FontSize = 24;
        lblStatus.FontAttributes = FontAttributes.Bold;
    }

    private async void ScrollToActiveHand()
    {
        // Give the UI a moment to update, especially the background color highlight
        await Task.Delay(100);

        if (_game.Player.ActiveHandIndex < pnlPlayerHand.Children.Count)
        {
            var activeHandView = pnlPlayerHand.Children[_game.Player.ActiveHandIndex];
            if (activeHandView is View view)
            {
                await PlayerHandsScrollView.ScrollToAsync(view, ScrollToPosition.MakeVisible, true);
            }
        }
    }


    private string GetCardImageFilename(Card card)
    {
        string face = card.Face.ToLower();
        string suit = card.Suit.ToLower();
        return $"card_{face}_of_{suit}.png";
    }

    private View CreateCardView(Card card, bool isHidden = false)
    {
        var cardImage = new Image
        {
            HeightRequest = 107,
            WidthRequest = 80,
            Margin = new Thickness(5),
            Aspect = Aspect.AspectFit
        };

        if (isHidden)
        {
            cardImage.Source = Settings.CardBack;
        }
        else
        {
            cardImage.Source = GetCardImageFilename(card);
        }

        return cardImage;
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

    public void StartOrResetGame(bool newGame)
    {
        GameInProgress = true;
        if (newGame)
        {
            _game.Reset();
        }

        // Reset the UI
    txtBet.Text = string.Empty;
        GameOverControls.IsVisible = false;
        pnlPlayerHand.Clear();
        pnlDealerHand.Clear();

        // Update all labels and button visibility for a fresh start
        UpdateUI();
        DrawHands(false);
    }

    private void btnNewGame_Click(object sender, EventArgs e)
    {
        StartOrResetGame(true);
    }

    private async void btnSettings_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void btnStats_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StatsPage));
    }

    private async void btnQuit_Click(object sender, EventArgs e)
    {
        GameInProgress = false;
        await Shell.Current.GoToAsync($"//{nameof(TitlePage)}");
    }

    private async void btnDoubleDown_Click(object sender, EventArgs e)
    {
        try
        {
            int activeHandIndex = _game.Player.ActiveHandIndex;
            _game.PlayerDoublesDown();

            var playerHandContainer = (VerticalStackLayout)pnlPlayerHand.Children[activeHandIndex];
            var cardFlexLayout = (FlexLayout)playerHandContainer.Children[1];
            var newCard = _game.Player.Hands[activeHandIndex].Last();
            await AddSingleCardToUI(cardFlexLayout, newCard);

            var handLabel = (Label)playerHandContainer.Children[0];
            handLabel.Text = $"Hand {activeHandIndex + 1} Score: {_game.Player.CalculateScore(activeHandIndex)}";

            UpdateUI();

            if (_game.CurrentState == GameState.DealerTurn)
            {
                await AnimateDealerTurn();
                EndHand();
            }
            else
            {
                DrawHands(false);
            }
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
            DrawHands(false);
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
        DrawHands(false);
    }

    private void btnSplit_Click(object sender, EventArgs e)
    {
        try
        {
            _game.PlayerSplits();
            UpdateUI();
            DrawHands(); // Should this be animated? A split is a big event. I'll leave it as true.
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
