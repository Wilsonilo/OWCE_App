using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using OWCE.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace OWCE.Pages
{
    public partial class AppSettingsPage : BaseContentPage
    {
        public bool MetricDisplay { get; set; }
        public bool AutoRideRecording { get; set; }

        public AppSettingsPage()
        {
            InitializeComponent();

            MetricDisplay = App.Current.MetricDisplay;
            AutoRideRecording = Microsoft.Maui.Storage.Preferences.Get("auto_ride_recording", false);

            CustomToolbarItems.Add(new CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                Text = "Cancel",
                Command = new AsyncRelayCommand(async () =>
                {
                    await Navigation.PopModalAsync();
                }),
            });


            CustomToolbarItems.Add(new CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Right,
                Text = "Save",
                Command = new AsyncRelayCommand(async () =>
                {
                    App.Current.MetricDisplay = MetricDisplay;

                    Microsoft.Maui.Storage.Preferences.Set("metric_display", MetricDisplay);
                    Microsoft.Maui.Storage.Preferences.Set("auto_ride_recording", AutoRideRecording);

                    MessagingCenter.Send<App>(App.Current, App.UnitDisplayUpdatedKey);

                    await Navigation.PopModalAsync();
                }),
            });

            BindingContext = this;

        }
    }
}

