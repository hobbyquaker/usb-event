using System.Collections.Generic;

// ── Modelle ────────────────────────────────────────────────────────────────────

class DeviceMapping
{
    public string DeviceId      { get; set; } = string.Empty;
    public string Executable    { get; set; } = string.Empty;
    public string? Arguments    { get; set; }
    public bool StartOnPlugIn   { get; set; } = true;
    public bool KillOnPlugOut   { get; set; } = true;
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
