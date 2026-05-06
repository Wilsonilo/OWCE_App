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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("SairaExtraCondensed-Black.ttf", "SairaBlack");
                    fonts.AddFont("SairaExtraCondensed-Bold.ttf", "SairaBold");
                    fonts.AddFont("SairaExtraCondensed-SemiBold.ttf", "SairaSemiBold");
                    fonts.AddFont("SairaExtraCondensed-Light.ttf", "SairaLight");
                    fonts.AddFont("SairaExtraCondensed-Medium.ttf", "SairaMedium");
                    fonts.AddFont("SairaExtraCondensed-ExtraBold.ttf", "SairaExtraBold");
                    fonts.AddFont("SairaExtraCondensed-Regular.ttf", "SairaRegular");
                })
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddHandler<ArcView, ArcViewHandler>();
                });

            builder.Services.AddSingleton(typeof(IOWBLE), typeof(OWBLE));
            builder.Services.AddSingleton(typeof(IPermissionPrompt), typeof(PermissionPrompt));
            builder.Services.AddSingleton(typeof(IUserAgent), typeof(UserAgent));
            builder.Services.AddSingleton(typeof(IWatch), typeof(Watch));

            return builder.Build();
        }
    }
}
