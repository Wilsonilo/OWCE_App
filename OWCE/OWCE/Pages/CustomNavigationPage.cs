using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace OWCE.Pages
{
    public class CustomNavigationPage : Microsoft.Maui.Controls.NavigationPage
    {
        public CustomNavigationPage(Microsoft.Maui.Controls.Page root) : base(root)
        {
            On<iOS>().SetHideNavigationBarSeparator(true);

        }

        protected override void OnParentSet()
        {
            base.OnParentSet();


            //BarBackgroundColor = App.Current.Resources[""];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            /*
            var oldBarBackgroundColor = BarBackgroundColor;
            BarBackgroundColor = Color.Beige;
            BarBackgroundColor = oldBarBackgroundColor;
            */
        }
    }
}
