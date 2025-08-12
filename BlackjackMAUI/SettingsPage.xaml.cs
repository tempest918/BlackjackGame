using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Decks
        sliderDecks.Value = Settings.NumberOfDecks;
        lblDecksValue.Text = Settings.NumberOfDecks.ToString();

        // Felt Color
        pickerFeltColor.SelectedItem = Settings.FeltColor;

        // Card Back
        pickerCardBack.SelectedItem = Settings.CardBack;

        // Sound
        switchSoundEffects.IsToggled = Settings.SoundEffectsEnabled;
        sliderBgmVolume.Value = Settings.BgmVolume;
        sliderSfxVolume.Value = Settings.SoundEffectsVolume;
    }

    private void sliderDecks_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        lblDecksValue.Text = ((int)e.NewValue).ToString();
    }

    private void sliderBgmVolume_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (AppShell.BgmPlayer is not null)
        {
            AppShell.BgmPlayer.Volume = e.NewValue;
        }
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        Settings.NumberOfDecks = (int)sliderDecks.Value;
        Settings.FeltColor = pickerFeltColor.SelectedItem.ToString();
        Settings.CardBack = pickerCardBack.SelectedItem.ToString();
        Settings.SoundEffectsEnabled = switchSoundEffects.IsToggled;
        Settings.BgmVolume = sliderBgmVolume.Value;
        Settings.SoundEffectsVolume = sliderSfxVolume.Value;

        if (AppShell.BgmPlayer is not null)
        {
            AppShell.BgmPlayer.Volume = Settings.BgmVolume;
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void btnCancel_Click(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
