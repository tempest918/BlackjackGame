using MyBlackjackMAUI.Services;

namespace MyBlackjackMAUI
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;
        private readonly BgmManagerService _bgmManager;

        public App(AppShell appShell, BgmManagerService bgmManager)
        {
            InitializeComponent();
            _appShell = appShell;
            _bgmManager = bgmManager;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await _bgmManager.InitializeAsync();
            _bgmManager.Play();
        }
    }
}