Onewheel Community Edition (OWCE) App
===========

A cross-platform app for use with the [Onewheel](https://onewheel.com/) V1, Plus, XR, Pint, Pint X and GT.

**This fork** focuses on a **.NET 9 / .NET MAUI iOS** port. Legacy Xamarin.Android, Xamarin.Mac, and watchOS projects remain in the repository for reference but are **not** part of the default MAUI solution.

NOTE: GT support requires patching with [Rewheel](https://github.com/rewheel-app/rewheel).  
Newer board firmware no longer sends through voltage, as Future Motion removed it. Those versions are:
- XR with firmware 4155 and higher
- Pint with firmware 5059 and higher
- Pint X, all firmware
- GT, all firmware

NOTE: Onewheel Community Edition app is not endorsed by or affiliated with Future Motion in any way.

Originally written in C# with Xamarin; the **iOS app in this branch** targets **.NET MAUI**.

Open Source Project by [@beeradmoore](http://www.twitter.com/beeradmoore).

---

## Build prerequisites (MAUI iOS)

- **macOS** with **Xcode** (full app from the App Store, not only Command Line Tools).
- **.NET 9 SDK**.
- MAUI iOS workload:

  ```bash
  dotnet workload restore
  # or, if needed:
  dotnet workload install maui-ios
  ```

- **Apple Developer Program** account for device deployment and App Store / TestFlight (optional for simulator-only).

Open the folder in VS Code or your IDE; the configured solution is **`OWCE/OWCE.sln`**.

---

## Quick commands

Working directory for the commands below: **`OWCE/`** (the folder that contains `OWCE.iOS` and `OWCE`).

### Simulator (Debug, run)

```bash
cd OWCE
dotnet build OWCE.iOS/OWCE.iOS.csproj -c Debug -f net9.0-ios -r iossimulator-arm64 -t:Run
```

Use `iossimulator-x64` on older Intel Macs if required.

### Physical device (Debug, run)

1. List devices (UDID and name):

   ```bash
   M="$(find "$HOME/.dotnet/packs" -path '*Microsoft.iOS.Sdk*/tools/bin/mlaunch' 2>/dev/null | head -1)"
   "$MLAUNCH" --listdev
   ```

2. Run on the device (substitute your device name **or** UDID from the list):

   ```bash
   dotnet build OWCE.iOS/OWCE.iOS.csproj -c Debug -f net9.0-ios -r ios-arm64 -t:Run \
     -p:_DeviceName="YOUR_DEVICE_NAME_OR_UDID"
   ```

If automatic signing fails from the CLI, add `-p:DevelopmentTeam=YOURTEAMID` (10-character Apple Developer Team ID from the Apple Developer portal).

### Release IPA (TestFlight / App Store)

From the **`OWCE/`** directory:

```bash
dotnet publish OWCE.iOS/OWCE.iOS.csproj -f net9.0-ios -c Release -r ios-arm64 \
  -p:ArchiveOnBuild=true \
  -p:CodesignKey="Apple Distribution: Your Name (TEAMID)" \
  -p:CodesignProvision="Your App Store provisioning profile name"
```

The `.ipa` is written under `OWCE.iOS/bin/Release/net9.0-ios/ios-arm64/publish/`. Upload with [Transporter](https://apps.apple.com/app/transporter/id1450874784) or `xcrun altool` / `notarytool` per Apple’s current docs.

Official reference: [Publish a .NET MAUI iOS app using the command line](https://learn.microsoft.com/en-us/dotnet/maui/ios/deployment/publish-cli?view=net-maui-9.0).

---

## Bluetooth and simulator

The app talks to the board over **Bluetooth LE**. **Simulator builds do not exercise real BLE.** For pairing and ride data, use a **physical device** with Bluetooth permission granted.

Discovery is filtered to boards advertising the **Onewheel BLE service UUID**; arbitrary BLE devices will not appear in the list.

---

## Configuration and secrets

- **`OWCE/OWCE/AppConstants.cs`** — optional Syncfusion / App Center keys. Empty strings disable those integrations. The file is safe to commit with empty values; do **not** commit real production secrets into a public fork.
- **Bundle identifier** — set in `OWCE.iOS.csproj` (`ApplicationId`) and `OWCE.iOS/Info.plist`. **Fork maintainers** should use their own identifier and signing assets for App Store distribution.

---

## Migration details

See **[docs/MAUI-MIGRATION.md](docs/MAUI-MIGRATION.md)** for a concise list of technical changes from the original Xamarin project.

---

## Upstream and platforms

The original **OnewheelCommunityEdition** project also targets Android and other platforms. This fork’s **active** development path described here is **iOS / MAUI** only unless contributors revive other targets.

## Frequently (or not so frequently) Asked Questions

### Why did you create this?

There are quite a number of members on the [Onewheel Owners Facebook group](https://www.facebook.com/groups/onewheelownersgroup/) that, for one reason or another, don't like the stock app by Future Motion. I figured, why not create an app with its development shaped by features that the community wants?

### Do other third-party apps already exist?

Yes, but Future Motion's firmware lockdowns prohibit them from being used. Such as pOnewheel [https://github.com/ponewheel/android-ponewheel] (deprecated) for Android and Float Deck (obsolete) for iOS. However, the problem is that one is for Android, and the other is for iOS. Wouldn't it be better if there was just 1 app with the exact same feature sets shared across both platforms?

### Does this change how my Onewheel performs?

No. This app uses the same Bluetooth low energy (BLE) interface that the official Onewheel app uses to read and display various stats.

### What Onewheels are supported?

Currently, v1, Plus, XR, and Pint. Pint X and GT have not yet been thoroughly tested.

### Will using this app void my warranty?

Although things such as riding your board without a helmet can void your warranty, we don't believe that using third-party apps will void your warranty.
