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
    public required string AutostartStartNow { get; init; }

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
            "ar" => Ar,
            "id" => Id,
            "ko" => Ko,
            "pt" => Pt,
            "hi" => Hi,
            "sv" => Sv,
            "no" => No,
            "nb" => No,
            "nn" => No,
            "fi" => Fi,
            "et" => Et,
            "lv" => Lv,
            "lt" => Lt,
            "pl" => Pl,
            "cs" => Cs,
            "hr" => Hr,
            "sq" => Sq,
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
        AutostartStartNow = "Start in background now? [Y/n] ",

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
        AutostartStartNow = "Jetzt im Hintergrund starten? [J/n] ",

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
        AutostartStartNow = "Démarrer en arrière-plan maintenant ? [O/n] ",

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
        AutostartStartNow = "¿Iniciar en segundo plano ahora? [S/n] ",

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
        AutostartStartNow = "Запустить в фоне сейчас? [Д/н] ",

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
        AutostartStartNow = "今すぐバックグラウンドで起動しますか？ [Y/n] ",

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
        AutostartStartNow = "现在在后台启动？[Y/n] ",

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

    static LocStrings Ar => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "تعديل الإعدادات…",
        MenuOpenFolder = "فتح مجلد الإعدادات/السجل",
        MenuExit = "خروج",

        MissingMappings = "لم يتم العثور على تعيينات أجهزة في الإعدادات.",
        ConfigFile = "ملف الإعدادات",
        Started = "بدأ مراقب أجهزة USB",
        ExitHint = "اضغط Ctrl+C للخروج",
        ConfigCount = "إدخالات الإعداد",
        Stopped = "توقف التطبيق.",

        UsbDevice = "جهاز USB",
        Connected = "متصل",
        Disconnected = "مفصول",

        ProcessAlreadyRunning = "  العملية تعمل بالفعل (PID {0}): {1}",
        ProcessStarted = "  ▶ بدأت العملية: {0} (PID {1})",
        ProcessStartedShort = "بدأ: {0}",
        ProcessStartError = "  ✖ خطأ في بدء العملية: {0}",
        ProcessStopped = "  ◀ توقفت العملية: {0} (PID {1})",
        ProcessStoppedShort = "توقف: {0}",
        ProcessStopError = "  ✖ خطأ في إيقاف العملية: {0}",

        CannotResolveProcessPath = "تعذر تحديد مسار العملية.",
        AutostartDotnetWarning = "⚠ يمكن تثبيت التشغيل التلقائي فقط مع الـ .exe المنشور.",
        AutostartPublishHint = "  يرجى تشغيل 'dotnet publish' أولاً.",
        RegistryNotAccessible = "مفتاح السجل غير متاح.",
        AutostartInstalled = "✔ تم تسجيل التشغيل التلقائي: {0} --tray",
        AutostartInfo = "  سيبدأ التطبيق في شريط المهام عند تسجيل الدخول التالي.",
        AutostartRemoved = "✔ تمت إزالة إدخال التشغيل التلقائي.",
        AutostartStartNow = "البدء في الخلفية الآن؟ [Y/n] ",

        RawLabel = "YAML خام",
        BtnSave = "حفظ",
        BtnCancel = "إلغاء",
        AddDevice = "+ إضافة جهاز",
        BrowseTitle = "اختر الملف التنفيذي",
        BrowseFilter = "الملفات التنفيذية (*.exe)|*.exe|جميع الملفات (*.*)|*.*",
        LabelDeviceId = "معرّف الجهاز:",
        LabelProgram = "الملف التنفيذي:",
        LabelArgs = "المعطيات:",
        DeleteConfirmTitle = "تأكيد الحذف",
        DeleteConfirmBody = "حذف تعيين هذا الجهاز؟",
        ErrorTitle = "خطأ",
        SaveError = "خطأ أثناء الحفظ:\n{0}",
        NoRecentDevices = "لا توجد أجهزة حديثة",
        QuickSelectHint = "وصّل الجهاز وافصله لملء قائمة الاختيار السريع (▾)",
    };

    static LocStrings Id => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Edit konfigurasi…",
        MenuOpenFolder = "Buka folder konfigurasi/log",
        MenuExit = "Keluar",

        MissingMappings = "Tidak ada pemetaan perangkat yang ditemukan dalam konfigurasi.",
        ConfigFile = "File konfigurasi",
        Started = "Monitor Perangkat USB dimulai",
        ExitHint = "Tekan Ctrl+C untuk keluar",
        ConfigCount = "Entri konfigurasi",
        Stopped = "Aplikasi dihentikan.",

        UsbDevice = "Perangkat USB",
        Connected = "terhubung",
        Disconnected = "terputus",

        ProcessAlreadyRunning = "  Proses sudah berjalan (PID {0}): {1}",
        ProcessStarted = "  ▶ Proses dimulai: {0} (PID {1})",
        ProcessStartedShort = "Dimulai: {0}",
        ProcessStartError = "  ✖ Kesalahan memulai proses: {0}",
        ProcessStopped = "  ◀ Proses dihentikan: {0} (PID {1})",
        ProcessStoppedShort = "Dihentikan: {0}",
        ProcessStopError = "  ✖ Kesalahan menghentikan proses: {0}",

        CannotResolveProcessPath = "Tidak dapat menentukan jalur proses.",
        AutostartDotnetWarning = "⚠ Mulai otomatis hanya dapat dipasang dengan .exe yang dipublikasikan.",
        AutostartPublishHint = "  Jalankan 'dotnet publish' terlebih dahulu.",
        RegistryNotAccessible = "Kunci registri tidak dapat diakses.",
        AutostartInstalled = "✔ Mulai otomatis terdaftar: {0} --tray",
        AutostartInfo = "  Aplikasi akan mulai di tray saat login berikutnya.",
        AutostartRemoved = "✔ Entri mulai otomatis dihapus.",
        AutostartStartNow = "Mulai di latar belakang sekarang? [Y/n] ",

        RawLabel = "YAML mentah",
        BtnSave = "Simpan",
        BtnCancel = "Batal",
        AddDevice = "+ Tambah perangkat",
        BrowseTitle = "Pilih file yang dapat dieksekusi",
        BrowseFilter = "File yang dapat dieksekusi (*.exe)|*.exe|Semua file (*.*)|*.*",
        LabelDeviceId = "ID Perangkat:",
        LabelProgram = "File yang dapat dieksekusi:",
        LabelArgs = "Argumen:",
        DeleteConfirmTitle = "Konfirmasi hapus",
        DeleteConfirmBody = "Hapus pemetaan perangkat ini?",
        ErrorTitle = "Kesalahan",
        SaveError = "Kesalahan saat menyimpan:\n{0}",
        NoRecentDevices = "Tidak ada perangkat terbaru",
        QuickSelectHint = "Hubungkan dan cabut perangkat untuk mengisi daftar pilihan cepat (▾)",
    };

    static LocStrings Ko => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "설정 편집…",
        MenuOpenFolder = "설정/로그 폴더 열기",
        MenuExit = "종료",

        MissingMappings = "설정에서 장치 매핑을 찾을 수 없습니다.",
        ConfigFile = "설정 파일",
        Started = "USB 장치 모니터 시작됨",
        ExitHint = "종료하려면 Ctrl+C를 누르세요",
        ConfigCount = "설정 항목",
        Stopped = "애플리케이션이 중지되었습니다.",

        UsbDevice = "USB 장치",
        Connected = "연결됨",
        Disconnected = "연결 해제됨",

        ProcessAlreadyRunning = "  프로세스가 이미 실행 중입니다 (PID {0}): {1}",
        ProcessStarted = "  ▶ 프로세스 시작됨: {0} (PID {1})",
        ProcessStartedShort = "시작됨: {0}",
        ProcessStartError = "  ✖ 프로세스 시작 오류: {0}",
        ProcessStopped = "  ◀ 프로세스 중지됨: {0} (PID {1})",
        ProcessStoppedShort = "중지됨: {0}",
        ProcessStopError = "  ✖ 프로세스 중지 오류: {0}",

        CannotResolveProcessPath = "프로세스 경로를 확인할 수 없습니다.",
        AutostartDotnetWarning = "⚠ 자동 시작은 게시된 .exe 파일에서만 설치할 수 있습니다.",
        AutostartPublishHint = "  먼저 'dotnet publish'를 실행하세요.",
        RegistryNotAccessible = "레지스트리 키에 접근할 수 없습니다.",
        AutostartInstalled = "✔ 자동 시작 등록됨: {0} --tray",
        AutostartInfo = "  다음 로그인 시 앱이 트레이에서 자동으로 시작됩니다.",
        AutostartRemoved = "✔ 자동 시작 항목이 제거되었습니다.",
        AutostartStartNow = "지금 백그라운드에서 시작하시겠습니까? [Y/n] ",

        RawLabel = "Raw YAML",
        BtnSave = "저장",
        BtnCancel = "취소",
        AddDevice = "+ 장치 추가",
        BrowseTitle = "실행 파일 선택",
        BrowseFilter = "실행 파일 (*.exe)|*.exe|모든 파일 (*.*)|*.*",
        LabelDeviceId = "장치 ID:",
        LabelProgram = "실행 파일:",
        LabelArgs = "인수:",
        DeleteConfirmTitle = "삭제 확인",
        DeleteConfirmBody = "이 장치 매핑을 삭제하시겠습니까?",
        ErrorTitle = "오류",
        SaveError = "저장 중 오류 발생:\n{0}",
        NoRecentDevices = "최근 장치 없음",
        QuickSelectHint = "장치를 연결하고 분리하여 빠른 선택 목록 (▾)을 채우세요",
    };

    static LocStrings Pt => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Editar configuração…",
        MenuOpenFolder = "Abrir pasta de config/log",
        MenuExit = "Sair",

        MissingMappings = "Nenhum mapeamento de dispositivo encontrado na configuração.",
        ConfigFile = "Arquivo de configuração",
        Started = "Monitor de dispositivo USB iniciado",
        ExitHint = "Pressione Ctrl+C para sair",
        ConfigCount = "Entradas de configuração",
        Stopped = "Aplicação encerrada.",

        UsbDevice = "Dispositivo USB",
        Connected = "conectado",
        Disconnected = "desconectado",

        ProcessAlreadyRunning = "  Processo já em execução (PID {0}): {1}",
        ProcessStarted = "  ▶ Processo iniciado: {0} (PID {1})",
        ProcessStartedShort = "Iniciado: {0}",
        ProcessStartError = "  ✖ Erro ao iniciar processo: {0}",
        ProcessStopped = "  ◀ Processo encerrado: {0} (PID {1})",
        ProcessStoppedShort = "Encerrado: {0}",
        ProcessStopError = "  ✖ Erro ao encerrar processo: {0}",

        CannotResolveProcessPath = "Não foi possível determinar o caminho do processo.",
        AutostartDotnetWarning = "⚠ A inicialização automática só pode ser instalada com o .exe publicado.",
        AutostartPublishHint = "  Execute 'dotnet publish' primeiro.",
        RegistryNotAccessible = "Chave de registro não acessível.",
        AutostartInstalled = "✔ Inicialização automática registrada: {0} --tray",
        AutostartInfo = "  O aplicativo iniciará na bandeja no próximo login.",
        AutostartRemoved = "✔ Entrada de inicialização automática removida.",
        AutostartStartNow = "Iniciar em segundo plano agora? [Y/n] ",

        RawLabel = "YAML bruto",
        BtnSave = "Salvar",
        BtnCancel = "Cancelar",
        AddDevice = "+ Adicionar dispositivo",
        BrowseTitle = "Selecionar executável",
        BrowseFilter = "Arquivos executáveis (*.exe)|*.exe|Todos os arquivos (*.*)|*.*",
        LabelDeviceId = "ID do dispositivo:",
        LabelProgram = "Executável:",
        LabelArgs = "Argumentos:",
        DeleteConfirmTitle = "Confirmar exclusão",
        DeleteConfirmBody = "Excluir este mapeamento de dispositivo?",
        ErrorTitle = "Erro",
        SaveError = "Erro ao salvar:\n{0}",
        NoRecentDevices = "Sem dispositivos recentes",
        QuickSelectHint = "Conecte e desconecte um dispositivo para preencher a lista de seleção rápida (▾)",
    };

    static LocStrings Hi => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "कॉन्फ़िगरेशन संपादित करें…",
        MenuOpenFolder = "कॉन्फ़िग/लॉग फ़ोल्डर खोलें",
        MenuExit = "बाहर निकलें",

        MissingMappings = "कॉन्फ़िगरेशन में कोई डिवाइस मैपिंग नहीं मिली।",
        ConfigFile = "कॉन्फ़िग फ़ाइल",
        Started = "USB डिवाइस मॉनिटर शुरू हुआ",
        ExitHint = "बाहर निकलने के लिए Ctrl+C दबाएँ",
        ConfigCount = "कॉन्फ़िगरेशन प्रविष्टियाँ",
        Stopped = "एप्लिकेशन बंद हो गया।",

        UsbDevice = "USB डिवाइस",
        Connected = "कनेक्टेड",
        Disconnected = "डिसकनेक्टेड",

        ProcessAlreadyRunning = "  प्रक्रिया पहले से चल रही है (PID {0}): {1}",
        ProcessStarted = "  ▶ प्रक्रिया शुरू हुई: {0} (PID {1})",
        ProcessStartedShort = "शुरू हुई: {0}",
        ProcessStartError = "  ✖ प्रक्रिया शुरू करने में त्रुटि: {0}",
        ProcessStopped = "  ◀ प्रक्रिया रोकी गई: {0} (PID {1})",
        ProcessStoppedShort = "रोकी गई: {0}",
        ProcessStopError = "  ✖ प्रक्रिया रोकने में त्रुटि: {0}",

        CannotResolveProcessPath = "प्रक्रिया का पथ निर्धारित नहीं किया जा सका।",
        AutostartDotnetWarning = "⚠ ऑटोस्टार्ट केवल प्रकाशित .exe के साथ इंस्टॉल किया जा सकता है।",
        AutostartPublishHint = "  पहले 'dotnet publish' चलाएँ।",
        RegistryNotAccessible = "रजिस्ट्री कुंजी सुलभ नहीं है।",
        AutostartInstalled = "✔ ऑटोस्टार्ट पंजीकृत: {0} --tray",
        AutostartInfo = "  अगले लॉगिन पर ऐप ट्रे में शुरू होगा।",
        AutostartRemoved = "✔ ऑटोस्टार्ट प्रविष्टि हटाई गई।",
        AutostartStartNow = "अभी पृष्ठभूमि में प्रारंभ करें? [Y/n] ",

        RawLabel = "Raw YAML",
        BtnSave = "सहेजें",
        BtnCancel = "रद्द करें",
        AddDevice = "+ डिवाइस जोड़ें",
        BrowseTitle = "एक्जीक्यूटेबल चुनें",
        BrowseFilter = "एक्जीक्यूटेबल फ़ाइलें (*.exe)|*.exe|सभी फ़ाइलें (*.*)|*.*",
        LabelDeviceId = "डिवाइस ID:",
        LabelProgram = "एक्जीक्यूटेबल:",
        LabelArgs = "तर्क:",
        DeleteConfirmTitle = "हटाने की पुष्टि",
        DeleteConfirmBody = "इस डिवाइस मैपिंग को हटाएँ?",
        ErrorTitle = "त्रुटि",
        SaveError = "सहेजते समय त्रुटि:\n{0}",
        NoRecentDevices = "कोई हाल के डिवाइस नहीं",
        QuickSelectHint = "त्वरित चयन सूची (▾) भरने के लिए डिवाइस कनेक्ट और डिसकनेक्ट करें",
    };

    static LocStrings Sv => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Redigera konfiguration…",
        MenuOpenFolder = "Öppna konfig-/loggmapp",
        MenuExit = "Avsluta",

        MissingMappings = "Inga enhetsmappningar hittades i konfigurationen.",
        ConfigFile = "Konfigurationsfil",
        Started = "USB-enhetsövervakare startad",
        ExitHint = "Tryck Ctrl+C för att avsluta",
        ConfigCount = "Konfigurationsposter",
        Stopped = "Programmet stoppat.",

        UsbDevice = "USB-enhet",
        Connected = "ansluten",
        Disconnected = "frånkopplad",

        ProcessAlreadyRunning = "  Processen körs redan (PID {0}): {1}",
        ProcessStarted = "  ▶ Process startad: {0} (PID {1})",
        ProcessStartedShort = "Startad: {0}",
        ProcessStartError = "  ✖ Fel vid start av process: {0}",
        ProcessStopped = "  ◀ Process stoppad: {0} (PID {1})",
        ProcessStoppedShort = "Stoppad: {0}",
        ProcessStopError = "  ✖ Fel vid stopp av process: {0}",

        CannotResolveProcessPath = "Det gick inte att fastställa processens sökväg.",
        AutostartDotnetWarning = "⚠ Autostart kan bara installeras med den publicerade .exe-filen.",
        AutostartPublishHint = "  Kör 'dotnet publish' först.",
        RegistryNotAccessible = "Registernyckeln är inte åtkomlig.",
        AutostartInstalled = "✔ Autostart registrerad: {0} --tray",
        AutostartInfo = "  Appen startar i fältet vid nästa inloggning.",
        AutostartRemoved = "✔ Autostart-post borttagen.",
        AutostartStartNow = "Starta i bakgrunden nu? [J/n] ",

        RawLabel = "Rå YAML",
        BtnSave = "Spara",
        BtnCancel = "Avbryt",
        AddDevice = "+ Lägg till enhet",
        BrowseTitle = "Välj körbar fil",
        BrowseFilter = "Körbara filer (*.exe)|*.exe|Alla filer (*.*)|*.*",
        LabelDeviceId = "Enhets-ID:",
        LabelProgram = "Körbar fil:",
        LabelArgs = "Argument:",
        DeleteConfirmTitle = "Bekräfta borttagning",
        DeleteConfirmBody = "Ta bort den här enhetsmappningen?",
        ErrorTitle = "Fel",
        SaveError = "Fel vid sparning:\n{0}",
        NoRecentDevices = "Inga senaste enheter",
        QuickSelectHint = "Anslut och koppla ur en enhet för att fylla snabbvalslistan (▾)",
    };

    static LocStrings No => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Rediger konfigurasjon…",
        MenuOpenFolder = "Åpne konfig-/loggmappe",
        MenuExit = "Avslutt",

        MissingMappings = "Ingen enhetstilordninger funnet i konfigurasjonen.",
        ConfigFile = "Konfigurasjonsfil",
        Started = "USB-enhetsovervåker startet",
        ExitHint = "Trykk Ctrl+C for å avslutte",
        ConfigCount = "Konfigurasjonsoppføringer",
        Stopped = "Programmet stoppet.",

        UsbDevice = "USB-enhet",
        Connected = "tilkoblet",
        Disconnected = "frakoblet",

        ProcessAlreadyRunning = "  Prosessen kjører allerede (PID {0}): {1}",
        ProcessStarted = "  ▶ Prosess startet: {0} (PID {1})",
        ProcessStartedShort = "Startet: {0}",
        ProcessStartError = "  ✖ Feil ved start av prosess: {0}",
        ProcessStopped = "  ◀ Prosess stoppet: {0} (PID {1})",
        ProcessStoppedShort = "Stoppet: {0}",
        ProcessStopError = "  ✖ Feil ved stopp av prosess: {0}",

        CannotResolveProcessPath = "Kunne ikke fastslå prosessens bane.",
        AutostartDotnetWarning = "⚠ Autostart kan bare installeres med den publiserte .exe-filen.",
        AutostartPublishHint = "  Kjør 'dotnet publish' først.",
        RegistryNotAccessible = "Registernøkkel ikke tilgjengelig.",
        AutostartInstalled = "✔ Autostart registrert: {0} --tray",
        AutostartInfo = "  Appen starter i systemfeltet ved neste innlogging.",
        AutostartRemoved = "✔ Autostart-oppføring fjernet.",
        AutostartStartNow = "Starte i bakgrunnen nå? [J/n] ",

        RawLabel = "Rå YAML",
        BtnSave = "Lagre",
        BtnCancel = "Avbryt",
        AddDevice = "+ Legg til enhet",
        BrowseTitle = "Velg kjørbar fil",
        BrowseFilter = "Kjørbare filer (*.exe)|*.exe|Alle filer (*.*)|*.*",
        LabelDeviceId = "Enhets-ID:",
        LabelProgram = "Kjørbar fil:",
        LabelArgs = "Argumenter:",
        DeleteConfirmTitle = "Bekreft sletting",
        DeleteConfirmBody = "Slette denne enhetstilordningen?",
        ErrorTitle = "Feil",
        SaveError = "Feil ved lagring:\n{0}",
        NoRecentDevices = "Ingen nylige enheter",
        QuickSelectHint = "Koble til og fra en enhet for å fylle hurtigvalglisten (▾)",
    };

    static LocStrings Fi => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Muokkaa asetuksia…",
        MenuOpenFolder = "Avaa asetukset/lokikansio",
        MenuExit = "Poistu",

        MissingMappings = "Asetuksista ei löydy laiteyhdistyksiä.",
        ConfigFile = "Asetustiedosto",
        Started = "USB-laitteiden seuranta käynnistyi",
        ExitHint = "Poistu painamalla Ctrl+C",
        ConfigCount = "Asetusmerkinnät",
        Stopped = "Sovellus pysäytetty.",

        UsbDevice = "USB-laite",
        Connected = "yhdistetty",
        Disconnected = "irrotettu",

        ProcessAlreadyRunning = "  Prosessi on jo käynnissä (PID {0}): {1}",
        ProcessStarted = "  ▶ Prosessi käynnistetty: {0} (PID {1})",
        ProcessStartedShort = "Käynnistetty: {0}",
        ProcessStartError = "  ✖ Virhe prosessin käynnistämisessä: {0}",
        ProcessStopped = "  ◀ Prosessi pysäytetty: {0} (PID {1})",
        ProcessStoppedShort = "Pysäytetty: {0}",
        ProcessStopError = "  ✖ Virhe prosessin pysäyttämisessä: {0}",

        CannotResolveProcessPath = "Prosessin polkua ei voitu selvittää.",
        AutostartDotnetWarning = "⚠ Automaattinen käynnistys voidaan asentaa vain julkaistun .exe-tiedoston kanssa.",
        AutostartPublishHint = "  Suorita ensin 'dotnet publish'.",
        RegistryNotAccessible = "Rekisteriavain ei ole saavutettavissa.",
        AutostartInstalled = "✔ Automaattinen käynnistys rekisteröity: {0} --tray",
        AutostartInfo = "  Sovellus käynnistyy ilmaisinalueella seuraavalla kirjautumiskerralla.",
        AutostartRemoved = "✔ Automaattinen käynnistys -merkintä poistettu.",
        AutostartStartNow = "Käynnistetäänkö taustalla nyt? [K/n] ",

        RawLabel = "Raaka YAML",
        BtnSave = "Tallenna",
        BtnCancel = "Peruuta",
        AddDevice = "+ Lisää laite",
        BrowseTitle = "Valitse suoritettava tiedosto",
        BrowseFilter = "Suoritettavat tiedostot (*.exe)|*.exe|Kaikki tiedostot (*.*)|*.*",
        LabelDeviceId = "Laitetunnus:",
        LabelProgram = "Suoritettava tiedosto:",
        LabelArgs = "Argumentit:",
        DeleteConfirmTitle = "Vahvista poisto",
        DeleteConfirmBody = "Poistetaanko tämä laiteyhdistys?",
        ErrorTitle = "Virhe",
        SaveError = "Virhe tallennuksessa:\n{0}",
        NoRecentDevices = "Ei viimeaikaisia laitteita",
        QuickSelectHint = "Yhdistä ja irrota laite täyttääksesi pikavalintaluettelon (▾)",
    };

    static LocStrings Et => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Muuda konfiguratsiooni…",
        MenuOpenFolder = "Ava konfig-/logikaust",
        MenuExit = "Välju",

        MissingMappings = "Konfiguratsioonis ei leitud seadme vastendusi.",
        ConfigFile = "Konfiguratsioonifail",
        Started = "USB-seadme monitor käivitatud",
        ExitHint = "Väljumiseks vajuta Ctrl+C",
        ConfigCount = "Konfiguratsiooni kirjed",
        Stopped = "Rakendus peatatud.",

        UsbDevice = "USB-seade",
        Connected = "ühendatud",
        Disconnected = "lahutatud",

        ProcessAlreadyRunning = "  Protsess töötab juba (PID {0}): {1}",
        ProcessStarted = "  ▶ Protsess käivitatud: {0} (PID {1})",
        ProcessStartedShort = "Käivitatud: {0}",
        ProcessStartError = "  ✖ Viga protsessi käivitamisel: {0}",
        ProcessStopped = "  ◀ Protsess peatatud: {0} (PID {1})",
        ProcessStoppedShort = "Peatatud: {0}",
        ProcessStopError = "  ✖ Viga protsessi peatamisel: {0}",

        CannotResolveProcessPath = "Protsessi teed ei saanud määrata.",
        AutostartDotnetWarning = "⚠ Automaatkäivitust saab installida ainult avaldatud .exe-ga.",
        AutostartPublishHint = "  Käivita esmalt 'dotnet publish'.",
        RegistryNotAccessible = "Registrivõti pole ligipääsetav.",
        AutostartInstalled = "✔ Automaatkäivitus registreeritud: {0} --tray",
        AutostartInfo = "  Rakendus käivitub salve järgmisel sisselogimisel.",
        AutostartRemoved = "✔ Automaatkäivituse kirje eemaldatud.",
        AutostartStartNow = "Käivitada taustal kohe? [J/n] ",

        RawLabel = "Toores YAML",
        BtnSave = "Salvesta",
        BtnCancel = "Tühista",
        AddDevice = "+ Lisa seade",
        BrowseTitle = "Vali käivitatav fail",
        BrowseFilter = "Käivitatavad failid (*.exe)|*.exe|Kõik failid (*.*)|*.*",
        LabelDeviceId = "Seadme ID:",
        LabelProgram = "Käivitatav fail:",
        LabelArgs = "Argumendid:",
        DeleteConfirmTitle = "Kinnita kustutamine",
        DeleteConfirmBody = "Kustutada see seadme vastendus?",
        ErrorTitle = "Viga",
        SaveError = "Viga salvestamisel:\n{0}",
        NoRecentDevices = "Hiljutised seadmed puuduvad",
        QuickSelectHint = "Ühenda ja lahuta seade kiirvalikuloendi (▾) täitmiseks",
    };

    static LocStrings Lv => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Rediģēt konfigurāciju…",
        MenuOpenFolder = "Atvērt konfigurācijas/žurnāla mapi",
        MenuExit = "Iziet",

        MissingMappings = "Konfigurācijā nav atrasts neviena ierīces kartējums.",
        ConfigFile = "Konfigurācijas fails",
        Started = "USB ierīces monitors startēts",
        ExitHint = "Nospiediet Ctrl+C, lai izietu",
        ConfigCount = "Konfigurācijas ieraksti",
        Stopped = "Lietojumprogramma apturēta.",

        UsbDevice = "USB ierīce",
        Connected = "pievienota",
        Disconnected = "atvienota",

        ProcessAlreadyRunning = "  Process jau darbojas (PID {0}): {1}",
        ProcessStarted = "  ▶ Process startēts: {0} (PID {1})",
        ProcessStartedShort = "Startēts: {0}",
        ProcessStartError = "  ✖ Kļūda, startējot procesu: {0}",
        ProcessStopped = "  ◀ Process apturēts: {0} (PID {1})",
        ProcessStoppedShort = "Apturēts: {0}",
        ProcessStopError = "  ✖ Kļūda, apturot procesu: {0}",

        CannotResolveProcessPath = "Nevarēja noteikt procesa ceļu.",
        AutostartDotnetWarning = "⚠ Automātisko startēšanu var instalēt tikai ar publicēto .exe.",
        AutostartPublishHint = "  Vispirms palaidiet 'dotnet publish'.",
        RegistryNotAccessible = "Reģistra atslēga nav pieejama.",
        AutostartInstalled = "✔ Automātiskā startēšana reģistrēta: {0} --tray",
        AutostartInfo = "  Lietotne startēs paziņojumu apgabalā nākamajā pieteikšanās reizē.",
        AutostartRemoved = "✔ Automātiskās startēšanas ieraksts noņemts.",
        AutostartStartNow = "Startēt fonā tagad? [J/n] ",

        RawLabel = "Neapstrādāts YAML",
        BtnSave = "Saglabāt",
        BtnCancel = "Atcelt",
        AddDevice = "+ Pievienot ierīci",
        BrowseTitle = "Atlasīt izpildāmo failu",
        BrowseFilter = "Izpildāmie faili (*.exe)|*.exe|Visi faili (*.*)|*.*",
        LabelDeviceId = "Ierīces ID:",
        LabelProgram = "Izpildāmais fails:",
        LabelArgs = "Argumenti:",
        DeleteConfirmTitle = "Apstiprināt dzēšanu",
        DeleteConfirmBody = "Dzēst šo ierīces kartējumu?",
        ErrorTitle = "Kļūda",
        SaveError = "Kļūda, saglabājot:\n{0}",
        NoRecentDevices = "Nav nesenu ierīču",
        QuickSelectHint = "Pievienojiet un atvienojiet ierīci, lai aizpildītu ātrās atlases sarakstu (▾)",
    };

    static LocStrings Lt => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Redaguoti konfigūraciją…",
        MenuOpenFolder = "Atidaryti konfigūracijos/žurnalo aplanką",
        MenuExit = "Išeiti",

        MissingMappings = "Konfigūracijoje nerasta jokių įrenginio susiejimų.",
        ConfigFile = "Konfigūracijos failas",
        Started = "USB įrenginio stebėjimas pradėtas",
        ExitHint = "Paspauskite Ctrl+C, kad išeitumėte",
        ConfigCount = "Konfigūracijos įrašai",
        Stopped = "Programa sustabdyta.",

        UsbDevice = "USB įrenginys",
        Connected = "prijungtas",
        Disconnected = "atjungtas",

        ProcessAlreadyRunning = "  Procesas jau veikia (PID {0}): {1}",
        ProcessStarted = "  ▶ Procesas paleistas: {0} (PID {1})",
        ProcessStartedShort = "Paleistas: {0}",
        ProcessStartError = "  ✖ Klaida paleidžiant procesą: {0}",
        ProcessStopped = "  ◀ Procesas sustabdytas: {0} (PID {1})",
        ProcessStoppedShort = "Sustabdytas: {0}",
        ProcessStopError = "  ✖ Klaida sustabdant procesą: {0}",

        CannotResolveProcessPath = "Nepavyko nustatyti proceso kelio.",
        AutostartDotnetWarning = "⚠ Automatinis paleidimas gali būti įdiegtas tik su publikuotu .exe.",
        AutostartPublishHint = "  Pirmiausia paleiskite 'dotnet publish'.",
        RegistryNotAccessible = "Registro raktas nepasiekiamas.",
        AutostartInstalled = "✔ Automatinis paleidimas užregistruotas: {0} --tray",
        AutostartInfo = "  Programa bus paleista sisteminėje juostoje kito prisijungimo metu.",
        AutostartRemoved = "✔ Automatinio paleidimo įrašas pašalintas.",
        AutostartStartNow = "Paleisti fone dabar? [T/n] ",

        RawLabel = "Neapdorotas YAML",
        BtnSave = "Išsaugoti",
        BtnCancel = "Atšaukti",
        AddDevice = "+ Pridėti įrenginį",
        BrowseTitle = "Pasirinkite vykdomąjį failą",
        BrowseFilter = "Vykdomieji failai (*.exe)|*.exe|Visi failai (*.*)|*.*",
        LabelDeviceId = "Įrenginio ID:",
        LabelProgram = "Vykdomasis failas:",
        LabelArgs = "Argumentai:",
        DeleteConfirmTitle = "Patvirtinti ištrynimą",
        DeleteConfirmBody = "Ištrinti šį įrenginio susiejimą?",
        ErrorTitle = "Klaida",
        SaveError = "Klaida išsaugant:\n{0}",
        NoRecentDevices = "Nėra naujausių įrenginių",
        QuickSelectHint = "Prijunkite ir atjunkite įrenginį, kad užpildytumėte greito pasirinkimo sąrašą (▾)",
    };

    static LocStrings Pl => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Edytuj konfigurację…",
        MenuOpenFolder = "Otwórz folder konfig./dziennika",
        MenuExit = "Wyjdź",

        MissingMappings = "W konfiguracji nie znaleziono mapowań urządzeń.",
        ConfigFile = "Plik konfiguracyjny",
        Started = "Monitor urządzeń USB uruchomiony",
        ExitHint = "Naciśnij Ctrl+C, aby wyjść",
        ConfigCount = "Wpisy konfiguracji",
        Stopped = "Aplikacja zatrzymana.",

        UsbDevice = "Urządzenie USB",
        Connected = "podłączone",
        Disconnected = "odłączone",

        ProcessAlreadyRunning = "  Proces już działa (PID {0}): {1}",
        ProcessStarted = "  ▶ Proces uruchomiony: {0} (PID {1})",
        ProcessStartedShort = "Uruchomiony: {0}",
        ProcessStartError = "  ✖ Błąd uruchamiania procesu: {0}",
        ProcessStopped = "  ◀ Proces zatrzymany: {0} (PID {1})",
        ProcessStoppedShort = "Zatrzymany: {0}",
        ProcessStopError = "  ✖ Błąd zatrzymywania procesu: {0}",

        CannotResolveProcessPath = "Nie można ustalić ścieżki procesu.",
        AutostartDotnetWarning = "⚠ Autostart można zainstalować tylko z opublikowanym plikiem .exe.",
        AutostartPublishHint = "  Najpierw uruchom 'dotnet publish'.",
        RegistryNotAccessible = "Klucz rejestru jest niedostępny.",
        AutostartInstalled = "✔ Autostart zarejestrowany: {0} --tray",
        AutostartInfo = "  Aplikacja uruchomi się w zasobniku przy następnym logowaniu.",
        AutostartRemoved = "✔ Wpis autostartu usunięty.",
        AutostartStartNow = "Uruchomić w tle teraz? [T/n] ",

        RawLabel = "Surowy YAML",
        BtnSave = "Zapisz",
        BtnCancel = "Anuluj",
        AddDevice = "+ Dodaj urządzenie",
        BrowseTitle = "Wybierz plik wykonywalny",
        BrowseFilter = "Pliki wykonywalne (*.exe)|*.exe|Wszystkie pliki (*.*)|*.*",
        LabelDeviceId = "ID urządzenia:",
        LabelProgram = "Plik wykonywalny:",
        LabelArgs = "Argumenty:",
        DeleteConfirmTitle = "Potwierdź usunięcie",
        DeleteConfirmBody = "Usunąć to mapowanie urządzenia?",
        ErrorTitle = "Błąd",
        SaveError = "Błąd podczas zapisywania:\n{0}",
        NoRecentDevices = "Brak ostatnich urządzeń",
        QuickSelectHint = "Podłącz i odłącz urządzenie, aby wypełnić listę szybkiego wyboru (▾)",
    };

    static LocStrings Cs => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Upravit konfiguraci…",
        MenuOpenFolder = "Otevřít složku konfig./protokolu",
        MenuExit = "Ukončit",

        MissingMappings = "V konfiguraci nebylo nalezeno žádné mapování zařízení.",
        ConfigFile = "Konfigurační soubor",
        Started = "Monitor USB zařízení spuštěn",
        ExitHint = "Stisknutím Ctrl+C ukončíte",
        ConfigCount = "Záznamy konfigurace",
        Stopped = "Aplikace zastavena.",

        UsbDevice = "USB zařízení",
        Connected = "připojeno",
        Disconnected = "odpojeno",

        ProcessAlreadyRunning = "  Proces již běží (PID {0}): {1}",
        ProcessStarted = "  ▶ Proces spuštěn: {0} (PID {1})",
        ProcessStartedShort = "Spuštěno: {0}",
        ProcessStartError = "  ✖ Chyba při spuštění procesu: {0}",
        ProcessStopped = "  ◀ Proces zastaven: {0} (PID {1})",
        ProcessStoppedShort = "Zastaveno: {0}",
        ProcessStopError = "  ✖ Chyba při zastavení procesu: {0}",

        CannotResolveProcessPath = "Nelze určit cestu k procesu.",
        AutostartDotnetWarning = "⚠ Automatické spuštění lze nainstalovat pouze s publikovaným souborem .exe.",
        AutostartPublishHint = "  Nejprve spusťte 'dotnet publish'.",
        RegistryNotAccessible = "Klíč registru není přístupný.",
        AutostartInstalled = "✔ Automatické spuštění registrováno: {0} --tray",
        AutostartInfo = "  Aplikace se spustí na hlavním panelu při příštím přihlášení.",
        AutostartRemoved = "✔ Záznam automatického spuštění odebrán.",
        AutostartStartNow = "Spustit na pozadí nyní? [A/n] ",

        RawLabel = "Surový YAML",
        BtnSave = "Uložit",
        BtnCancel = "Zrušit",
        AddDevice = "+ Přidat zařízení",
        BrowseTitle = "Vyberte spustitelný soubor",
        BrowseFilter = "Spustitelné soubory (*.exe)|*.exe|Všechny soubory (*.*)|*.*",
        LabelDeviceId = "ID zařízení:",
        LabelProgram = "Spustitelný soubor:",
        LabelArgs = "Argumenty:",
        DeleteConfirmTitle = "Potvrdit odstranění",
        DeleteConfirmBody = "Odstranit toto mapování zařízení?",
        ErrorTitle = "Chyba",
        SaveError = "Chyba při ukládání:\n{0}",
        NoRecentDevices = "Žádná nedávná zařízení",
        QuickSelectHint = "Připojte a odpojte zařízení, abyste naplnili seznam rychlého výběru (▾)",
    };

    static LocStrings Hr => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Uredi konfiguraciju…",
        MenuOpenFolder = "Otvori mapu konfig./dnevnika",
        MenuExit = "Izlaz",

        MissingMappings = "U konfiguraciji nisu pronađena mapiranja uređaja.",
        ConfigFile = "Konfiguracijska datoteka",
        Started = "Monitor USB uređaja pokrenut",
        ExitHint = "Pritisnite Ctrl+C za izlaz",
        ConfigCount = "Konfiguracije unosa",
        Stopped = "Aplikacija zaustavljena.",

        UsbDevice = "USB uređaj",
        Connected = "spojen",
        Disconnected = "odvojen",

        ProcessAlreadyRunning = "  Proces već radi (PID {0}): {1}",
        ProcessStarted = "  ▶ Proces pokrenut: {0} (PID {1})",
        ProcessStartedShort = "Pokrenuto: {0}",
        ProcessStartError = "  ✖ Greška pri pokretanju procesa: {0}",
        ProcessStopped = "  ◀ Proces zaustavljen: {0} (PID {1})",
        ProcessStoppedShort = "Zaustavljeno: {0}",
        ProcessStopError = "  ✖ Greška pri zaustavljanju procesa: {0}",

        CannotResolveProcessPath = "Nije moguće odrediti putanju procesa.",
        AutostartDotnetWarning = "⚠ Automatsko pokretanje može se instalirati samo s objavljenim .exe-om.",
        AutostartPublishHint = "  Najprije pokrenite 'dotnet publish'.",
        RegistryNotAccessible = "Ključ registra nije dostupan.",
        AutostartInstalled = "✔ Automatsko pokretanje registrirano: {0} --tray",
        AutostartInfo = "  Aplikacija će se pokrenuti u sistemskoj traci pri sljedećoj prijavi.",
        AutostartRemoved = "✔ Unos automatskog pokretanja uklonjen.",
        AutostartStartNow = "Pokrenuti u pozadini sada? [D/n] ",

        RawLabel = "Sirovi YAML",
        BtnSave = "Spremi",
        BtnCancel = "Odustani",
        AddDevice = "+ Dodaj uređaj",
        BrowseTitle = "Odaberite izvršnu datoteku",
        BrowseFilter = "Izvršne datoteke (*.exe)|*.exe|Sve datoteke (*.*)|*.*",
        LabelDeviceId = "ID uređaja:",
        LabelProgram = "Izvršna datoteka:",
        LabelArgs = "Argumenti:",
        DeleteConfirmTitle = "Potvrdi brisanje",
        DeleteConfirmBody = "Izbrisati ovo mapiranje uređaja?",
        ErrorTitle = "Greška",
        SaveError = "Greška pri spremanju:\n{0}",
        NoRecentDevices = "Nema nedavnih uređaja",
        QuickSelectHint = "Spojite i odvojite uređaj za popunjavanje popisa brzog odabira (▾)",
    };

    static LocStrings Sq => new()
    {
        AppName = "USB Event",
        MenuEditConfig = "Ndrysho konfigurimin…",
        MenuOpenFolder = "Hap dosjen konfig./regjistri",
        MenuExit = "Dil",

        MissingMappings = "Nuk u gjetën harta pajisjesh në konfigurim.",
        ConfigFile = "Skedari i konfigurimit",
        Started = "Monitori i pajisjes USB u nis",
        ExitHint = "Shtypni Ctrl+C për të dalë",
        ConfigCount = "Shënimet e konfigurimit",
        Stopped = "Aplikacioni u ndal.",

        UsbDevice = "Pajisje USB",
        Connected = "e lidhur",
        Disconnected = "e shkëputur",

        ProcessAlreadyRunning = "  Procesi është duke ekzekutuar (PID {0}): {1}",
        ProcessStarted = "  ▶ Procesi u nis: {0} (PID {1})",
        ProcessStartedShort = "Nisur: {0}",
        ProcessStartError = "  ✖ Gabim gjatë nisjes së procesit: {0}",
        ProcessStopped = "  ◀ Procesi u ndal: {0} (PID {1})",
        ProcessStoppedShort = "Ndalur: {0}",
        ProcessStopError = "  ✖ Gabim gjatë ndaljes së procesit: {0}",

        CannotResolveProcessPath = "Nuk mund të përcaktohet rruga e procesit.",
        AutostartDotnetWarning = "⚠ Nisja automatike mund të instalohet vetëm me .exe të publikuar.",
        AutostartPublishHint = "  Ekzekutoni fillimisht 'dotnet publish'.",
        RegistryNotAccessible = "Çelësi i regjistrit nuk është i aksesueshëm.",
        AutostartInstalled = "✔ Nisja automatike u regjistrua: {0} --tray",
        AutostartInfo = "  Aplikacioni do të niset në ikonën e sistemit në hyrjen tjetër.",
        AutostartRemoved = "✔ Shënimi i nisjes automatike u hoq.",
        AutostartStartNow = "Të niset në sfond tani? [P/n] ",

        RawLabel = "YAML i papërpunuar",
        BtnSave = "Ruaj",
        BtnCancel = "Anulo",
        AddDevice = "+ Shto pajisje",
        BrowseTitle = "Zgjidh skedarin e ekzekutueshëm",
        BrowseFilter = "Skedarë të ekzekutueshëm (*.exe)|*.exe|Të gjitha skedarët (*.*)|*.*",
        LabelDeviceId = "ID e pajisjes:",
        LabelProgram = "Skedari i ekzekutueshëm:",
        LabelArgs = "Argumentet:",
        DeleteConfirmTitle = "Konfirmo fshirjen",
        DeleteConfirmBody = "Fshini këtë hartë pajisje?",
        ErrorTitle = "Gabim",
        SaveError = "Gabim gjatë ruajtjes:\n{0}",
        NoRecentDevices = "Nuk ka pajisje të fundit",
        QuickSelectHint = "Lidhni dhe shkëputni një pajisje për të mbushur listën e zgjedhjes së shpejtë (▾)",
    };
}
