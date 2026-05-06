# OWCE — Xamarin → .NET MAUI Migration Plan
**Branch:** `maui-migration`  
**Repo:** `Wilsonilo/OWCE_App`  
**Target:** TestFlight under `net.wilsonmunoz.owce`, Team `DYWX76A76F`

---

## Approach

Manual migration (not upgrade-assistant). We replace the Xamarin.iOS host with a MAUI iOS host, update the shared project to MAUI packages, rewrite the two platform seams (custom renderers → handlers, DI registrations), and fix all XAML. WatchOS is excluded from scope. Android and macOS are excluded from scope.

Working directory for all steps: `/Users/will/Desktop/Personal Projects/Others/OWCE_App_Fork`

---

## Trade-offs

| Option | Notes |
|--------|-------|
| upgrade-assistant | Handles ~60% automatically but leaves PancakeView, Rg.Plugins.Popup, and renderer rewrites broken — creates harder debugging state |
| Manual migration | More upfront work, but full control and predictable build errors |

**Chosen:** Manual migration.

---

## Todo Checklist

### A — Toolchain setup
- [ ] A1. Install .NET 9 SDK via brew
- [ ] A2. Install `maui-ios` workload (`dotnet workload install maui-ios`)
- [ ] A3. Verify: `dotnet workload list` shows `maui-ios`

### B — Solution restructure
- [ ] B1. Delete `OWCE.iOS/OWCE.iOS.csproj` and replace with new MAUI iOS `.csproj` targeting `net9.0-ios`
- [ ] B2. Delete `OWCE.Android`, `OWCE.MacOS`, `OWCE.WatchOS` project references from `OWCE.sln` (keep folders, remove from solution)
- [ ] B3. Update `OWCE/OWCE.csproj` → change target to `net9.0-ios` (multi-target later if needed), update all package references
- [ ] B4. Update `OWCE.sln` to reference new project structure

### C — Shared project (`OWCE/OWCE.csproj`) package replacements
- [ ] C1. Remove: `Xamarin.Forms`, `Xamarin.Essentials`, `Xamarin.CommunityToolkit`, `Xamarin.Forms.PancakeView`, `Rg.Plugins.Popup`, `SkiaSharp.Views.Forms`, `Xam.Plugin.Geolocator`
- [ ] C2. Add: `Microsoft.Maui.Controls`, `CommunityToolkit.Maui`, `Mopups`, `SkiaSharp.Views.Maui.Controls`
- [ ] C3. Keep as-is: `SkiaSharp`, `sqlite-net-pcl`, `Google.Protobuf`, `SharpZipLib`, `Microsoft.AppCenter.*`, `CommunityToolkit.Mvvm`, `Refractored.MvvmHelpers`, `System.Text.Json`, `System.Net.Http.Json`

### D — Shared project: namespace and API renames (all `.cs` and `.xaml` files)
- [ ] D1. `using Xamarin.Forms` → `using Microsoft.Maui.Controls`
- [ ] D2. `using Xamarin.Forms.Shapes` → `using Microsoft.Maui.Controls.Shapes`
- [ ] D3. `using Xamarin.Forms.PlatformConfiguration` → `using Microsoft.Maui.Controls.PlatformConfiguration`
- [ ] D4. `using Xamarin.Forms.PlatformConfiguration.iOSSpecific` → `using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific`
- [ ] D5. `using Xamarin.Essentials` → `using Microsoft.Maui.ApplicationModel` (and sub-namespaces where needed: `Devices`, `Storage`, etc.)
- [ ] D6. `using Xamarin.CommunityToolkit.ObjectModel` → `using CommunityToolkit.Mvvm.ComponentModel` + `using CommunityToolkit.Mvvm.Input`
  - `AsyncCommand` → `AsyncRelayCommand`
  - `ObservableRangeCollection` → `ObservableCollection` (MAUI built-in) or CommunityToolkit equivalent
- [ ] D7. `using Xamarin.CommunityToolkit.UI.Views` → `using CommunityToolkit.Maui.Views`
- [ ] D8. `using Rg.Plugins.Popup.*` → `using Mopups.*`
  - `PopupNavigation.Instance.PushAsync()` → `MopupService.Instance.PushAsync()`
  - `PopupPage` base class → `Mopups.Pages.PopupPage`
- [ ] D9. Remove `[assembly: XamlCompilation(XamlCompilationOptions.Compile)]` from `App.xaml.cs` (default in MAUI)

### E — App entry point: create `MauiProgram.cs`
- [ ] E1. Create `OWCE.iOS/MauiProgram.cs` with `CreateMauiApp()` builder
  ```csharp
  public static class MauiProgram
  {
      public static MauiApp CreateMauiApp()
      {
          var builder = MauiApp.CreateBuilder();
          builder
              .UseMauiApp<App>()
              .UseMopups()
              .UseMauiCommunityToolkit()
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("SairaExtraCondensed-Black.ttf", "SairaBlack");
                  fonts.AddFont("SairaExtraCondensed-Bold.ttf", "SairaBold");
                  fonts.AddFont("SairaExtraCondensed-SemiBold.ttf", "SairaSemiBold");
                  fonts.AddFont("SairaExtraCondensed-Light.ttf", "SairaLight");
                  fonts.AddFont("SairaExtraCondensed-Medium.ttf", "SairaMedium");
              })
              .ConfigureMauiHandlers(handlers =>
              {
                  handlers.AddHandler<ArcView, ArcViewHandler>();
              });

          // Register platform services
          builder.Services.AddSingleton<IOWBLE, OWBLE>();
          builder.Services.AddSingleton<IPermissionPrompt, PermissionPrompt>();
          builder.Services.AddSingleton<IUserAgent, UserAgent>();

          return builder.Build();
      }
  }
  ```
- [ ] E2. Update `AppDelegate.cs` — inherit from `MauiUIApplicationDelegate`, remove `Forms.Init()`, remove `Popup.Init()`, remove `LoadApplication()`
  ```csharp
  [Register("AppDelegate")]
  public class AppDelegate : MauiUIApplicationDelegate
  {
      protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
  }
  ```
- [ ] E3. Update `Main.cs` — ensure it points to `AppDelegate` (no changes needed typically)
- [ ] E4. Remove `[assembly: ExportFont(...)]` lines from `App.xaml.cs` (moved to MauiProgram)
- [ ] E5. Remove AppCenter app secret injection from `App.xaml.cs` (leave `AppCenteriOS = ""`, it's a no-op)

### F — Dependency injection: replace `[assembly: Dependency]`
- [ ] F1. Remove `[assembly: Dependency(typeof(OWBLE))]` from `OWBLE.cs`, remove all `DependencyService.Get<>()` calls in shared code → use constructor injection via DI
- [ ] F2. Same for `PermissionPrompt.cs`, `UserAgent.cs`, `Watch.cs`
- [ ] F3. In `App.xaml.cs` constructor: receive `IOWBLE` via DI parameter instead of `DependencyService.Get<IOWBLE>()`

### G — Custom renderers → MAUI Handlers
- [ ] G1. Rewrite `ArcViewRenderer.cs` as `ArcViewHandler.cs`
  - Implement `IViewHandler` for `ArcView`
  - The renderer used `SKCanvasView` (SkiaSharp) — this maps to `SkiaSharp.Views.Maui` handler
- [ ] G2. Rewrite `CustomNavigationPageRenderer.cs` → MAUI `NavigationPage` no longer needs a custom renderer for the status bar behavior; evaluate if still needed
  - If only used for status bar style: use MAUI Shell or `On<iOS>().SetStatusBarTextColorMode()`

### H — XAML files: update namespace declarations
- [ ] H1. All `.xaml` files: replace `xmlns="http://xamarin.com/schemas/2014/forms"` → `xmlns="http://schemas.microsoft.com/dotnet/2021/maui"`
- [ ] H2. All `.xaml` files: remove `xmlns:yummy="clr-namespace:Xamarin.Forms.PancakeView;assembly=Xamarin.Forms.PancakeView"`
- [ ] H3. Affected files with PancakeView (`yummy:PancakeView`): replace with MAUI `Border` + `StrokeShape="RoundRectangle ..."` + gradient brushes
  - `FootpadsView.xaml`
  - `TemperatureView.xaml`
  - `BatteryView.xaml`
  - `AngleView.xaml`
  - `SpeedRangeDistanceView.xaml`
  - `SettingsSwitch.xaml`
  - `FakeNavigationBar.xaml`
  - `BoardListPage.xaml`
  - `SubmitRidePage.xaml`
  - `App.xaml` (global styles)

### I — iOS project: Info.plist and signing
- [ ] I1. Update `CFBundleIdentifier` → `net.wilsonmunoz.owce`
- [ ] I2. Add `beta-reports-active` key (true) directly in Info.plist (was injected by AppCenter script)
- [ ] I3. Update `.csproj` signing: set `ApplicationId`, `ApplicationVersion`, `ApplicationDisplayVersion`, `CodesignKey`, `CodesignProvision`, `DevelopmentTeam = DYWX76A76F`

### J — Build, fix, archive, upload
- [ ] J1. Run `dotnet build OWCE/OWCE.iOS/OWCE.iOS.csproj -f net9.0-ios -c Debug` — fix all compilation errors
- [ ] J2. Run `dotnet build ... -c Release` — fix release-specific issues
- [ ] J3. Run `dotnet publish OWCE/OWCE.iOS/OWCE.iOS.csproj -f net9.0-ios -c Release` → produces `.ipa`
- [ ] J4. Upload to TestFlight via `xcrun altool` or Xcode Organizer

---

## File paths that will change

| File | Change |
|------|--------|
| `OWCE/OWCE.iOS/OWCE.iOS.csproj` | Replaced entirely |
| `OWCE/OWCE.iOS/AppDelegate.cs` | Rewritten |
| `OWCE/OWCE.iOS/Main.cs` | Minor or no change |
| `OWCE/OWCE.iOS/MauiProgram.cs` | New file |
| `OWCE/OWCE.iOS/Info.plist` | Bundle ID, beta key |
| `OWCE/OWCE.iOS/DependencyImplementations/OWBLE.cs` | Remove assembly attribute |
| `OWCE/OWCE.iOS/DependencyImplementations/PermissionPrompt.cs` | Remove assembly attribute |
| `OWCE/OWCE.iOS/DependencyImplementations/UserAgent.cs` | Remove assembly attribute |
| `OWCE/OWCE.iOS/DependencyImplementations/Watch.cs` | Remove assembly attribute |
| `OWCE/OWCE.iOS/Renderers/ArcViewRenderer.cs` | Rewritten as ArcViewHandler.cs |
| `OWCE/OWCE.iOS/Renderers/CustomNavigationPageRenderer.cs` | Evaluate/rewrite |
| `OWCE/OWCE/OWCE.csproj` | All package refs updated |
| `OWCE/OWCE/App.xaml.cs` | ExportFont removed, DI updated |
| `OWCE/OWCE/App.xaml` | XAML namespace updated |
| All 25 `.xaml` files | xmlns updated, PancakeView replaced |
| All 115 `.cs` files (shared) | Xamarin.* usings updated |
| `OWCE.sln` | Android/Mac/Watch removed |

---

## Suggestions (not in scope unless approved)
- Migrate WatchOS to native Swift/SwiftUI watchOS app (separate project)
- Add fastlane Matchfile for cert management
- Wire AppCenter analytics with real key
