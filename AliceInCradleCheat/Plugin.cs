using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using m2d;

namespace AliceInCradleCheat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("AliceInCradle.exe")]
    public class AICCheat : BaseUnityPlugin
    {
        internal static ManualLogSource cheat_logger;
        public static ConfigFile config;

        private void Start()
        {
            cheat_logger = Logger;
            config = Config;
            string config_file_path = config.ConfigFilePath;
            if (File.Exists(config_file_path))
            {
                config.Reload();
            }
            LocNames.InitializeLocNames();
            // Plugin startup logic
            _ = new MainReference();

            _ = new MenuLocalization();
            _ = new AddCustomText();

            _ = new LockStatus();
            _ = new SuperNoel();
            _ = new SetGameValue();
            _ = new RestrictionLift();
            _ = new NonHModeEnhance();
            _ = new PervertFuncs();
            _ = new SpecialItemEffect();
            _ = new AdditionalDrop();
            _ = new OtherFuncs();
            cheat_logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}

