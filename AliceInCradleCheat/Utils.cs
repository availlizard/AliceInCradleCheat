using System;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using HarmonyLib;
using nel;
using m2d;

namespace AliceInCradleCheat
{
    public abstract class BasePatchClass
    {
        public List<ConfigDefinition> config_list = new();
        public ConfigEntry<T> TrackBindConfig<T>(string section, string key, T val,
            AcceptableValueBase acp_value = null, bool show_percent = false)
        {
            // add config with simple description
            ConfigEntry<T> entry = BindConfig(section, key, val, acp_value, show_percent);
            config_list.Add(entry.Definition);
            return entry;
        }
        public static ConfigEntry<T> BindConfig<T>(string section, string key, T val, 
            AcceptableValueBase acp_value = null, bool show_percent = false)
        {
            // add config with simple description
            ConfigDefinition config_def = new(section, key);
            ConfigDescription config_desc = new(LocNames.GetLocDesc(section, key),
                acp_value, new ConfigurationManagerAttributes
                {
                    DispName = LocNames.GetEntryLocName(section, key),
                    Category = LocNames.GetSectionLocName(section),
                    Order = LocNames.GetEntryOrder(section, key),
                    HideDefaultButton = false,
                    ShowRangeAsPercent = show_percent,
                });
            return AICCheat.config.Bind(config_def, val, config_desc);
        }
        public void RemoveConfigs()
        {
            foreach (ConfigDefinition config_def in config_list)
            {
                AICCheat.config.Remove(config_def);
            }
        }
        public void TryPatch(Type patch_type)
        {
            try
            {
                Harmony.CreateAndPatchAll(patch_type);
            }
            catch //(Exception ex)
            {
                AICCheat.cheat_logger.LogError($"Patch {patch_type} failed!");
                //AICCheat.cheat_logger.LogInfo(ex.ToString());
                RemoveConfigs();
            }
        }
    }
    public class TimedFlag
    {
        /*Attach a timer to a boolean flag,
        If the flag is switched to true, the timer will
        start the count down, once timer reached 0,
        the flag will be set to false;
        //*/
        private readonly ConfigEntry<bool> config_flag;
        private float timer;
        private const float max_time = 0.1f;
        public TimedFlag(ConfigEntry<bool> config_flag)
        {
            this.config_flag = config_flag;
            timer = 0;
        }
        public bool Check()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    config_flag.Value = false;
                    timer = 0;
                }
                else
                {
                    config_flag.Value = true;
                }
                return false;
            }
            else
            {
                if (config_flag.Value)
                {
                    timer = max_time;
                }
                return config_flag.Value;
            }
        }
        public void SetFlag(bool val)
        {
            if (val == false)
            {
                timer = 0;
            }
            config_flag.Value = val;
        }
    }
    internal class MainReference : BasePatchClass
    {
        private static NelM2DBase m2d;
        private static PRNoel noel;
        public MainReference()
        {
            TryPatch(typeof(MainReference));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PRNoel), "newGame")]
        private static void PatchContent()
        {
            FetchNoel();
        }
        internal static void FetchNoel()
        {
            m2d = M2DBase.Instance as NelM2DBase;
            noel = m2d.getPrNoel();
        }
        internal static NelM2DBase GetM2D()
        {
            if (m2d == null)
            {
                FetchNoel();
            }
            return m2d;
        }
        internal static PRNoel GetNoel()
        {
            if (noel == null)
            {
                FetchNoel();
            }
            return noel;
        }
    }
    public class BaseLocClass
    {
        public int order = 0;
        public string name;
        public readonly string[] loc_name_array;
        public BaseLocClass(string name, string[] loc_name_array)
        {
            this.name = name;
            this.loc_name_array = loc_name_array;
        }
        public static int GetLangIndex(string lang = "")
        {
            int i;
            if (lang == "" || lang == null)
            {
                i = Array.IndexOf(LocNames.lang_array, LocNames.menu_lang);
            }
            else
            {
                i = Array.IndexOf(LocNames.lang_array, lang);
            }
            return i == -1 ? 0 : i;
        }
        public string Loc_name(string lang = "")
        {
            int i = GetLangIndex(lang);
            return loc_name_array[i] == "" ? name : loc_name_array[i];
        }
    }
    public class LocConfigSection : BaseLocClass
    {
        // Create localization for BepInEx config sections
        public Dictionary<string, LocConfigEntry> entry_dict = new();
        public LocConfigSection(string name, string[] loc_name_array, string[] entry_loc_array) :
            base(name, loc_name_array)
        {
            int n = LocNames.lang_array.Length + 1;
            int order = 0;
            for (int i = 0; i < entry_loc_array.Length; i += n)
            {
                string entry_key = entry_loc_array[i];
                if (entry_key.StartsWith("desc_") && name != "")
                {
                    continue;
                }
                List<string> loc_list = new();
                for (int j = 1; j < n; j++)
                {
                    loc_list.Add(entry_loc_array[i + j]);
                }
                entry_dict[entry_key] = new(entry_key, loc_list.ToArray(), order);
                order--;
            }
            for (int i = 0; i < entry_loc_array.Length; i += n)
            {
                string entry_key = entry_loc_array[i];
                if (!entry_key.StartsWith("desc_") || name == "")
                {
                    continue;
                }
                entry_key = entry_key.Remove(0, "desc_".Length);
                List<string> loc_list = new();
                for (int j = 1; j < n; j++)
                {
                    loc_list.Add(entry_loc_array[i + j]);
                }
                try
                {
                    entry_dict[entry_key].SetLocDesc(loc_list.ToArray());
                }
                catch
                {
                    AICCheat.cheat_logger.LogError($"key not exist for 'desc_{entry_key}'");
                }
            }
        }
    }
    public class LocConfigEntry : BaseLocClass
    {
        // Create localization for BepInEx config entries
        private string[] loc_desc_array;
        public LocConfigEntry(string name, string[] loc_name_array, int order) :
            base(name, loc_name_array)
        {
            this.order = order;
        }
        public string Loc_desc(string lang = "")
        {
            int i = GetLangIndex(lang);
            return loc_desc_array[i];
        }
        public void SetLocDesc(string[] loc_desc_array)
        {
            this.loc_desc_array = loc_desc_array;
        }
    }
}
