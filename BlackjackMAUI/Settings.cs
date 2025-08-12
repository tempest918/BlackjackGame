using Microsoft.Maui.Storage;

namespace MyBlackjackMAUI
{
    public static class Settings
    {
        // Default values
        private const int DefaultNumberOfDecks = 1;
        private const string DefaultFeltColor = "Green";
        private const string DefaultCardBack = "Red";
        private const bool DefaultSoundEffectsEnabled = true;
        private const double DefaultBgmVolume = 0.5;
        private const double DefaultSoundEffectsVolume = 0.8;

        public static int NumberOfDecks
        {
            get => Preferences.Get(nameof(NumberOfDecks), DefaultNumberOfDecks);
            set => Preferences.Set(nameof(NumberOfDecks), value);
        }

        public static string FeltColor
        {
            get => Preferences.Get(nameof(FeltColor), DefaultFeltColor);
            set => Preferences.Set(nameof(FeltColor), value);
        }

        public static string CardBack
        {
            get => Preferences.Get(nameof(CardBack), DefaultCardBack);
            set => Preferences.Set(nameof(CardBack), value);
        }

        public static bool SoundEffectsEnabled
        {
            get => Preferences.Get(nameof(SoundEffectsEnabled), DefaultSoundEffectsEnabled);
            set => Preferences.Set(nameof(SoundEffectsEnabled), value);
        }

        public static double BgmVolume
        {
            get => Preferences.Get(nameof(BgmVolume), DefaultBgmVolume);
            set => Preferences.Set(nameof(BgmVolume), value);
        }

        public static double SoundEffectsVolume
        {
            get => Preferences.Get(nameof(SoundEffectsVolume), DefaultSoundEffectsVolume);
            set => Preferences.Set(nameof(SoundEffectsVolume), value);
        }

        public static void ResetToDefaults()
        {
            Preferences.Clear();
        }
    }
}
