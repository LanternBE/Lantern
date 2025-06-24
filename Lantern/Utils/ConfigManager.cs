using System.Diagnostics.CodeAnalysis;
using Lantern.Utils.Configs;
using RakSharp.Utils;
using Tomlet;

namespace Lantern.Utils;

public class ConfigManager {
    
    private const string ConfigDirectory = "Configurations";
    private const string ConfigFileName = "ServerSettings.toml";

    private readonly string _configPath = Path.Combine(ConfigDirectory, ConfigFileName);
    private readonly ServerSettings _serverSettings;

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Dynamic code required for Tomlet")]
    public ConfigManager() {

        EnsureDirectoryExists();
        _serverSettings = LoadOrCreateConfig();
        ApplyLogLevel();
    }

    /// <summary>
    /// Ensures the configuration directory exists.
    /// </summary>
    private static void EnsureDirectoryExists() {
        
        if (!Directory.Exists(ConfigDirectory)) {
            Directory.CreateDirectory(ConfigDirectory);
        }
    }

    /// <summary>
    /// Loads the server settings config or creates a default one.
    /// </summary>
    [RequiresDynamicCode("Calls Tomlet.TomletMain.To<T>(String, TomlSerializerOptions)")]
    private ServerSettings LoadOrCreateConfig() {
        
        if (!File.Exists(_configPath)) {
            
            var defaultSettings = new ServerSettings();
            var toml = TomletMain.TomlStringFrom(defaultSettings);
            
            File.WriteAllText(_configPath, toml);
            Logger.LogWarn($"{_configPath} not found. Default configuration file has been created.");
            
            return defaultSettings;
        }

        var tomlContent = File.ReadAllText(_configPath);
        var settings = TomletMain.To<ServerSettings>(tomlContent);

        Logger.LogInfo($"{_configPath} loaded.");
        return settings;
    }

    /// <summary>
    /// Applies the minimum logger level based on DebugMode.
    /// </summary>
    private void ApplyLogLevel() {
        Logger.MinimumLevel = _serverSettings.DebugMode ? Logger.LogLevel.Debug : Logger.LogLevel.Info;
    }

    public ServerSettings Settings => _serverSettings;
}
