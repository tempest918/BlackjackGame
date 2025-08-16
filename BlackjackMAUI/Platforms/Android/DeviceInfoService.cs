using Android.Content.Res;
using MyBlackjackMAUI.Services;

namespace MyBlackjackMAUI.Platforms.Android
{
    public class DeviceInfoService : IDeviceInfoService
    {
        public bool IsTablet()
        {
            var context = MainActivity.Instance;
            var screenLayout = context.Resources.Configuration.ScreenLayout;
            return (screenLayout & ScreenLayout.SizeMask) >= ScreenLayout.SizeLarge;
        }
    }
}
