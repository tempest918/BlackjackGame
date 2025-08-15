using BlackjackLogic;
using MyBlackjackMAUI.Services;

namespace MyBlackjackMAUI;

public partial class SettingsPage : ContentPage
{
    private readonly BgmManagerService _bgmManager;
    private readonly Dictionary<string, string> _cardBackOptions = new()
    {
        { "Red 1", "card_back_red_one.png" },
        { "Red 2", "card_back_red_two.png" },
        { "Blue 1", "card_back_blue_one.png" },
        { "Blue 2", "card_back_blue_two.png" },
        { "Grey 1", "card_back_grey_one.png" },
        { "Grey 2", "card_back_grey_two.png" }
    };

    public SettingsPage(BgmManagerService bgmManager)
    {
        InitializeComponent();
        _bgmManager = bgmManager;
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
        pickerCardBack.ItemsSource = _cardBackOptions.Keys.ToList();
        string savedCardBackFilename = Settings.CardBack;
        var savedFriendlyName = _cardBackOptions.FirstOrDefault(x => x.Value == savedCardBackFilename).Key;
        if (savedFriendlyName != null)
        {
            pickerCardBack.SelectedItem = savedFriendlyName;
        }
        else
        {
            pickerCardBack.SelectedIndex = 0; // Default to the first item if the saved one is not found
        }

        // Sound
        sliderBgmVolume.Value = Settings.BgmVolume;
        sliderSfxVolume.Value = Settings.SoundEffectsVolume;

        UpdateMuteButtons();
    }

    private void sliderDecks_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        lblDecksValue.Text = ((int)e.NewValue).ToString();
    }

    private void sliderBgmVolume_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        _bgmManager.SetVolume(e.NewValue);
        // If user manually changes volume, update the pre-mute setting
        if (e.NewValue > 0)
        {
            Settings.PreMuteBgmVolume = e.NewValue;
        }
        UpdateMuteButtons();
    }

    private void sliderSfxVolume_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        // If user manually changes volume, update the pre-mute setting
        if (e.NewValue > 0)
        {
            Settings.PreMuteSfxVolume = e.NewValue;
        }
        UpdateMuteButtons();
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        Settings.NumberOfDecks = (int)sliderDecks.Value;
        Settings.FeltColor = pickerFeltColor.SelectedItem.ToString();

        string selectedFriendlyName = pickerCardBack.SelectedItem.ToString();
        Settings.CardBack = _cardBackOptions[selectedFriendlyName];

        // Save final slider values
        Settings.BgmVolume = sliderBgmVolume.Value;
        Settings.SoundEffectsVolume = sliderSfxVolume.Value;

        // PreMute volumes are already updated by sliders, so they are implicitly saved

        await Shell.Current.GoToAsync("..");
    }

    private async void btnCancel_Click(object sender, EventArgs e)
    {
        // Revert BGM volume to its state when the page was opened
        _bgmManager.SetVolume(Settings.BgmVolume);
        await Shell.Current.GoToAsync("..");
    }

    private void btnMuteBgm_Click(object sender, EventArgs e)
    {
        if (sliderBgmVolume.Value > 0)
        {
            // Muting: PreMute volume is already set by the ValueChanged event.
            // Just set the slider to 0.
            sliderBgmVolume.Value = 0;
        }
        else
        {
            // Unmuting: Restore from the persistent PreMute setting.
            sliderBgmVolume.Value = Settings.PreMuteBgmVolume;
        }
    }

    private void btnMuteSfx_Click(object sender, EventArgs e)
    {
        if (sliderSfxVolume.Value > 0)
        {
            // Muting
            sliderSfxVolume.Value = 0;
        }
        else
        {
            // Unmuting
            sliderSfxVolume.Value = Settings.PreMuteSfxVolume;
        }
    }

    private void UpdateMuteButtons()
    {
        // BGM Mute Button
        btnMuteBgm.Text = sliderBgmVolume.Value > 0 ? "\ue050" : "\ue04f";

        // SFX Mute Button
        btnMuteSfx.Text = sliderSfxVolume.Value > 0 ? "\ue050" : "\ue04f";
    }

    private void switchSoundEnabled_Toggled(object sender, ToggledEventArgs e)
    {
        // This switch now only controls sound effects, not BGM.
        // The BGM is controlled by its own volume slider and mute button.
    }
}
