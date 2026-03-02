# Copilot Instructions — usb-event

## Project overview
Windows-only .NET 10 WinForms tray application that monitors USB plug/unplug events via WMI and launches or kills configured executables accordingly. Console mode is also supported. 100% vibe-coded.

## Repository layout
```
usb-event/              ← project directory (contains .csproj)
  Program.cs            ← entry point, tray/console runners, WMI watchers, autostart, tray icon drawing
  ConfigEditorForm.cs   ← WinForms dialog: visual card editor + raw YAML mode
  DeviceHistory.cs      ← tracks recently seen USB devices; persists to device-history.json
  Loc.cs                ← LocStrings sealed class + static Loc class with translations (EN + 21 others)
  Logger.cs             ← thread-safe Logger (console with colors or file)
  Models.cs             ← DeviceMapping, AppConfig, ConfigHolder (plain data classes)
  NativeMethods.cs      ← P/Invoke: FreeConsole (kernel32), DwmSetWindowAttribute (dwmapi)
  Theme.cs              ← Theme.Dark detection via registry; DarkMenuRenderer + DarkColorTable
  config.yaml           ← user config, copied to output; must stay beside the .exe at runtime
  installer.iss         ← Inno Setup script for building the installer artifact
  usb-event.csproj      ← SDK-style, net10.0-windows, WinForms, NuGets: System.Management + YamlDotNet
README.md               ← user-facing readme (root level)
AGENTS.md               ← agent instructions (root level, mirrors this file)
LICENSE                 ← MIT, Sebastian "hobbyquaker" Raff
usb-event.slnx          ← solution file
```

## Tech stack
- **Runtime:** .NET 10, `net10.0-windows`, WinForms (`UseWindowsForms=true`)
- **NuGet packages:** `System.Management` 10.0.3 (WMI), `YamlDotNet` 16.3.0
- **Windows APIs:** WMI (`Win32_PnPEntity`), Registry (`HKCU`), DWM (dark title bar), kernel32 (`FreeConsole`)
- **Language features:** C# 13, top-level implicit usings, nullable enabled, records, collection expressions `[]`

## Build & run
> **Note:** This project targets `net10.0-windows` and requires a Windows environment with WinForms support. Build commands will fail on Linux/macOS runners — this is expected.

```bash
dotnet build usb-event/usb-event.csproj
dotnet run --project usb-event/usb-event.csproj              # console mode
dotnet run --project usb-event/usb-event.csproj -- --tray    # tray mode
```
Publish (required for autostart):
```bash
dotnet publish usb-event/usb-event.csproj -c Release -r win-x64 --self-contained false
usb-event.exe --install    # register autostart in HKCU Run key
usb-event.exe --uninstall  # remove autostart
```

## CI / workflows
- Four release workflows in `.github/workflows/` (`release.yml`, `major-release.yml`, `minor-release.yml`, `patch-release.yml`) triggered by version tags (`v*.*.*`).
- `release.yml` does: checkout → extract version → setup .NET 10 → `dotnet publish` (portable, win-x64, single-file) → Inno Setup installer build → GitHub Release upload.
- Artifacts: `usb-event-vX.Y.Z-portable.exe` and `usb-event-vX.Y.Z-installer.exe`.
- There is **no automated test suite** — no test project exists in this repo.

## Key design decisions
- **No namespaces** — all classes are in the global namespace (top-level file style).
- **ConfigHolder** uses `volatile AppConfig Current` for lock-free live reload after config save.
- **WMI watchers** use `WITHIN 2` polling interval; two separate `ManagementEventWatcher` instances for creation and deletion events.
- **USB filter:** `DeviceID` must start with `USB\` OR `PNPClass == USB`.
- **Process lifecycle:** started via `Process.Start` with `UseShellExecute=true`; killed with `Kill(entireProcessTree:true)` on disconnect and on app exit.
- **Localization:** `Loc.Init()` must be called once at startup before any `Loc.T` access. Language is detected from `CultureInfo.CurrentUICulture.TwoLetterISOLanguageName`.
- **Dark mode:** `Theme.Dark` is a static readonly field evaluated once at startup from the registry; `DarkMenuRenderer`/`DarkColorTable` are applied to the tray context menu and the config editor form.
- **Tray icon** is drawn programmatically at runtime (USB trident + gear badge) — no embedded `.ico` resource.
- **Device history:** `DeviceHistory` stores up to 4 recently seen devices in `device-history.json` next to the config file, used by the dropdown in the visual card editor.
- **Config directory fallback:** if `config.yaml` is next to the .exe and the directory is writable → portable mode; otherwise falls back to `%APPDATA%\usb-event\`.

## config.yaml schema
```yaml
devices:
  - deviceId: 'USB\VID_1234&PID_ABCD'   # StartsWith match, instance suffix can be omitted
    executable: 'C:\Path\to\App.exe'
    arguments: '--optional'              # optional
```
The file is read with `YamlDotNet` using `CamelCaseNamingConvention`. It is located beside the `.exe` (or in `AppContext.BaseDirectory` during `dotnet run`).

## Localization
`Loc.cs` contains `LocStrings` (a sealed class with `required` init-only properties) and a `static class Loc` with one `static LocStrings` field per language.

**Currently supported languages (language code → field name):**
`en` → `En`, `de` → `De`, `fr` → `Fr`, `es` → `Es`, `ru` → `Ru`, `ja` → `Ja`, `zh` → `Zh`, `ar` → `Ar`, `id` → `Id`, `ko` → `Ko`, `pt` → `Pt`, `hi` → `Hi`, `sv` → `Sv`, `no`/`nb`/`nn` → `No`, `fi` → `Fi`, `et` → `Et`, `lv` → `Lv`, `lt` → `Lt`, `pl` → `Pl`, `cs` → `Cs`, `hr` → `Hr`, `sq` → `Sq`.

**Rule:** When adding a new `LocStrings` property, add it to **every single language block** listed above (22 blocks). Never hardcode user-visible strings — always go through `Loc.T`.

## Coding conventions
- Align field/variable assignments with spaces for readability.
- Section comments use the `// ── Label ────` style.
- Prefer `using var` and local functions inside methods where appropriate.
- No explicit namespaces; no partial classes.
- All string resources go through `Loc.T` — never hardcode user-visible strings.
- Format strings for `Loc.T` use `{0}`, `{1}` placeholders consumed via `string.Format(...)`.
- Collection expressions `[]` preferred over `new List<T>()`.
- `record` types preferred for immutable data (e.g., `RecentDevice`, `CardInputs`).

## What agents should NOT do
- Do not add namespaces to existing files.
- Do not change `config.yaml` content — it is a user file, not a code file.
- Do not add embedded resources or change the project to use `.resx` files.
- Do not change the target framework away from `net10.0-windows`.
- Do not extract `Theme`, `NativeMethods`, or `Loc` into sub-folders or sub-projects.
- Do not create a test project — there is no existing test infrastructure.
- Do not add or bump NuGet packages unless the task explicitly requires it.

## Known environment limitations
- The sandbox/CI environment is Linux; `dotnet build` for this Windows-only WinForms project will fail with "The current platform does not support building project" on non-Windows machines. This is expected and not a code error.
- WMI features (`System.Management`) are Windows-only; any runtime features depending on WMI cannot be tested outside Windows.
