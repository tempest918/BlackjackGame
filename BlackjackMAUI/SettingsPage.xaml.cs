using BlackjackLogic;

namespace MyBlackjackMAUI;

public partial class SettingsPage : ContentPage
{
    private double _preMuteBgmVolume;
    private double _preMuteSfxVolume;

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
        switchSoundEnabled.IsToggled = Settings.SoundEffectsEnabled; // Assuming this is the master switch now
        sliderBgmVolume.Value = Settings.BgmVolume;
        sliderSfxVolume.Value = Settings.SoundEffectsVolume;

        _preMuteBgmVolume = Settings.BgmVolume;
        _preMuteSfxVolume = Settings.SoundEffectsVolume;
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
        Settings.SoundEffectsEnabled = switchSoundEnabled.IsToggled;
        Settings.BgmVolume = sliderBgmVolume.Value;
        Settings.SoundEffectsVolume = sliderSfxVolume.Value;

        if (AppShell.BgmPlayer is not null)
        {
            AppShell.BgmPlayer.Volume = Settings.BgmVolume;
            if (Settings.SoundEffectsEnabled)
            {
                if (!AppShell.BgmPlayer.IsPlaying) AppShell.BgmPlayer.Play();
            }
            else
            {
                AppShell.BgmPlayer.Stop();
            }
        }

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
            _preMuteBgmVolume = sliderBgmVolume.Value;
            sliderBgmVolume.Value = 0;
            btnMuteBgm.Text = "Unmute";
        }
        else
        {
            sliderBgmVolume.Value = _preMuteBgmVolume;
            btnMuteBgm.Text = "Mute";
        }
    }

    private void btnMuteSfx_Click(object sender, EventArgs e)
    {
        if (sliderSfxVolume.Value > 0)
        {
            _preMuteSfxVolume = sliderSfxVolume.Value;
            sliderSfxVolume.Value = 0;
            btnMuteSfx.Text = "Unmute";
        }
        else
        {
            sliderSfxVolume.Value = _preMuteSfxVolume;
            btnMuteSfx.Text = "Mute";
        }
    }
}
