using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;
using Mopups.Hosting;
using OWCE.DependencyInterfaces;
using OWCE.iOS.DependencyImplementations;
using OWCE.iOS.Handlers;
using OWCE.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace OWCE.iOS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMopups()
                .UseSkiaSharp()
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler<ArcView, ArcViewHandler>();
                });

            builder.Services.AddSingleton(typeof(IOWBLE), typeof(OWBLE));
            builder.Services.AddSingleton(typeof(IPermissionPrompt), typeof(PermissionPrompt));
            builder.Services.AddSingleton(typeof(IUserAgent), typeof(UserAgent));
            builder.Services.AddSingleton(typeof(IWatch), typeof(Watch));

            AppCenterStartup.TryStart();

            return builder.Build();
        }
    }
}
