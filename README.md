# MAUI Blackjack

A modern, cross-platform implementation of the classic card game Blackjack, built with .NET MAUI. Enjoy a rich user experience with animations, sound effects, and configurable settings.

## Features

-   **Classic Blackjack Gameplay:** Play against the dealer with all the standard actions, including Hit, Stay, Double Down, Split, and Insurance.
-   **Rich User Interface:** A clean, visually appealing interface with smooth card animations.
-   **Sound and Music:** Includes background music and sound effects for an immersive experience.
-   **Configurable Settings:**
    -   Adjust the number of decks used in the game (1-8).
    -   Customize the felt color (Green, Blue, Red).
    -   Choose your preferred card back design.
    -   Full audio controls, including separate volume sliders for background music and sound effects, with persistent mute states.
-   **Comprehensive Statistics Tracking:**
    -   Your game statistics (Wins, Losses, Pushes, Blackjacks, etc.) are tracked for your current session.
    -   The game saves a history of your past sessions ("runs"), allowing you to see your performance over time.
    -   Ran out of money? The game lets you reset your bankroll to $100 and start a fresh run, archiving your previous stats.
-   **Correct Rule Implementation:** The game correctly handles complex scenarios like a natural Blackjack vs. a multi-card 21.

## How to Play

1.  **Launch the game:** Start the application on your device.
2.  **Place your bet:** Use the chip buttons or enter an amount to place your bet for the hand.
3.  **Play your hand:** Choose to **Hit** (take another card) or **Stay** (end your turn). Based on your hand, you may also have options to **Double Down** (double your bet for one more card) or **Split** (if you have two cards of the same value).
4.  **Beat the Dealer:** The goal is to get a hand value closer to 21 than the dealer without going over. A two-card 21 is a "Blackjack" and pays 3:2.

## Building and Running

This is a standard .NET MAUI project. To build and run it, you will need the .NET SDK and the MAUI workload installed.

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/tempest918/BlackjackGame.git
    ```
2.  **Open the solution:** Open `Blackjack.sln` in Visual Studio 2022 (or later) or your preferred editor.
3.  **Build and Run:** Select your target platform (Android, iOS, macOS, or Windows) and run the application.

Alternatively, you can use the .NET CLI:

```bash
# Restore dependencies
dotnet restore

# Build the project for a specific platform, e.g., Windows
dotnet build -f net9.0-windows10.0.19041.0

# You can then run the built application.
```

## How to Contribute

Contributions to improve or expand the game are welcome!

1.  Fork the repository.
2.  Create a branch for your changes.
3.  Make your modifications.
4.  Submit a pull request with a clear explanation of your changes.