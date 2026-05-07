using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Devices;
using Mopups.Pages;
using Mopups.Services;
using CommunityToolkit.Mvvm.Input;

namespace OWCE.Pages.Popup
{
    public partial class SideMenuPopup : PopupPage
    {
        static SideMenuPopup _instance;
        public static SideMenuPopup Instance => _instance ??= new SideMenuPopup();

        IAsyncRelayCommand _closeCommand;
        public IAsyncRelayCommand CloseCommand => _closeCommand ??= new AsyncRelayCommand(CloseCommand_Clicked);

        IAsyncRelayCommand<Grid> _aboutCommand;
        public IAsyncRelayCommand<Grid> AboutCommand => _aboutCommand ??= new AsyncRelayCommand<Grid>(async (sender) => await AboutCommand_Clicked(sender));

        IAsyncRelayCommand<Grid> _settingsCommand;
        public IAsyncRelayCommand<Grid> SettingsCommand => _settingsCommand ??= new AsyncRelayCommand<Grid>(async (sender) => await SettingsCommand_Clicked(sender));

        View _pageSpecificSideMenu = null;
        public View PageSpecificSideMenu {
            get
            {
                return _pageSpecificSideMenu;
            }
            set
            {
                if (_pageSpecificSideMenu != null)
                {
                    MainGrid.Children.Remove(_pageSpecificSideMenu);
                    _pageSpecificSideMenu = null;
                }

                if (value != null)
                {
                    _pageSpecificSideMenu = value;
                    Grid.SetRow(_pageSpecificSideMenu, 2);
                    MainGrid.Children.Add(_pageSpecificSideMenu);
                    Debug.WriteLine($"[OWCE:SideMenu] PageSpecificSideMenu set: {value.GetType().Name}");
                }
            }
        }

        private SideMenuPopup()
        {
            InitializeComponent();

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                HasSystemPadding = false;
            }

            BindingContext = this;
        }

        async void OnCloseMenuTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("[OWCE:SideMenu] Close (X) tapped");
            try
            {
                await CloseCommand_Clicked();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OWCE:SideMenu] OnCloseMenuTapped error: {ex}");
            }
        }

        async void OnSettingsMenuTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("[OWCE:SideMenu] Settings row tapped");
            try
            {
                await SettingsCommand_Clicked(SettingsMenuRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OWCE:SideMenu] OnSettingsMenuTapped error: {ex}");
            }
        }

        async void OnAboutMenuTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("[OWCE:SideMenu] About row tapped");
            try
            {
                await AboutCommand_Clicked(AboutMenuRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OWCE:SideMenu] OnAboutMenuTapped error: {ex}");
            }
        }

        internal async Task CloseCommand_Clicked()
        {
            Debug.WriteLine("[OWCE:SideMenu] CloseCommand_Clicked: PopAsync");
            await MopupService.Instance.PopAsync(true);
            Debug.WriteLine("[OWCE:SideMenu] CloseCommand_Clicked: done");
        }

        static Microsoft.Maui.Controls.NavigationPage GetRootNavigationPage()
        {
            var page = Microsoft.Maui.Controls.Application.Current?.Windows?.FirstOrDefault()?.Page;
            Debug.WriteLine($"[OWCE:SideMenu] Root Window.Page type: {page?.GetType().FullName ?? "null"}");
            return page as Microsoft.Maui.Controls.NavigationPage;
        }

        async Task AboutCommand_Clicked(Grid sender)
        {
            if (sender != null)
                sender.Opacity = 0.6f;

            var nav = GetRootNavigationPage();
            if (nav == null)
            {
                Debug.WriteLine("[OWCE:SideMenu] About: no NavigationPage; cannot PushModal");
                return;
            }

            Debug.WriteLine("[OWCE:SideMenu] About: PushModalAsync AboutPage + remove menu");
            try
            {
                await Task.WhenAll(
                    sender != null ? sender.FadeTo(1f) : Task.CompletedTask,
                    nav.Navigation.PushModalAsync(new CustomNavigationPage(new AboutPage())),
                    MopupService.Instance.RemovePageAsync(this)
                );
                Debug.WriteLine("[OWCE:SideMenu] About: navigation complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OWCE:SideMenu] AboutCommand_Clicked error: {ex}");
            }
        }

        async Task SettingsCommand_Clicked(Grid sender)
        {
            if (sender != null)
                sender.Opacity = 0.6f;

            var nav = GetRootNavigationPage();
            if (nav == null)
            {
                Debug.WriteLine("[OWCE:SideMenu] Settings: no NavigationPage; cannot PushModal");
                return;
            }

            Debug.WriteLine("[OWCE:SideMenu] Settings: PushModalAsync AppSettingsPage + remove menu");
            try
            {
                await Task.WhenAll(
                    sender != null ? sender.FadeTo(1f) : Task.CompletedTask,
                    nav.Navigation.PushModalAsync(new CustomNavigationPage(new AppSettingsPage())),
                    MopupService.Instance.RemovePageAsync(this)
                );
                Debug.WriteLine("[OWCE:SideMenu] Settings: navigation complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OWCE:SideMenu] SettingsCommand_Clicked error: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("[OWCE:SideMenu] OnAppearing");

            if (Content is Grid grid)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    var safeInsets = On<iOS>().SafeAreaInsets();
                    grid.Padding = new Thickness(0, safeInsets.Top, 0, safeInsets.Bottom);
                }
            }
        }
    }
}
