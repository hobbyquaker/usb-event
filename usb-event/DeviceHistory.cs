using System.Text;
using System.Text.Json;

// ── Device-Verlauf ────────────────────────────────────────────────────────────

record RecentDevice(string DeviceId, string Name, DateTime LastSeen);

static class DeviceHistory
{
    const int MaxEntries = 4;

    static string? _path;

    // in-memory ordered list of device IDs that connected after program start (most-recently-connected first)
    static readonly List<string> _postStartIds = [];

    public static void MarkPostStart(string deviceId)
    {
        lock (_postStartIds)
        {
            _postStartIds.RemoveAll(id => id.Equals(deviceId, StringComparison.OrdinalIgnoreCase));
            _postStartIds.Insert(0, deviceId);
        }
    }

    public static IReadOnlyList<string> GetPostStartIds()
    {
        lock (_postStartIds)
            return [.._postStartIds];
    }

    public static void Init(string configDir)
        => _path = Path.Combine(configDir, "device-history.json");

    public static IReadOnlyList<RecentDevice> Load()
    {
        if (_path is null || !File.Exists(_path)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<RecentDevice>>(
                File.ReadAllText(_path, Encoding.UTF8)) ?? [];
        }
        catch { return []; }
    }

    public static void Record(string deviceId, string name)
    {
        if (_path is null) return;
        var list = Load().ToList();
        list.RemoveAll(d => d.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase));
        list.Insert(0, new RecentDevice(deviceId, string.IsNullOrWhiteSpace(name) ? deviceId : name, DateTime.Now));
        if (list.Count > MaxEntries) list = list[..MaxEntries];
        try
        {
            File.WriteAllText(_path,
                JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }),
                Encoding.UTF8);
        }
        catch { }
    }
}
