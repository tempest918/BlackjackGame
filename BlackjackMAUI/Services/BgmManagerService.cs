using Plugin.Maui.Audio;
using System.Threading.Tasks;

namespace MyBlackjackMAUI.Services
{
    public class BgmManagerService
    {
        private IAudioPlayer? _bgmPlayer;
        private readonly IAudioManager _audioManager;

        public bool IsPlaying => _bgmPlayer?.IsPlaying ?? false;

        public BgmManagerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task InitializeAsync()
        {
            if (_bgmPlayer is not null)
            {
                return;
            }

            _bgmPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("bgm.mp3"));
            _bgmPlayer.Loop = true;
            _bgmPlayer.PlaybackEnded += OnPlaybackEnded;
        }

        public void Play()
        {
            if (_bgmPlayer is null || _bgmPlayer.IsPlaying)
            {
                return;
            }
            _bgmPlayer.Play();
        }

        public void Stop()
        {
            if (_bgmPlayer?.IsPlaying ?? false)
            {
                _bgmPlayer.Stop();
            }
        }

        private double _currentVolume = 1.0;
        public void SetVolume(double volume)
        {
            _currentVolume = volume;
            if (_bgmPlayer is not null)
            {
                _bgmPlayer.Volume = _currentVolume;
            }
        }

        private async void OnPlaybackEnded(object? sender, EventArgs e)
        {
            // Re-create the player to ensure it loops reliably on all platforms.
            // This is a more drastic approach but is necessary if Seek(0) fails.
            if (sender is IAudioPlayer oldPlayer)
            {
                oldPlayer.PlaybackEnded -= OnPlaybackEnded;
                oldPlayer.Dispose();
            }

            _bgmPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("bgm.mp3"));
            _bgmPlayer.Loop = true;
            _bgmPlayer.PlaybackEnded += OnPlaybackEnded;
            _bgmPlayer.Volume = _currentVolume;
            _bgmPlayer.Play();
        }
    }
}
