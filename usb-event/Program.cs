using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    const string AppDisplayName = "USB Event";
    const string RegistryRunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    static readonly string AppVersion =
        typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion.Split('+')[0]   // strip git-hash suffix if present
            ?? "0.0.0";

    [STAThread]
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Loc.Init();

        if (args.Contains("--install"))        { InstallAutostart();       return; }
        if (args.Contains("--install-silent")) { InstallAutostartSilent(); return; }
        if (args.Contains("--uninstall"))      { UninstallAutostart();     return; }
        if (args.Contains("--stop"))           { StopRunningInstance();    return; }

        var trayMode   = args.Contains("--tray");
        var (configDir, _) = GetConfigDir();
        var configPath  = Path.Combine(configDir, "config.yaml");
        var isNewConfig = EnsureConfigExists(configPath);
        var config      = LoadConfig(configPath);
        var running     = new ConcurrentDictionary<string, Process>(StringComparer.OrdinalIgnoreCase);

        if (trayMode)
            RunAsTray(config, running, configPath, openEditor: isNewConfig);
        else
            RunAsConsole(config, configPath, running);
    }

    // ── Tray-Modus ─────────────────────────────────────────────────────────────

    const string ShutdownEventName = "Local\\usb-event-shutdown";

    static void RunAsTray(AppConfig config, ConcurrentDictionary<string, Process> running, string configPath, bool openEditor = false)
    {
        var dir     = Path.GetDirectoryName(configPath)!;
        var logPath = Path.Combine(dir, "usb-event.log");
        using var logger = new Logger(toConsole: false, logFile: logPath);
        DeviceHistory.Init(dir);

        Console.CancelKeyPress += (_, e) => { e.Cancel = true; Application.Exit(); };
        NativeMethods.FreeConsole();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var holder = new ConfigHolder(config);

        using var trayIconImage = CreateTrayIcon();
        using var trayIcon = new NotifyIcon
        {
            Icon    = trayIconImage,
            Text    = $"{Loc.T.AppName} v{AppVersion}",
            Visible = true
        };

        var menu   = new ContextMenuStrip();
        var header = new ToolStripMenuItem($"{Loc.T.AppName} v{AppVersion}") { Enabled = false };
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

        bool registered = IsAutostartRegistered();
        var installItem   = new ToolStripMenuItem(Loc.T.MenuInstall)   { Enabled = !registered };
        var uninstallItem = new ToolStripMenuItem(Loc.T.MenuUninstall) { Enabled = registered  };
        installItem.Click += (_, _) =>
        {
            if (RegisterAutostart() is not null)
            {
                installItem.Enabled   = false;
                uninstallItem.Enabled = true;
            }
        };
        uninstallItem.Click += (_, _) =>
        {
            var result = MessageBox.Show(
                Loc.T.UninstallConfirmBody,
                Loc.T.UninstallConfirmTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UninstallAutostart();
                trayIcon.Visible = false;
                Application.Exit();
            }
        };
        menu.Items.Add(installItem);
        menu.Items.Add(uninstallItem);

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

        CheckAlreadyConnectedDevices(holder, running, logger, trayIcon);

        if (openEditor)
        {
            using var editor = new ConfigEditorForm(configPath,
                () => holder.Current = LoadConfig(configPath));
            editor.ShowDialog();
        }

        using var shutdownEvent = new EventWaitHandle(false, EventResetMode.ManualReset, ShutdownEventName);
        new Thread(() =>
        {
            try   { shutdownEvent.WaitOne(); Application.Exit(); }
            catch (ObjectDisposedException) { }
        }) { IsBackground = true }.Start();

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

        DeviceHistory.Init(Path.GetDirectoryName(configPath)!);

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        var holder = new ConfigHolder(config);
        using var logger = new Logger(toConsole: true);
        using var insertWatcher = CreateWatcher("__InstanceCreationEvent", holder, running, null, logger, isConnect: true);
        using var removeWatcher = CreateWatcher("__InstanceDeletionEvent", holder, running, null, logger, isConnect: false);
        insertWatcher.Start();
        removeWatcher.Start();

        CheckAlreadyConnectedDevices(holder, running, logger, null);

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

            if (isConnect)
            {
                DeviceHistory.Record(deviceId, device["Name"]?.ToString() ?? string.Empty);
                DeviceHistory.MarkPostStart(deviceId);
            }

            if (mapping is null) return;

            if (isConnect && mapping.StartOnPlugIn)
                StartProcess(mapping, running, logger, trayIcon);
            else if (!isConnect && mapping.KillOnPlugOut)
                StopProcess(mapping, running, logger, trayIcon);
        };

        return watcher;
    }

    // ── Startup check ──────────────────────────────────────────────────────────

    static void CheckAlreadyConnectedDevices(
        ConfigHolder holder,
        ConcurrentDictionary<string, Process> running,
        Logger? logger,
        NotifyIcon? trayIcon)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            foreach (ManagementObject device in searcher.Get())
            {
                if (!IsUsbDevice(device)) continue;

                var deviceId = device["DeviceID"]?.ToString() ?? string.Empty;
                var mapping  = FindMapping(holder.Current, deviceId);
                if (mapping is null || !mapping.StartOnStart) continue;

                DeviceHistory.Record(deviceId, device["Name"]?.ToString() ?? string.Empty);
                StartProcess(mapping, running, logger, trayIcon);
            }
        }
        catch (Exception ex)
        {
            logger?.Write(string.Format(Loc.T.ProcessStartError, ex.Message), ConsoleColor.Yellow);
        }
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

    static bool IsAutostartRegistered()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey);
        return key?.GetValue(AppDisplayName) is not null;
    }

    static void StopRunningInstance()
    {
        if (EventWaitHandle.TryOpenExisting(ShutdownEventName, out var ev))
        {
            ev.Set();
            ev.Dispose();
        }
    }

    static void InstallAutostart()
    {
        var exePath = RegisterAutostart();
        if (exePath is null) return;

        Console.WriteLine(string.Format(Loc.T.AutostartInstalled, exePath));
        Console.WriteLine(Loc.T.AutostartInfo);

        Console.Write(Loc.T.AutostartStartNow);
        var answer = Console.ReadLine() ?? string.Empty;
        if (answer.Length == 0 || (!answer.StartsWith("n", StringComparison.OrdinalIgnoreCase)
                                && !answer.StartsWith("н", StringComparison.OrdinalIgnoreCase)))
        {
            Process.Start(new ProcessStartInfo(exePath, "--tray") { UseShellExecute = true });
        }
    }

    static void InstallAutostartSilent()
    {
        var exePath = RegisterAutostart();
        if (exePath is null) return;

        Process.Start(new ProcessStartInfo(exePath, "--tray") { UseShellExecute = true });
    }

    // Registers the HKCU Run key entry and returns the exe path, or null if running under dotnet.exe.
    static string? RegisterAutostart()
    {
        var exePath = Environment.ProcessPath
            ?? throw new InvalidOperationException(Loc.T.CannotResolveProcessPath);

        if (Path.GetFileName(exePath).Equals("dotnet.exe", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(Loc.T.AutostartDotnetWarning);
            Console.WriteLine(Loc.T.AutostartPublishHint);
            return null;
        }

        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true)
            ?? throw new InvalidOperationException(Loc.T.RegistryNotAccessible);

        key.SetValue(AppDisplayName, $"\"{exePath}\" --tray");
        return exePath;
    }

    static void UninstallAutostart()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true);
        key?.DeleteValue(AppDisplayName, throwOnMissingValue: false);
        Console.WriteLine(Loc.T.AutostartRemoved);
    }

    // ── Hilfsmethoden ──────────────────────────────────────────────────────────

    // Portable: config.yaml next to the .exe AND the directory is writable (e.g. USB-stick).
    // Installed: falls back to %APPDATA%\usb-event\ (Windows convention).
    //            Program Files is never writable without elevation, so it always falls back.
    static (string dir, bool portable) GetConfigDir()
    {
        var baseDir        = AppContext.BaseDirectory;
        var portableConfig = Path.Combine(baseDir, "config.yaml");
        if (File.Exists(portableConfig) && IsDirectoryWritable(baseDir))
            return (baseDir, true);

        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "usb-event");
        Directory.CreateDirectory(dir);
        return (dir, false);
    }

    static bool IsDirectoryWritable(string dir)
    {
        try
        {
            var probe = Path.Combine(dir, ".write-probe");
            File.WriteAllText(probe, "");
            File.Delete(probe);
            return true;
        }
        catch { return false; }
    }

    static bool EnsureConfigExists(string path)
    {
        if (File.Exists(path)) return false;
        const string template =
            "# USB device mappings\n" +
            "# deviceId:   prefix of the Device ID shown in console output\n" +
            "#             VID/PID prefix is enough, e.g. \"USB\\VID_1234&PID_ABCD\"\n" +
            "# executable: full path to the .exe to launch\n" +
            "# arguments:  (optional) command-line arguments\n" +
            "\n" +
            "devices: []\n";
        File.WriteAllText(path, template, System.Text.Encoding.UTF8);
        return true;
    }

    static AppConfig LoadConfig(string path)
    {
        if (!File.Exists(path))
            return new AppConfig();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using var reader = new StreamReader(path);
        return deserializer.Deserialize<AppConfig>(reader) ?? new AppConfig();
    }

    static DeviceMapping? FindMapping(AppConfig config, string deviceId) =>
        config.Devices.FirstOrDefault(m =>
            deviceId.StartsWith(m.DeviceId, StringComparison.OrdinalIgnoreCase));

    internal static IReadOnlyList<RecentDevice> GetCurrentUsbDevices()
    {
        try
        {
            var list = new List<RecentDevice>();
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            foreach (ManagementObject device in searcher.Get())
            {
                if (!IsUsbDevice(device)) continue;
                var deviceId = device["DeviceID"]?.ToString() ?? string.Empty;
                var name     = device["Name"]?.ToString() ?? string.Empty;
                list.Add(new RecentDevice(deviceId, string.IsNullOrWhiteSpace(name) ? deviceId : name, DateTime.MinValue /* not a history entry */));
            }
            return list;
        }
        catch { return []; }
    }

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

    internal static Icon CreateTrayIcon()
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

        g.FillPolygon(blueBrush, new PointF[] { new(17, 3), new(10, 14), new(24, 14) });
        g.FillRectangle(blueBrush, 2, 12, 8, 8);
        g.FillEllipse(blueBrush, 22, 12, 9, 9);
        g.FillRectangle(blueBrush, 4, 20, 4, 8);
        g.FillRectangle(blueBrush, 24, 21, 4, 7);
        g.FillRectangle(blueBrush, 2, 28, 28, 3);
        g.FillRectangle(blueBrush, 14, 14, 6, 22);
        g.FillRectangle(blueBrush, 10, 35, 14, 10);
        using var slotBrush = new SolidBrush(Color.FromArgb(190, 225, 250));
        g.FillRectangle(slotBrush, 12, 37, 10, 6);

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
