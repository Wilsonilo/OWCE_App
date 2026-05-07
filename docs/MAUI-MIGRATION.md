# Xamarin.Forms → .NET MAUI (iOS) migration

This document describes the **MAUI iOS** work on this fork relative to the original **Xamarin.Forms** multi-platform repository ([OnewheelCommunityEdition/OWCE_App](https://github.com/OnewheelCommunityEdition/OWCE_App)).

## Scope

- **In scope:** iOS app as a **.NET 9 MAUI** host (`net9.0-ios`), shared UI/library project (`OWCE`), handlers instead of renderers, MAUI DI instead of `DependencyService`.
- **Out of scope (for this fork):** Android, macOS, and watchOS targets remain in the tree as legacy Xamarin projects but are **not** referenced by the trimmed solution used for MAUI builds.

## High-level changes

| Area | Before (Xamarin) | After (MAUI) |
|------|------------------|--------------|
| Host | `OWCE.iOS` classic Xamarin.iOS + Forms | `OWCE.iOS` SDK-style, `UseMaui`, `MauiProgram` + `MauiUIApplicationDelegate` |
| Shared UI | `Xamarin.Forms` packages | `Microsoft.Maui.Controls`, `CommunityToolkit.Maui`, `Mopups` |
| PancakeView | `Xamarin.Forms.PancakeView` | `Border`, gradients, `RoundRectangle` |
| Popups | `Rg.Plugins.Popup` | `Mopups` |
| Skia | `SkiaSharp.Views.Forms` | `SkiaSharp.Views.Maui.Controls` |
| Fonts | `ExportFont` attributes + iOS `UIAppFonts` | MAUI `MauiFont` items (fonts consolidated on the iOS head project to avoid double registration) |
| Images | Legacy bundle names | `MauiImage` under `OWCE.iOS/Resources/Images/`, referenced in XAML as `Images/filename.png` |
| Platform glue | `[assembly: Dependency]` | `MauiProgram` service registration + `App.GetService<T>()` where needed |
| BLE | `OWBLE` + `DependencyService` | Same implementation class; resolved via DI / `App.GetService` |
| App Center | Packages always referenced | On **simulator** RID, `DISABLE_APPCENTER` is defined and App Center packages are **excluded** (NuGet limitation); device builds include App Center. Startup is in `AppCenterStartup.cs`; secrets stay in `AppConstants` (empty = no-op). |

## Behavioral notes

- **Bluetooth discovery** only lists devices that advertise the Onewheel GATT service (`OWBoard.ServiceUUID`). Random BLE peripherals will not appear; a powered board in range is required for meaningful testing.
- **Simulator:** UI and much of the app run; BLE is not available. `OWBLE` surfaces a clear “not supported on simulator” style message where applicable.
- **Signing:** The repository uses **automatic** provisioning in the project file. Each maintainer must use their own Apple Developer account, team, and bundle identifier policy (see README).

## Solution layout

From the repository root, the MAUI solution is:

- `OWCE/OWCE.sln` — **OWCE** (library) + **OWCE.iOS** (MAUI head).

Build commands should target **`OWCE.iOS/OWCE.iOS.csproj`**, not the whole solution, when using `dotnet publish` (Microsoft recommends publishing the app project directly).

## VS Code / CLI ergonomics

The repo includes `.vscode/tasks.json` (repository root) with:

- List connected devices via `mlaunch --listdev`
- Build and run on **iOS Simulator** (`iossimulator-arm64`)
- Build and run on a **physical device** using `-p:_DeviceName=…`

See the main [README](../README.md) and your local developer handbook for exact commands.

## Historical plan

An earlier step-by-step migration checklist lived in `plan.md` at the root; it may be partially outdated. This file is the accurate summary of what was implemented.
