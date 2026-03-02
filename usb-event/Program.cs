using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    const string AppDisplayName = "USB Device Monitor";
    const string RegistryRunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    [STAThread]
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Loc.Init();

        if (args.Contains("--install"))   { InstallAutostart();   return; }
        if (args.Contains("--uninstall")) { UninstallAutostart(); return; }

        var trayMode   = args.Contains("--tray");
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.yaml");
        var config     = LoadConfig(configPath);
        var running    = new ConcurrentDictionary<string, Process>(StringComparer.OrdinalIgnoreCase);

        if (trayMode)
            RunAsTray(config, running, configPath);
        else
            RunAsConsole(config, configPath, running);
    }

    // ── Tray-Modus ─────────────────────────────────────────────────────────────

    static void RunAsTray(AppConfig config, ConcurrentDictionary<string, Process> running, string configPath)
    {
        var dir     = Path.GetDirectoryName(configPath)!;
        var logPath = Path.Combine(dir, "usb-event.log");
        using var logger = new Logger(toConsole: false, logFile: logPath);

        NativeMethods.FreeConsole();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var holder = new ConfigHolder(config);

        using var trayIconImage = CreateTrayIcon();
        using var trayIcon = new NotifyIcon
        {
            Icon    = trayIconImage,
            Text    = Loc.T.AppName,
            Visible = true
        };

        var menu   = new ContextMenuStrip();
        var header = new ToolStripMenuItem(Loc.T.AppName) { Enabled = false };
        menu.Items.Add(header);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(Loc.T.MenuEditConfig, null, (_, _) =>
        {
            using var editor = new ConfigEditorForm(configPath,
                () => holder.Current = LoadConfig(configPath));
            editor.ShowDialog();
        });
        menu.Items.Add(Loc.T.MenuOpenFolder, null, (_, _) =>
            Process.Start(new ProcessStartInfo("explorer.exe", dir) { UseShellExecute = true }));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(Loc.T.MenuExit, null, (_, _) =>
        {
            trayIcon.Visible = false;
            Application.Exit();
        });
        if (Theme.Dark) menu.Renderer = new DarkMenuRenderer();
        trayIcon.ContextMenuStrip = menu;

        using var insertWatcher = CreateWatcher("__InstanceCreationEvent", holder, running, trayIcon, logger, isConnect: true);
        using var removeWatcher = CreateWatcher("__InstanceDeletionEvent", holder, running, trayIcon, logger, isConnect: false);
        insertWatcher.Start();
        removeWatcher.Start();

        Application.Run();

        insertWatcher.Stop();
        removeWatcher.Stop();
        KillAll(running);
    }

    // ── Konsolen-Modus ─────────────────────────────────────────────────────────

    static void RunAsConsole(AppConfig config, string configPath, ConcurrentDictionary<string, Process> running)
    {
        if (config.Devices.Count == 0)
        {
        Console.WriteLine(Loc.T.MissingMappings);
        Console.WriteLine($"{Loc.T.ConfigFile}: {configPath}");
            return;
        }

        Console.WriteLine($"{Loc.T.Started}: {Loc.T.ExitHint}");
        Console.WriteLine($"{Loc.T.ConfigCount}: {config.Devices.Count} | {Loc.T.ConfigFile}: {configPath}");
        Console.WriteLine(new string('─', 60));

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        var holder = new ConfigHolder(config);
        using var logger = new Logger(toConsole: true);
        using var insertWatcher = CreateWatcher("__InstanceCreationEvent", holder, running, null, logger, isConnect: true);
        using var removeWatcher = CreateWatcher("__InstanceDeletionEvent", holder, running, null, logger, isConnect: false);
        insertWatcher.Start();
        removeWatcher.Start();

        cts.Token.WaitHandle.WaitOne();

        insertWatcher.Stop();
        removeWatcher.Stop();
        KillAll(running);

        Console.WriteLine($"\n{Loc.T.Stopped}");
    }

    // ── WMI-Watcher ────────────────────────────────────────────────────────────

    static ManagementEventWatcher CreateWatcher(
        string eventType,
        ConfigHolder holder,
        ConcurrentDictionary<string, Process> running,
        NotifyIcon? trayIcon,
        Logger? logger,
        bool isConnect)
    {
        var query   = new WqlEventQuery($"SELECT * FROM {eventType} WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
        var watcher = new ManagementEventWatcher(query);

        watcher.EventArrived += (_, e) =>
        {
            var device = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            if (!IsUsbDevice(device)) return;

            var deviceId = device["DeviceID"]?.ToString() ?? string.Empty;
            var mapping  = FindMapping(holder.Current, deviceId);

            var color = isConnect ? ConsoleColor.Green : ConsoleColor.Red;
            logger?.Write($"\n[{DateTime.Now:HH:mm:ss}] {(isConnect ? "▶" : "◀")} {Loc.T.UsbDevice} {(isConnect ? Loc.T.Connected : Loc.T.Disconnected)}", color);
            logger?.Write(FormatDeviceInfo(device), color);

            if (mapping is null) return;

            if (isConnect)
                StartProcess(mapping, running, logger, trayIcon);
            else
                StopProcess(mapping, running, logger, trayIcon);
        };

        return watcher;
    }

    // ── Prozessverwaltung ──────────────────────────────────────────────────────

    static void StartProcess(DeviceMapping mapping, ConcurrentDictionary<string, Process> running, Logger? logger, NotifyIcon? trayIcon)
    {
        if (running.TryGetValue(mapping.DeviceId, out var existing) && !existing.HasExited)
        {
            logger?.Write(string.Format(Loc.T.ProcessAlreadyRunning, existing.Id, Path.GetFileName(mapping.Executable)), ConsoleColor.DarkCyan);
            return;
        }

        try
        {
            var process = Process.Start(new ProcessStartInfo(mapping.Executable)
            {
                Arguments       = mapping.Arguments ?? string.Empty,
                UseShellExecute = true
            });

            if (process is not null)
            {
                running[mapping.DeviceId] = process;
                logger?.Write(string.Format(Loc.T.ProcessStarted, Path.GetFileName(mapping.Executable), process.Id), ConsoleColor.Cyan);
                trayIcon?.ShowBalloonTip(3000, Loc.T.AppName, string.Format(Loc.T.ProcessStartedShort, Path.GetFileName(mapping.Executable)), ToolTipIcon.Info);
            }
        }
        catch (Exception ex)
        {
            logger?.Write(string.Format(Loc.T.ProcessStartError, ex.Message), ConsoleColor.Yellow);
            trayIcon?.ShowBalloonTip(3000, Loc.T.AppName, ex.Message, ToolTipIcon.Error);
        }
    }

    static void StopProcess(DeviceMapping mapping, ConcurrentDictionary<string, Process> running, Logger? logger, NotifyIcon? trayIcon)
    {
        if (!running.TryRemove(mapping.DeviceId, out var process)) return;

        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                logger?.Write(string.Format(Loc.T.ProcessStopped, Path.GetFileName(mapping.Executable), process.Id), ConsoleColor.Cyan);
                trayIcon?.ShowBalloonTip(3000, Loc.T.AppName, string.Format(Loc.T.ProcessStoppedShort, Path.GetFileName(mapping.Executable)), ToolTipIcon.Info);
            }
        }
        catch (Exception ex)
        {
            logger?.Write(string.Format(Loc.T.ProcessStopError, ex.Message), ConsoleColor.Yellow);
            trayIcon?.ShowBalloonTip(3000, Loc.T.AppName, ex.Message, ToolTipIcon.Error);
        }
    }

    static void KillAll(ConcurrentDictionary<string, Process> running)
    {
        foreach (var (_, p) in running)
            try { p.Kill(entireProcessTree: true); } catch { }
    }

    // ── Autostart ──────────────────────────────────────────────────────────────

    static void InstallAutostart()
    {
        var exePath = Environment.ProcessPath
            ?? throw new InvalidOperationException(Loc.T.CannotResolveProcessPath);

        if (Path.GetFileName(exePath).Equals("dotnet.exe", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(Loc.T.AutostartDotnetWarning);
            Console.WriteLine(Loc.T.AutostartPublishHint);
            return;
        }

        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true)
            ?? throw new InvalidOperationException(Loc.T.RegistryNotAccessible);

        key.SetValue(AppDisplayName, $"\"{exePath}\" --tray");
        Console.WriteLine(string.Format(Loc.T.AutostartInstalled, exePath));
        Console.WriteLine(Loc.T.AutostartInfo);
    }

    static void UninstallAutostart()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true);
        key?.DeleteValue(AppDisplayName, throwOnMissingValue: false);
        Console.WriteLine(Loc.T.AutostartRemoved);
    }

    // ── Hilfsmethoden ──────────────────────────────────────────────────────────

    static AppConfig LoadConfig(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Konfigurationsdatei nicht gefunden: {path}");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using var reader = new StreamReader(path);
        return deserializer.Deserialize<AppConfig>(reader) ?? new AppConfig();
    }

    static DeviceMapping? FindMapping(AppConfig config, string deviceId) =>
        config.Devices.FirstOrDefault(m =>
            deviceId.StartsWith(m.DeviceId, StringComparison.OrdinalIgnoreCase));

    static bool IsUsbDevice(ManagementBaseObject device)
    {
        var deviceId = device["DeviceID"]?.ToString() ?? string.Empty;
        var pnpClass = device["PNPClass"]?.ToString() ?? string.Empty;
        return deviceId.StartsWith("USB\\", StringComparison.OrdinalIgnoreCase)
            || pnpClass.Equals("USB", StringComparison.OrdinalIgnoreCase);
    }

    static string FormatDeviceInfo(ManagementBaseObject device)
    {
        var fields = new (string Label, string Property)[]
        {
            ("Name",         "Name"),
            ("Beschreibung", "Description"),
            ("Geräteklasse", "PNPClass"),
            ("Hersteller",   "Manufacturer"),
            ("Geräte-ID",    "DeviceID"),
            ("Status",       "Status"),
        };

        var sb = new System.Text.StringBuilder();
        foreach (var (label, property) in fields)
        {
            var value = device[property]?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
                sb.AppendLine($"  {label,-14}: {value}");
        }
        sb.Append(new string('─', 60));
        return sb.ToString();
    }

    static Icon CreateTrayIcon()
    {
        const int S = 48;
        using var bmp = new Bitmap(S, S);
        using var g   = Graphics.FromImage(bmp);
        g.SmoothingMode   = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.Clear(Color.Transparent);

        var blue = Color.FromArgb(0, 114, 196);
        var gold = Color.FromArgb(224, 148, 0);

        using var blueBrush = new SolidBrush(blue);

        // ── USB-Trident ────────────────────────────────────────────────────
        // Dreieck (Spitze oben, Mitte des Symbols)
        g.FillPolygon(blueBrush, new PointF[] { new(17, 3), new(10, 14), new(24, 14) });

        // Linkes Quadrat
        g.FillRectangle(blueBrush, 2, 12, 8, 8);

        // Rechter Kreis
        g.FillEllipse(blueBrush, 22, 12, 9, 9);

        // Linker Ast (vertikal, vom Quadrat zur Querlinie)
        g.FillRectangle(blueBrush, 4, 20, 4, 8);

        // Rechter Ast (vertikal, vom Kreis zur Querlinie)
        g.FillRectangle(blueBrush, 24, 21, 4, 7);

        // Querlinie
        g.FillRectangle(blueBrush, 2, 28, 28, 3);

        // Hauptschaft (vertikal, von Dreieckbasis bis Stecker)
        g.FillRectangle(blueBrush, 14, 14, 6, 22);

        // Stecker-Gehäuse (USB-A Rechteck)
        g.FillRectangle(blueBrush, 10, 35, 14, 10);

        // Stecker-Öffnung (heller Ausschnitt)
        using var slotBrush = new SolidBrush(Color.FromArgb(190, 225, 250));
        g.FillRectangle(slotBrush, 12, 37, 10, 6);

        // ── Zahnrad (unten rechts als Badge) ──────────────────────────────
        // Heller Trennring damit das Zahnrad sich vom Trident absetzt
        using var ringBrush = new SolidBrush(Color.FromArgb(245, 248, 252));
        g.FillEllipse(ringBrush, 26, 27, 22, 22);

        DrawGear(g, 37f, 38f, 9f, 4.5f, 8, gold);

        return Icon.FromHandle(bmp.GetHicon());
    }

    static void DrawGear(Graphics g, float cx, float cy, float bodyR, float holeR, int teeth, Color color)
    {
        float  toothH = bodyR * 0.40f;
        double toothW = Math.PI * 2 / teeth * 0.32;

        using var path = new GraphicsPath();
        path.FillMode = FillMode.Alternate;

        var pts = new PointF[teeth * 4];
        for (int i = 0; i < teeth; i++)
        {
            double a  = Math.PI * 2.0 / teeth * i;
            pts[i*4]   = Polar(cx, cy, bodyR,         a - toothW);
            pts[i*4+1] = Polar(cx, cy, bodyR + toothH, a - toothW * 0.35);
            pts[i*4+2] = Polar(cx, cy, bodyR + toothH, a + toothW * 0.35);
            pts[i*4+3] = Polar(cx, cy, bodyR,         a + toothW);
        }
        path.AddPolygon(pts);
        path.AddEllipse(cx - holeR, cy - holeR, holeR * 2, holeR * 2);

        using var brush = new SolidBrush(color);
        g.FillPath(brush, path);
    }

    static PointF Polar(float cx, float cy, float r, double angle) =>
        new((float)(cx + r * Math.Cos(angle)), (float)(cy + r * Math.Sin(angle)));
}

// ── Localization ─────────────────────────────────────────────────────────────

sealed class LocStrings
{
    public required string AppName { get; init; }
    public required string MenuEditConfig { get; init; }
    public required string MenuOpenFolder { get; init; }
    public required string MenuExit { get; init; }

    public required string MissingMappings { get; init; }
    public required string ConfigFile { get; init; }
    public required string Started { get; init; }
    public required string ExitHint { get; init; }
    public required string ConfigCount { get; init; }
    public required string Stopped { get; init; }

    public required string UsbDevice { get; init; }
    public required string Connected { get; init; }
    public required string Disconnected { get; init; }

    public required string ProcessAlreadyRunning { get; init; }
    public required string ProcessStarted { get; init; }
    public required string ProcessStartedShort { get; init; }
    public required string ProcessStartError { get; init; }
    public required string ProcessStopped { get; init; }
    public required string ProcessStoppedShort { get; init; }
    public required string ProcessStopError { get; init; }

    public required string CannotResolveProcessPath { get; init; }
    public required string AutostartDotnetWarning { get; init; }
    public required string AutostartPublishHint { get; init; }
    public required string RegistryNotAccessible { get; init; }
    public required string AutostartInstalled { get; init; }
    public required string AutostartInfo { get; init; }
    public required string AutostartRemoved { get; init; }

    public required string RawLabel { get; init; }
    public required string BtnSave { get; init; }
    public required string BtnCancel { get; init; }
    public required string AddDevice { get; init; }
    public required string BrowseTitle { get; init; }
    public required string BrowseFilter { get; init; }
    public required string LabelDeviceId { get; init; }
    public required string LabelProgram { get; init; }
    public required string LabelArgs { get; init; }
    public required string DeleteConfirmTitle { get; init; }
    public required string DeleteConfirmBody { get; init; }
    public required string ErrorTitle { get; init; }
    public required string SaveError { get; init; }
}

static class Loc
{
    public static LocStrings T { get; private set; } = null!;

    public static void Init()
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        T = lang switch
        {
            "de" => De,
            "fr" => Fr,
            "es" => Es,
            "ru" => Ru,
            "ja" => Ja,
            "zh" => Zh,
            _     => En,
        };
    }

    static LocStrings En => new()
    {
        AppName = "USB Device Monitor",
        MenuEditConfig = "Edit configuration…",
        MenuOpenFolder = "Open config/log directory",
        MenuExit = "Exit",

        MissingMappings = "No device mappings found in configuration.",
        ConfigFile = "Config file",
        Started = "USB Device Monitor started",
        ExitHint = "Press Ctrl+C to exit",
        ConfigCount = "Configuration entries",
        Stopped = "Application stopped.",

        UsbDevice = "USB device",
        Connected = "connected",
        Disconnected = "disconnected",

        ProcessAlreadyRunning = "  Process already running (PID {0}): {1}",
        ProcessStarted = "  ▶ Process started: {0} (PID {1})",
        ProcessStartedShort = "Started: {0}",
        ProcessStartError = "  ✖ Error starting process: {0}",
        ProcessStopped = "  ◀ Process stopped: {0} (PID {1})",
        ProcessStoppedShort = "Stopped: {0}",
        ProcessStopError = "  ✖ Error stopping process: {0}",

        CannotResolveProcessPath = "Could not determine process path.",
        AutostartDotnetWarning = "⚠ Autostart can only be installed with the published .exe.",
        AutostartPublishHint = "  Please run 'dotnet publish' first.",
        RegistryNotAccessible = "Registry key not accessible.",
        AutostartInstalled = "✔ Autostart registered: {0} --tray",
        AutostartInfo = "  The app will start in the tray at next login.",
        AutostartRemoved = "✔ Autostart entry removed.",

        RawLabel = "Raw YAML",
        BtnSave = "Save",
        BtnCancel = "Cancel",
        AddDevice = "+ Add device",
        BrowseTitle = "Select executable",
        BrowseFilter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
        LabelDeviceId = "Device ID:",
        LabelProgram = "Executable:",
        LabelArgs = "Arguments:",
        DeleteConfirmTitle = "Confirm delete",
        DeleteConfirmBody = "Delete this device mapping?",
        ErrorTitle = "Error",
        SaveError = "Error while saving:\n{0}",
    };

    static LocStrings De => new()
    {
        AppName = "USB-Geräte-Monitor",
        MenuEditConfig = "Konfiguration bearbeiten…",
        MenuOpenFolder = "Konfig-/Logverzeichnis öffnen",
        MenuExit = "Beenden",

        MissingMappings = "Keine Gerätezuordnungen in der Konfigurationsdatei gefunden.",
        ConfigFile = "Konfigurationsdatei",
        Started = "USB-Geräte-Monitor gestartet",
        ExitHint = "Strg+C zum Beenden",
        ConfigCount = "Konfigurationseinträge",
        Stopped = "Programm beendet.",

        UsbDevice = "USB-Gerät",
        Connected = "angeschlossen",
        Disconnected = "getrennt",

        ProcessAlreadyRunning = "  Prozess läuft bereits (PID {0}): {1}",
        ProcessStarted = "  ▶ Prozess gestartet: {0} (PID {1})",
        ProcessStartedShort = "Gestartet: {0}",
        ProcessStartError = "  ✖ Fehler beim Starten: {0}",
        ProcessStopped = "  ◀ Prozess beendet: {0} (PID {1})",
        ProcessStoppedShort = "Beendet: {0}",
        ProcessStopError = "  ✖ Fehler beim Beenden: {0}",

        CannotResolveProcessPath = "Pfad des Prozesses konnte nicht ermittelt werden.",
        AutostartDotnetWarning = "⚠ Autostart kann nur mit der veröffentlichten .exe eingerichtet werden.",
        AutostartPublishHint = "  Führen Sie zuerst 'dotnet publish' aus.",
        RegistryNotAccessible = "Registrierungsschlüssel nicht zugänglich.",
        AutostartInstalled = "✔ Autostart eingerichtet: {0} --tray",
        AutostartInfo = "  Die Anwendung startet beim nächsten Login automatisch im Hintergrund.",
        AutostartRemoved = "✔ Autostart-Eintrag entfernt.",

        RawLabel = "Raw YAML",
        BtnSave = "Speichern",
        BtnCancel = "Abbrechen",
        AddDevice = "+ Gerät hinzufügen",
        BrowseTitle = "Ausführbare Datei auswählen",
        BrowseFilter = "Ausführbare Dateien (*.exe)|*.exe|Alle Dateien (*.*)|*.*",
        LabelDeviceId = "Geräte-ID:",
        LabelProgram = "Programm:",
        LabelArgs = "Argumente:",
        DeleteConfirmTitle = "Löschen bestätigen",
        DeleteConfirmBody = "Diese Gerätezuordnung wirklich löschen?",
        ErrorTitle = "Fehler",
        SaveError = "Fehler beim Speichern:\n{0}",
    };

    static LocStrings Fr => new()
    {
        AppName = "Moniteur USB",
        MenuEditConfig = "Modifier la configuration…",
        MenuOpenFolder = "Ouvrir le dossier config/log",
        MenuExit = "Quitter",

        MissingMappings = "Aucun mappage d'appareil dans la configuration.",
        ConfigFile = "Fichier de config",
        Started = "Moniteur USB démarré",
        ExitHint = "Appuyez sur Ctrl+C pour quitter",
        ConfigCount = "Entrées de configuration",
        Stopped = "Application arrêtée.",

        UsbDevice = "Périphérique USB",
        Connected = "connecté",
        Disconnected = "déconnecté",

        ProcessAlreadyRunning = "  Processus déjà en cours (PID {0}) : {1}",
        ProcessStarted = "  ▶ Processus démarré : {0} (PID {1})",
        ProcessStartedShort = "Démarré : {0}",
        ProcessStartError = "  ✖ Erreur au démarrage : {0}",
        ProcessStopped = "  ◀ Processus arrêté : {0} (PID {1})",
        ProcessStoppedShort = "Arrêté : {0}",
        ProcessStopError = "  ✖ Erreur à l'arrêt : {0}",

        CannotResolveProcessPath = "Impossible de déterminer le chemin du processus.",
        AutostartDotnetWarning = "⚠ L'autostart ne peut être installé qu'avec l'exécutable publié.",
        AutostartPublishHint = "  Exécutez d'abord 'dotnet publish'.",
        RegistryNotAccessible = "Clé de registre inaccessible.",
        AutostartInstalled = "✔ Autostart enregistré : {0} --tray",
        AutostartInfo = "  L'application démarrera dans le tray à la prochaine ouverture de session.",
        AutostartRemoved = "✔ Entrée d'autostart supprimée.",

        RawLabel = "YAML brut",
        BtnSave = "Enregistrer",
        BtnCancel = "Annuler",
        AddDevice = "+ Ajouter un appareil",
        BrowseTitle = "Sélectionner un exécutable",
        BrowseFilter = "Fichiers exécutables (*.exe)|*.exe|Tous les fichiers (*.*)|*.*",
        LabelDeviceId = "ID de l'appareil :",
        LabelProgram = "Exécutable :",
        LabelArgs = "Arguments :",
        DeleteConfirmTitle = "Confirmer la suppression",
        DeleteConfirmBody = "Supprimer ce mappage d'appareil ?",
        ErrorTitle = "Erreur",
        SaveError = "Erreur lors de l'enregistrement:\n{0}",
    };

    static LocStrings Es => new()
    {
        AppName = "Monitor de dispositivos USB",
        MenuEditConfig = "Editar configuración…",
        MenuOpenFolder = "Abrir carpeta de config/log",
        MenuExit = "Salir",

        MissingMappings = "No se encontraron asignaciones de dispositivos en la configuración.",
        ConfigFile = "Archivo de configuración",
        Started = "Monitor USB iniciado",
        ExitHint = "Presione Ctrl+C para salir",
        ConfigCount = "Entradas de configuración",
        Stopped = "Aplicación detenida.",

        UsbDevice = "Dispositivo USB",
        Connected = "conectado",
        Disconnected = "desconectado",

        ProcessAlreadyRunning = "  El proceso ya se está ejecutando (PID {0}): {1}",
        ProcessStarted = "  ▶ Proceso iniciado: {0} (PID {1})",
        ProcessStartedShort = "Iniciado: {0}",
        ProcessStartError = "  ✖ Error al iniciar: {0}",
        ProcessStopped = "  ◀ Proceso detenido: {0} (PID {1})",
        ProcessStoppedShort = "Detenido: {0}",
        ProcessStopError = "  ✖ Error al detener: {0}",

        CannotResolveProcessPath = "No se pudo determinar la ruta del proceso.",
        AutostartDotnetWarning = "⚠ El inicio automático solo funciona con el .exe publicado.",
        AutostartPublishHint = "  Ejecute primero 'dotnet publish'.",
        RegistryNotAccessible = "Clave de registro no accesible.",
        AutostartInstalled = "✔ Inicio automático registrado: {0} --tray",
        AutostartInfo = "  La app se iniciará en el tray en el próximo inicio de sesión.",
        AutostartRemoved = "✔ Entrada de inicio automático eliminada.",

        RawLabel = "YAML sin formato",
        BtnSave = "Guardar",
        BtnCancel = "Cancelar",
        AddDevice = "+ Añadir dispositivo",
        BrowseTitle = "Seleccionar ejecutable",
        BrowseFilter = "Archivos ejecutables (*.exe)|*.exe|Todos los archivos (*.*)|*.*",
        LabelDeviceId = "ID del dispositivo:",
        LabelProgram = "Ejecutable:",
        LabelArgs = "Argumentos:",
        DeleteConfirmTitle = "Confirmar eliminación",
        DeleteConfirmBody = "¿Eliminar esta asignación de dispositivo?",
        ErrorTitle = "Error",
        SaveError = "Error al guardar:\n{0}",
    };

    static LocStrings Ru => new()
    {
        AppName = "USB-монитор устройств",
        MenuEditConfig = "Изменить конфигурацию…",
        MenuOpenFolder = "Открыть каталог config/log",
        MenuExit = "Выход",

        MissingMappings = "В конфигурации нет привязок устройств.",
        ConfigFile = "Файл конфигурации",
        Started = "USB-монитор запущен",
        ExitHint = "Нажмите Ctrl+C для выхода",
        ConfigCount = "Записей конфигурации",
        Stopped = "Приложение остановлено.",

        UsbDevice = "USB-устройство",
        Connected = "подключено",
        Disconnected = "отключено",

        ProcessAlreadyRunning = "  Процесс уже запущен (PID {0}): {1}",
        ProcessStarted = "  ▶ Процесс запущен: {0} (PID {1})",
        ProcessStartedShort = "Запущено: {0}",
        ProcessStartError = "  ✖ Ошибка запуска: {0}",
        ProcessStopped = "  ◀ Процесс остановлен: {0} (PID {1})",
        ProcessStoppedShort = "Остановлено: {0}",
        ProcessStopError = "  ✖ Ошибка остановки: {0}",

        CannotResolveProcessPath = "Не удалось определить путь процесса.",
        AutostartDotnetWarning = "⚠ Автозапуск возможен только с опубликованным .exe.",
        AutostartPublishHint = "  Сначала выполните 'dotnet publish'.",
        RegistryNotAccessible = "Ключ реестра недоступен.",
        AutostartInstalled = "✔ Автозапуск зарегистрирован: {0} --tray",
        AutostartInfo = "  Приложение запустится в трее при следующем входе.",
        AutostartRemoved = "✔ Запись автозапуска удалена.",

        RawLabel = "Raw YAML",
        BtnSave = "Сохранить",
        BtnCancel = "Отмена",
        AddDevice = "+ Добавить устройство",
        BrowseTitle = "Выберите исполняемый файл",
        BrowseFilter = "Исполняемые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*",
        LabelDeviceId = "ID устройства:",
        LabelProgram = "Исполняемый файл:",
        LabelArgs = "Аргументы:",
        DeleteConfirmTitle = "Подтверждение удаления",
        DeleteConfirmBody = "Удалить эту привязку устройства?",
        ErrorTitle = "Ошибка",
        SaveError = "Ошибка сохранения:\n{0}",
    };

    static LocStrings Ja => new()
    {
        AppName = "USBデバイスモニター",
        MenuEditConfig = "設定を編集…",
        MenuOpenFolder = "設定/ログフォルダーを開く",
        MenuExit = "終了",

        MissingMappings = "設定にデバイスの割り当てがありません。",
        ConfigFile = "設定ファイル",
        Started = "USBデバイスモニターを開始",
        ExitHint = "終了するには Ctrl+C",
        ConfigCount = "設定エントリ数",
        Stopped = "アプリケーションを終了しました。",

        UsbDevice = "USBデバイス",
        Connected = "接続",
        Disconnected = "切断",

        ProcessAlreadyRunning = "  プロセスは既に実行中です (PID {0}): {1}",
        ProcessStarted = "  ▶ プロセス開始: {0} (PID {1})",
        ProcessStartedShort = "開始: {0}",
        ProcessStartError = "  ✖ 起動エラー: {0}",
        ProcessStopped = "  ◀ プロセス終了: {0} (PID {1})",
        ProcessStoppedShort = "終了: {0}",
        ProcessStopError = "  ✖ 終了エラー: {0}",

        CannotResolveProcessPath = "プロセスのパスを特定できません。",
        AutostartDotnetWarning = "⚠ 自動起動は公開済みの .exe でのみ設定できます。",
        AutostartPublishHint = "  先に 'dotnet publish' を実行してください。",
        RegistryNotAccessible = "レジストリキーにアクセスできません。",
        AutostartInstalled = "✔ 自動起動を登録: {0} --tray",
        AutostartInfo = "  次回ログイン時にトレイで自動起動します。",
        AutostartRemoved = "✔ 自動起動エントリを削除しました。",

        RawLabel = "Raw YAML",
        BtnSave = "保存",
        BtnCancel = "キャンセル",
        AddDevice = "+ デバイスを追加",
        BrowseTitle = "実行ファイルを選択",
        BrowseFilter = "実行ファイル (*.exe)|*.exe|すべてのファイル (*.*)|*.*",
        LabelDeviceId = "デバイスID:",
        LabelProgram = "実行ファイル:",
        LabelArgs = "引数:",
        DeleteConfirmTitle = "削除の確認",
        DeleteConfirmBody = "このデバイスの割り当てを削除しますか？",
        ErrorTitle = "エラー",
        SaveError = "保存中にエラーが発生しました:\n{0}",
    };

    static LocStrings Zh => new()
    {
        AppName = "USB 设备监控",
        MenuEditConfig = "编辑配置…",
        MenuOpenFolder = "打开配置/日志目录",
        MenuExit = "退出",

        MissingMappings = "配置中未找到设备映射。",
        ConfigFile = "配置文件",
        Started = "USB 设备监控已启动",
        ExitHint = "按 Ctrl+C 退出",
        ConfigCount = "配置条目",
        Stopped = "应用已停止。",

        UsbDevice = "USB 设备",
        Connected = "已连接",
        Disconnected = "已断开",

        ProcessAlreadyRunning = "  进程已在运行 (PID {0}): {1}",
        ProcessStarted = "  ▶ 进程已启动: {0} (PID {1})",
        ProcessStartedShort = "已启动: {0}",
        ProcessStartError = "  ✖ 启动出错: {0}",
        ProcessStopped = "  ◀ 进程已停止: {0} (PID {1})",
        ProcessStoppedShort = "已停止: {0}",
        ProcessStopError = "  ✖ 停止出错: {0}",

        CannotResolveProcessPath = "无法确定进程路径。",
        AutostartDotnetWarning = "⚠ 仅发布后的 .exe 才能设置自启动。",
        AutostartPublishHint = "  请先执行 'dotnet publish'。",
        RegistryNotAccessible = "无法访问注册表键。",
        AutostartInstalled = "✔ 已注册自启动: {0} --tray",
        AutostartInfo = "  应用将在下次登录时在托盘中启动。",
        AutostartRemoved = "✔ 已移除自启动项。",

        RawLabel = "原始 YAML",
        BtnSave = "保存",
        BtnCancel = "取消",
        AddDevice = "+ 添加设备",
        BrowseTitle = "选择可执行文件",
        BrowseFilter = "可执行文件 (*.exe)|*.exe|所有文件 (*.*)|*.*",
        LabelDeviceId = "设备 ID:",
        LabelProgram = "可执行文件:",
        LabelArgs = "参数:",
        DeleteConfirmTitle = "确认删除",
        DeleteConfirmBody = "确定删除此设备映射吗？",
        ErrorTitle = "错误",
        SaveError = "保存时出错:\n{0}",
    };
}

// ── Logger ────────────────────────────────────────────────────────────────────

sealed class Logger : IDisposable
{
    readonly object        _lock = new();
    readonly bool          _toConsole;
    readonly StreamWriter? _file;

    public Logger(bool toConsole, string? logFile = null)
    {
        _toConsole = toConsole;
        if (logFile is not null)
            _file = new StreamWriter(logFile, append: true, System.Text.Encoding.UTF8) { AutoFlush = true };
    }

    public void Write(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        lock (_lock)
        {
            _file?.WriteLine(message);
            if (!_toConsole) return;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public void Dispose() => _file?.Dispose();
}

// ── Modelle ────────────────────────────────────────────────────────────────────

class DeviceMapping
{
    public string DeviceId   { get; set; } = string.Empty;
    public string Executable { get; set; } = string.Empty;
    public string? Arguments { get; set; }
}

class AppConfig
{
    public List<DeviceMapping> Devices { get; set; } = [];
}

class ConfigHolder
{
    public volatile AppConfig Current;
    public ConfigHolder(AppConfig config) => Current = config;
}

// ── Konfigurations-Editor ──────────────────────────────────────────────────────

sealed class ConfigEditorForm : Form
{
    readonly string _configPath;
    readonly Action _onSaved;

    readonly CheckBox _rawCheck;
    readonly TextBox  _rawTextBox;
    readonly Panel    _rawPanel;
    readonly Panel    _visualPanel;
    readonly Panel    _cardContainer;
    readonly Panel    _scrollContainer;

    static readonly Color Bg         = Theme.Dark ? Color.FromArgb(32, 32, 32)    : Color.FromArgb(243, 243, 243);
    static readonly Color CardBg     = Theme.Dark ? Color.FromArgb(45, 45, 45)    : Color.White;
    static readonly Color AccentBlue = Color.FromArgb(0, 103, 192);
    static readonly Color Border     = Theme.Dark ? Color.FromArgb(62, 62, 62)    : Color.FromArgb(218, 218, 218);
    static readonly Color TextMuted  = Theme.Dark ? Color.FromArgb(155, 155, 155) : Color.FromArgb(86, 86, 86);
    static readonly Color TextNorm   = Theme.Dark ? Color.FromArgb(240, 240, 240) : Color.FromArgb(30, 30, 30);

    public ConfigEditorForm(string configPath, Action onSaved)
    {
        _configPath   = configPath;
        _onSaved      = onSaved;
        Text          = Loc.T.MenuEditConfig.TrimEnd('.','…');
        Size          = new Size(720, 580);
        MinimumSize   = new Size(520, 420);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor     = Bg;
        ForeColor     = TextNorm;
        Font          = new Font("Segoe UI", 9.5f);

        // ── Top bar ───────────────────────────────────────────────────────────
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CardBg };
        topBar.Paint += (_, e) =>
            e.Graphics.DrawLine(new Pen(Border), 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);

        _rawCheck = new CheckBox
        {
            Text      = Loc.T.RawLabel,
            AutoSize  = true,
            Location  = new Point(14, 12),
            Font      = new Font("Segoe UI", 9.5f),
            ForeColor = TextNorm,
            Cursor    = Cursors.Hand,
        };
        topBar.Controls.Add(_rawCheck);

        // ── Raw panel ─────────────────────────────────────────────────────────
        _rawPanel   = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(12, 8, 12, 8), BackColor = Bg };
        _rawTextBox = new TextBox
        {
            Multiline   = true,
            ScrollBars  = ScrollBars.Both,
            WordWrap    = false,
            Font        = new Font("Consolas", 10f),
            Dock        = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor   = CardBg,
            ForeColor   = TextNorm,
        };
        _rawPanel.Controls.Add(_rawTextBox);

        // ── Visual panel ──────────────────────────────────────────────────────
        _visualPanel     = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 12, 4), BackColor = Bg };
        _scrollContainer = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Bg };
        _cardContainer   = new Panel { BackColor = Bg, Location = Point.Empty };
        _scrollContainer.Controls.Add(_cardContainer);
        _scrollContainer.Resize += (_, _) => RelayoutCards();

        var addRow = new Panel { Dock = DockStyle.Bottom, Height = 44, BackColor = Bg };
        var addBtn = new Button
        {
            Text      = Loc.T.AddDevice,
            AutoSize  = true,
            Location  = new Point(2, 8),
            FlatStyle = FlatStyle.Flat,
            ForeColor = AccentBlue,
            BackColor = CardBg,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        addBtn.FlatAppearance.BorderColor = AccentBlue;
        addBtn.FlatAppearance.BorderSize  = 1;
        addBtn.Click += (_, _) => { AddCard(); RelayoutCards(); };
        addRow.Controls.Add(addBtn);

        _visualPanel.Controls.Add(_scrollContainer);
        _visualPanel.Controls.Add(addRow);

        // ── Content container ─────────────────────────────────────────────────
        var content = new Panel { Dock = DockStyle.Fill, BackColor = Bg };
        content.Controls.Add(_rawPanel);
        content.Controls.Add(_visualPanel);

        // ── Button bar ────────────────────────────────────────────────────────
        var btnBar = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = CardBg };
        btnBar.Paint += (_, e) =>
            e.Graphics.DrawLine(new Pen(Border), 0, 0, btnBar.Width, 0);

        var btnSave   = MakePrimaryBtn(Loc.T.BtnSave);
        var btnCancel = MakeSecondaryBtn(Loc.T.BtnCancel);
        btnSave.Anchor = btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSave.Location   = new Point(btnBar.Width - 106, 11);
        btnCancel.Location = new Point(btnBar.Width - 208, 11);
        btnCancel.Click += (_, _) => Close();
        btnSave.Click   += (_, _) => Save();
        btnBar.Controls.AddRange(new Control[] { btnSave, btnCancel });

        // ── Assemble ──────────────────────────────────────────────────────────
        Controls.Add(content);
        Controls.Add(btnBar);
        Controls.Add(topBar);

        _rawCheck.CheckedChanged += (_, _) => SwitchMode();
        Load += (_, _) =>
        {
            RelayoutCards();
            if (Theme.Dark) NativeMethods.SetDarkTitleBar(Handle);
        };

        var yaml = File.Exists(configPath)
            ? File.ReadAllText(configPath, System.Text.Encoding.UTF8)
            : string.Empty;
        _rawTextBox.Text = yaml;
        TryPopulateVisual(yaml);
    }

    // ── Mode switching ────────────────────────────────────────────────────────

    void SwitchMode()
    {
        if (_rawCheck.Checked)
        {
            _rawTextBox.Text     = BuildYaml();
            _rawPanel.Visible    = true;
            _visualPanel.Visible = false;
        }
        else
        {
            _cardContainer.Controls.Clear();
            TryPopulateVisual(_rawTextBox.Text);
            RelayoutCards();
            _rawPanel.Visible    = false;
            _visualPanel.Visible = true;
        }
    }

    void TryPopulateVisual(string yaml)
    {
        try
        {
            var cfg = string.IsNullOrWhiteSpace(yaml)
                ? new AppConfig()
                : new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build()
                    .Deserialize<AppConfig>(yaml) ?? new AppConfig();
            foreach (var d in cfg.Devices)
                AddCard(d.DeviceId, d.Executable, d.Arguments ?? string.Empty);
        }
        catch { }
    }

    // ── Card management ───────────────────────────────────────────────────────

    void AddCard(string id = "", string exe = "", string args = "")
        => _cardContainer.Controls.Add(BuildCard(id, exe, args));

    void RelayoutCards()
    {
        int w = Math.Max(_scrollContainer.ClientSize.Width - 2, 400);
        int y = 0;
        foreach (Control c in _cardContainer.Controls)
        {
            c.Location = new Point(0, y);
            c.Width    = w;
            y         += c.Height + 8;
        }
        _cardContainer.Size = new Size(w, Math.Max(y, 1));
    }

    Control BuildCard(string id, string exe, string args)
    {
        const int LW = 92, P = 14, Gap = 10;

        var txtId   = MakeInput(id);
        var txtExe  = MakeInput(exe);
        var txtArgs = MakeInput(args);

        // Measure real TextBox height after creation
        int rh    = txtId.PreferredHeight;
        int cardH = P + rh * 3 + Gap * 2 + P + 2;

        var card = new Panel { Height = cardH, BackColor = CardBg };
        card.Paint += (_, e) =>
        {
            using var pen = new Pen(Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var removeBtn = new Button
        {
            Text      = "✕",
            Size      = new Size(24, 24),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.Dark ? Color.FromArgb(160, 160, 160) : Color.FromArgb(110, 110, 110),
            BackColor = Color.Transparent,
            Font      = new Font("Segoe UI", 8.5f),
            Anchor    = AnchorStyles.Top | AnchorStyles.Right,
            Cursor    = Cursors.Hand,
        };
        removeBtn.FlatAppearance.BorderSize = 0;
        removeBtn.Click += (_, _) =>
        {
        var result = MessageBox.Show(
            Loc.T.DeleteConfirmBody,
            Loc.T.DeleteConfirmTitle,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes) return;

            _cardContainer.Controls.Remove(card);
            card.Dispose();
            RelayoutCards();
        };

        var browseBtn = new Button
        {
            Width     = 30,
            Height    = rh,
            Text      = "…",
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Dark ? Color.FromArgb(62, 62, 62) : Color.FromArgb(238, 238, 238),
            ForeColor = TextNorm,
            Font      = new Font("Segoe UI", 9f),
            Cursor    = Cursors.Hand,
        };
        browseBtn.FlatAppearance.BorderColor = Border;
        browseBtn.Click += (_, _) =>
        {
            using var dlg = new OpenFileDialog
            {
                Title  = Loc.T.BrowseTitle,
                Filter = Loc.T.BrowseFilter,
            };
            if (!string.IsNullOrEmpty(txtExe.Text)) dlg.FileName = txtExe.Text;
            var dir = Path.GetDirectoryName(txtExe.Text);
            if (Directory.Exists(dir)) dlg.InitialDirectory = dir;
            if (dlg.ShowDialog() == DialogResult.OK)
                txtExe.Text = dlg.FileName;
        };

        card.Controls.AddRange(new Control[]
            { removeBtn, MakeLabel(Loc.T.LabelDeviceId), txtId,
                         MakeLabel(Loc.T.LabelProgram),  txtExe, browseBtn,
                         MakeLabel(Loc.T.LabelArgs),     txtArgs });

        card.Tag = new CardInputs(txtId, txtExe, txtArgs);

        void Layout()
        {
            int iw = card.Width - LW - P * 2;
            int x  = P + LW;
            removeBtn.Location = new Point(card.Width - 32, 6);

            int y = P;
            card.Controls[1].Location = new Point(P, y + 3);   // lblId
            txtId.Location = new Point(x, y); txtId.Width = iw;

            y += rh + Gap;
            card.Controls[3].Location = new Point(P, y + 3);   // lblExe
            txtExe.Location   = new Point(x, y); txtExe.Width = iw - browseBtn.Width - 4;
            browseBtn.Location = new Point(x + txtExe.Width + 4, y);

            y += rh + Gap;
            card.Controls[6].Location = new Point(P, y + 3);   // lblArgs
            txtArgs.Location = new Point(x, y); txtArgs.Width = iw;
        }

        Layout();
        card.Resize += (_, _) => Layout();
        return card;
    }

    // ── Save / YAML ───────────────────────────────────────────────────────────

    void Save()
    {
        try
        {
            var yaml = _rawCheck.Checked ? _rawTextBox.Text : BuildYaml();
            File.WriteAllText(_configPath, yaml, System.Text.Encoding.UTF8);
            _onSaved();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format(Loc.T.SaveError, ex.Message), Loc.T.ErrorTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    string BuildYaml()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("devices:");
        foreach (Control c in _cardContainer.Controls)
        {
            if (c.Tag is CardInputs d)
            {
                sb.AppendLine($"  - deviceId: {Ys(d.Id.Text.Trim())}");
                sb.AppendLine($"    executable: {Ys(d.Exe.Text.Trim())}");
                if (!string.IsNullOrWhiteSpace(d.Args.Text))
                    sb.AppendLine($"    arguments: {Ys(d.Args.Text.Trim())}");
            }
        }
        return sb.ToString();
    }

    static string Ys(string v) => string.IsNullOrEmpty(v) ? "''"
        : v.Contains('\'')
            ? $"\"{v.Replace("\\", "\\\\").Replace("\"", "\\\"")}\""
            : $"'{v}'";

    // ── Control factories ─────────────────────────────────────────────────────

    static Label MakeLabel(string text) => new()
    {
        Text      = text,
        AutoSize  = true,
        ForeColor = TextMuted,
        Font      = new Font("Segoe UI", 9f),
    };

    static TextBox MakeInput(string text) => new()
    {
        Text        = text,
        BorderStyle = BorderStyle.FixedSingle,
        Font        = new Font("Segoe UI", 9.5f),
        BackColor   = CardBg,
        ForeColor   = TextNorm,
    };

    static Button MakePrimaryBtn(string text)
    {
        var b = new Button
        {
            Text      = text,
            Width     = 94,
            Height    = 32,
            FlatStyle = FlatStyle.Flat,
            BackColor = AccentBlue,
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        b.FlatAppearance.BorderSize          = 0;
        b.FlatAppearance.MouseOverBackColor  = Color.FromArgb(0, 84, 166);
        b.FlatAppearance.MouseDownBackColor  = Color.FromArgb(0, 66, 140);
        return b;
    }

    static Button MakeSecondaryBtn(string text)
    {
        var b = new Button
        {
            Text      = text,
            Width     = 94,
            Height    = 32,
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Dark ? Color.FromArgb(62, 62, 62) : Color.FromArgb(238, 238, 238),
            ForeColor = TextNorm,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        b.FlatAppearance.BorderColor        = Border;
        b.FlatAppearance.MouseOverBackColor = Theme.Dark ? Color.FromArgb(76, 76, 76) : Color.FromArgb(225, 225, 225);
        b.FlatAppearance.MouseDownBackColor = Theme.Dark ? Color.FromArgb(50, 50, 50) : Color.FromArgb(210, 210, 210);
        return b;
    }
}

record CardInputs(TextBox Id, TextBox Exe, TextBox Args);

// ── Theme ────────────────────────────────────────────────────────────────────

static class Theme
{
    public static readonly bool Dark = Detect();

    static bool Detect()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int v && v == 0;
        }
        catch { return false; }
    }
}

// ── Dark Menu Renderer ────────────────────────────────────────────────────────

sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
{
    static readonly Color MenuBg   = Color.FromArgb(31, 31, 31);
    static readonly Color TextCol  = Color.FromArgb(235, 235, 235);
    static readonly Color TextGray = Color.FromArgb(130, 130, 130);

    public DarkMenuRenderer() : base(new DarkColorTable()) { RoundedEdges = false; }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.TextColor = e.Item.Enabled ? TextCol : TextGray;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        => e.Graphics.Clear(MenuBg);

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(60, 60, 60));
        int y = e.Item.Height / 2;
        e.Graphics.DrawLine(pen, 28, y, e.Item.Width - 4, y);
    }
}

sealed class DarkColorTable : ProfessionalColorTable
{
    static readonly Color Bg  = Color.FromArgb(31, 31, 31);
    static readonly Color Hov = Color.FromArgb(54, 54, 54);
    static readonly Color Bdr = Color.FromArgb(50, 50, 50);

    public override Color MenuItemSelected              => Hov;
    public override Color MenuItemBorder                => Hov;
    public override Color MenuItemSelectedGradientBegin => Hov;
    public override Color MenuItemSelectedGradientEnd   => Hov;
    public override Color MenuItemPressedGradientBegin  => Hov;
    public override Color MenuItemPressedGradientEnd    => Hov;
    public override Color ToolStripDropDownBackground   => Bg;
    public override Color ImageMarginGradientBegin      => Bg;
    public override Color ImageMarginGradientMiddle     => Bg;
    public override Color ImageMarginGradientEnd        => Bg;
    public override Color MenuBorder                    => Bdr;
    public override Color SeparatorDark                 => Bdr;
    public override Color SeparatorLight                => Bdr;
    public override Color CheckBackground               => Hov;
    public override Color CheckSelectedBackground       => Hov;
    public override Color CheckPressedBackground        => Hov;
}

// ── P/Invoke ───────────────────────────────────────────────────────────────────

static class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public static void SetDarkTitleBar(IntPtr hwnd)
    {
        int v = 1;
        DwmSetWindowAttribute(hwnd, 20, ref v, 4);
    }
}
