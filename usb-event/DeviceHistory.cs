using System.Text;
using System.Text.Json;

// ── Device-Verlauf ────────────────────────────────────────────────────────────

record RecentDevice(string DeviceId, string Name, DateTime LastSeen);

static class DeviceHistory
{
    const int MaxEntries = 4;

    static string? _path;

    static readonly List<string> _postStartIds = [];
    static readonly object       _postStartLock = new();

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

    // Tracks devices that connected after program start (in-memory, insertion order).
    public static void MarkPostStart(string deviceId)
    {
        lock (_postStartLock)
        {
            _postStartIds.RemoveAll(id => id.Equals(deviceId, StringComparison.OrdinalIgnoreCase));
            _postStartIds.Add(deviceId);
        }
    }

    // Returns post-start device IDs in insertion order (oldest first).
    public static IReadOnlyList<string> GetPostStartIds()
    {
        lock (_postStartLock)
            return [.._postStartIds];
    }
}
