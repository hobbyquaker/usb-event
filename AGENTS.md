# Agent Instructions — usb-event

## Project overview
Windows-only .NET 10 WinForms tray application that monitors USB plug/unplug events via WMI and launches or kills configured executables accordingly. Console mode is also supported. 100% vibe-coded.

## Repository layout
```
usb-event/              ← project directory (contains .csproj)
  Program.cs            ← entry point, tray/console runners, WMI watchers, autostart, tray icon drawing
  ConfigEditorForm.cs   ← WinForms dialog: visual card editor + raw YAML mode
  Loc.cs                ← LocStrings record + static Loc class with translations (EN, DE, FR, ES, RU, JA, ZH)
  Logger.cs             ← thread-safe Logger (console with colors or file)
  Models.cs             ← DeviceMapping, AppConfig, ConfigHolder (plain data classes)
  Theme.cs              ← Theme.Dark detection via registry; DarkMenuRenderer + DarkColorTable
  NativeMethods.cs      ← P/Invoke: FreeConsole (kernel32), DwmSetWindowAttribute (dwmapi)
  config.yaml           ← user config, copied to output; must stay beside the .exe at runtime
  usb-event.csproj      ← SDK-style, net10.0-windows, WinForms, NuGets: System.Management + YamlDotNet
README.md               ← user-facing readme (root level)
LICENSE                 ← MIT, Sebastian "hobbyquaker" Raff
```

## Tech stack
- **Runtime:** .NET 10, `net10.0-windows`, WinForms (`UseWindowsForms=true`)
- **NuGet packages:** `System.Management` 10.0.3 (WMI), `YamlDotNet` 16.3.0
- **Windows APIs:** WMI (`Win32_PnPEntity`), Registry (`HKCU`), DWM (dark title bar), kernel32 (`FreeConsole`)
- **Language features:** C# 13, top-level implicit usings, nullable enabled, records, collection expressions `[]`

## Build & run
```bash
dotnet build usb-event/usb-event.csproj
dotnet run --project usb-event/usb-event.csproj              # console mode
dotnet run --project usb-event/usb-event.csproj -- --tray    # tray mode
```
Publish (required for autostart):
```bash
dotnet publish usb-event/usb-event.csproj -c Release -r win-x64 --self-contained false
usb-event.exe --install    # register autostart
usb-event.exe --uninstall  # remove autostart
```

## Key design decisions
- **No namespaces** — all classes are in the global namespace (top-level file style).
- **ConfigHolder** uses `volatile AppConfig Current` for lock-free live reload after config save.
- **WMI watchers** use `WITHIN 2` polling interval; two separate `ManagementEventWatcher` instances for creation and deletion events.
- **USB filter:** `DeviceID` must start with `USB\` OR `PNPClass == USB`.
- **Process lifecycle:** started via `Process.Start` with `UseShellExecute=true`; killed with `Kill(entireProcessTree:true)` on disconnect and on app exit.
- **Localization:** `Loc.Init()` must be called once at startup before any `Loc.T` access. Language is detected from `CultureInfo.CurrentUICulture.TwoLetterISOLanguageName`.
- **Dark mode:** `Theme.Dark` is a static readonly field evaluated once at startup from the registry; `DarkMenuRenderer`/`DarkColorTable` are applied to the tray context menu.
- **Tray icon** is drawn programmatically at runtime (USB trident + gear badge) — no embedded `.ico` resource.

## config.yaml schema
```yaml
devices:
  - deviceId: 'USB\VID_1234&PID_ABCD'   # StartsWith match, instance suffix can be omitted
    executable: 'C:\Path\to\App.exe'
    arguments: '--optional'              # optional
```
The file is read with `YamlDotNet` using `CamelCaseNamingConvention`. It is located beside the `.exe` (or in `AppContext.BaseDirectory` during `dotnet run`).

## Coding conventions
- Align field/variable assignments with spaces for readability.
- Section comments use the `// ── Label ────` style.
- Prefer `using var` and local functions inside methods where appropriate.
- No explicit namespaces; no partial classes.
- All string resources go through `Loc.T` — never hardcode user-visible strings.
- When adding a new `LocStrings` property, add it to **all** seven language blocks in `Loc.cs`.

## What agents should NOT do
- Do not add namespaces to existing files.
- Do not change `config.yaml` content — it is a user file, not a code file.
- Do not add embedded resources or change the project to use `.resx` files.
- Do not change the target framework away from `net10.0-windows`.
- Do not extract `Theme`, `NativeMethods`, or `Loc` into sub-folders or sub-projects.
