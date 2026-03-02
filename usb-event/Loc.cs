using System.Globalization;

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
    public required string NoRecentDevices { get; init; }
    public required string QuickSelectHint { get; init; }
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
        AppName = "USB Event",
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
        NoRecentDevices = "No recent devices",
        QuickSelectHint = "Plug and unplug a device to populate the quick-select list (▾)",
    };

    static LocStrings De => new()
    {
        AppName = "USB Event",
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
        NoRecentDevices = "Kein Geräteverlauf",
        QuickSelectHint = "Gerät ein- und ausstecken, um die Schnellauswahl (▾) zu befüllen",
    };

    static LocStrings Fr => new()
    {
        AppName = "USB Event",
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
        NoRecentDevices = "Aucun appareil récent",
        QuickSelectHint = "Branchez et débranchez un appareil pour alimenter la liste rapide (▾)",
    };

    static LocStrings Es => new()
    {
        AppName = "USB Event",
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
        NoRecentDevices = "Sin dispositivos recientes",
        QuickSelectHint = "Conecta y desconecta un dispositivo para llenar la selección rápida (▾)",
    };

    static LocStrings Ru => new()
    {
        AppName = "USB Event",
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
        NoRecentDevices = "Нет недавних устройств",
        QuickSelectHint = "Подключите и отключите устройство, чтобы заполнить быстрый список (▾)",
    };

    static LocStrings Ja => new()
    {
        AppName = "USB Event",
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
        NoRecentDevices = "履歴なし",
        QuickSelectHint = "デバイスを接続・切断してクイック選択 (▾) を使えるようにする",
    };

    static LocStrings Zh => new()
    {
        AppName = "USB Event",
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
        NoRecentDevices = "无最近设备",
        QuickSelectHint = "插拔设备以填充快速选择列表 (▾)",
    };
}
