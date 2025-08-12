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

        public void SetVolume(double volume)
        {
            if (_bgmPlayer is not null)
            {
                _bgmPlayer.Volume = volume;
            }
        }

        private void OnPlaybackEnded(object? sender, EventArgs e)
        {
            if (sender is IAudioPlayer player)
            {
                player.Seek(0);
                player.Play();
            }
        }
    }
}
