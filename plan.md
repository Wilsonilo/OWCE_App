# OWCE — Xamarin → .NET MAUI migration plan (historical)

**Branch:** `maui-migration`

This file is an **archive** of the original migration checklist. For an accurate description of what was implemented, see **[docs/MAUI-MIGRATION.md](docs/MAUI-MIGRATION.md)** and the root **[README.md](README.md)**.

---

## Original approach (summary)

Manual migration (not the .NET upgrade assistant): replace the Xamarin.iOS host with a MAUI iOS host, update the shared project to MAUI packages, rewrite platform seams (custom renderers → handlers, DI registrations), and fix XAML. WatchOS, Android, and macOS were excluded from the MAUI solution scope.

---

## Checklist status

The items below were used during the migration. Many are **completed** on branch `maui-migration`; treat this list as historical context, not a live todo.

### A — Toolchain setup
- [x] Install .NET 9 SDK
- [x] Install `maui-ios` workload
- [x] Verify `dotnet workload list` shows `maui-ios`

### B — Solution restructure
- [x] Replace `OWCE.iOS.csproj` with MAUI SDK-style project targeting `net9.0-ios`
- [x] Remove Android / Mac / Watch from the MAUI solution (legacy folders may remain)
- [x] Update `OWCE/OWCE.csproj` packages and targets for MAUI

### C — Shared project package replacements
- [x] Xamarin.Forms / Essentials / CommunityToolkit → MAUI equivalents
- [x] PancakeView → MAUI `Border` / shapes
- [x] Rg.Plugins.Popup → Mopups

### D — Namespace and API renames
- [x] `Xamarin.Forms` → `Microsoft.Maui.Controls` (and related)

### E — iOS host bootstrap
- [x] `MauiProgram`, `MauiUIApplicationDelegate`, font/image MAUI items

### F — Dependency injection
- [x] Replace `[assembly: Dependency]` with MAUI DI + `App.GetService` where needed

### G — Handlers
- [x] e.g. `ArcView` handler migration

### H — XAML
- [x] MAUI xmlns, PancakeView removal, binding fixes as needed

### I — Signing and bundle ID
- [x] `ApplicationId`, `Info.plist`, automatic provisioning (no committed team secrets)

### J — Build, archive, distribute
- [ ] Ongoing: device testing, Release IPA, TestFlight (maintainer-specific)

---

## Suggestions (optional future work)

- Migrate WatchOS to a separate modern project
- fastlane / Match for certificate management
- Wire App Center only via local `AppConstants` (never commit secrets)
