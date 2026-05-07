#if !DISABLE_APPCENTER
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endif

namespace OWCE.iOS;

internal static class AppCenterStartup
{
    public static void TryStart()
    {
#if !DISABLE_APPCENTER
        AppCenter.Start(
            $"android={OWCE.AppConstants.AppCenterAndroid};ios={OWCE.AppConstants.AppCenteriOS}",
            typeof(Analytics),
            typeof(Crashes));
#endif
    }
}
