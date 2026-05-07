using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using OWCE.DependencyInterfaces;
using OWCE.Pages;

namespace OWCE
{
    public partial class App : Application
    {
        public const string UnitDisplayUpdatedKey = "UnitDisplayUpdated";

        public static new App Current => Application.Current as App;
        public IOWBLE OWBLE { get; private set; }

        public static T GetService<T>() where T : class =>
            IPlatformApplication.Current?.Services?.GetService<T>();

#if DEBUG
        public const string OWCEApiServer = "api.dev.owce.app";
#else
        public const string OWCEApiServer = "api.owce.app";
#endif

        public static readonly BindableProperty MetricDisplayProperty = BindableProperty.Create(
            nameof(MetricDisplay),
            typeof(bool),
            typeof(App),
            false);

        public bool MetricDisplay
        {
            get { return (bool)GetValue(MetricDisplayProperty); }
            set { SetValue(MetricDisplayProperty, value); }
        }

        public string LogsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "past_rides");

        public string UserAgent => $"OWCE/{AppInfo.VersionString} Build/{AppInfo.BuildString} ({DeviceInfo.Platform}; {DeviceInfo.VersionString})";

        public App(IOWBLE owble)
        {
            OWBLE = owble;

            MetricDisplay = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);

            if (Directory.Exists(LogsDirectory) == false)
            {
                Directory.CreateDirectory(LogsDirectory);
            }

            Database.Init();

            InitializeComponent();

            MainPage = new CustomNavigationPage(new BoardListPage());
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        internal async Task<OWBoard> ConnectToBoard(OWBaseBoard baseBoard, CancellationToken token)
        {
            var didConnect = await OWBLE.Connect(baseBoard, token);
            if (didConnect)
            {
                return new OWBoard(OWBLE, baseBoard);
            }

            return null;
        }

        internal void DisconnectFromBoard()
        {
        }
    }
}
