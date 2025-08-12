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
        switchSoundEnabled.IsToggled = Settings.SoundEffectsEnabled;
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
        if (AppShell.BgmPlayer is not null)
        {
            AppShell.BgmPlayer.Volume = e.NewValue;
        }
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
        Settings.CardBack = pickerCardBack.SelectedItem.ToString();
        Settings.SoundEffectsEnabled = switchSoundEnabled.IsToggled;

        // Save final slider values
        Settings.BgmVolume = sliderBgmVolume.Value;
        Settings.SoundEffectsVolume = sliderSfxVolume.Value;

        // PreMute volumes are already updated by sliders, so they are implicitly saved

        await Shell.Current.GoToAsync("..");
    }

    private async void btnCancel_Click(object sender, EventArgs e)
    {
        // Revert BGM volume to its state when the page was opened
        if (AppShell.BgmPlayer is not null)
        {
            AppShell.BgmPlayer.Volume = Settings.BgmVolume;
        }
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
        if (AppShell.BgmPlayer is null) return;

        if (e.Value)
        {
            AppShell.BgmPlayer.Play();
        }
        else
        {
            AppShell.BgmPlayer.Stop();
        }
    }
}
