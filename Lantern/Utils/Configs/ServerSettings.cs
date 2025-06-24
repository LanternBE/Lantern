using Tomlet.Attributes;

namespace Lantern.Utils.Configs;

public class ServerSettings {
    
    [TomlProperty("ServerIp")]
    public string ServerIp { get; init; } = "0.0.0.0";
    
    [TomlProperty("Port")]
    public int Port { get; init; } = 19132;
    
    [TomlProperty("EnableIPv6")]
    public bool EnableIpv6 { get; init; }
    
    [TomlProperty("IPv6Port")]
    public int Ipv6Port { get; init; } = 19133;
    
    [TomlProperty("MaxPlayers")]
    public int MaxPlayers { get; init; } = 10;
    
    [TomlProperty("ServerName")]
    public string ServerName { get; init; } = $"{TextFormat.Aqua}Lantern{TextFormat.Reset}";
    
    [TomlProperty("Motd")]
    public string Motd { get; init; } = $"{TextFormat.Gold}Fast Light!{TextFormat.Reset}";
    
    [TomlProperty("OnlineMode")]
    public bool OnlineMode { get; init; } = true;
    
    [TomlProperty("GameMode")]
    public string GameMode { get; init; } = "Survival";
    
    [TomlProperty("ViewDistance")]
    public int ViewDistance { get; init; } = 10;
    
    [TomlProperty("DebugMode")]
    public bool DebugMode { get; init; }
}