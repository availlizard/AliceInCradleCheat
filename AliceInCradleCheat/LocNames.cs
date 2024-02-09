using System;
using System.Text;
using System.Collections.Generic;
using BepInEx.Configuration;
using XX;
using HarmonyLib;

namespace AliceInCradleCheat
{
    public class MenuLocalization : BasePatchClass
    {
        public MenuLocalization () {
            TryPatch(GetType());
        }
        [HarmonyPostfix, HarmonyPatch(typeof(TX), "changeFamily")]
        private static void PatchContent()
        {
            string lang = TX.getCurrentFamilyName();
            /*
            key, simple_name, full_name
            en, EN, English
            zh-cn, CN, 中文(CN)A-v0.22
            zh-cnB, CN, 中文(CN)B-v0.22
            zh-tc, TC, 中文(TC)-v0.22
            // */
            lang = new List<string> { "zh-cn", "zh-cnB", "zh-tc" }.Contains(lang) ? "zh-cn" : "en";
            if (lang != LocNames.menu_lang)
            {
                LocNames.menu_lang = lang;
                LocNames.ResetLocName();
            }
            AdditionalDrop.ResetItemNames();
        }
    }
    public class AddCustomText : BasePatchClass
    {
        private static readonly string[] custom_lang_list ={ "en", "zh-cn" };
        private static readonly Dictionary<string, string> lang_target = new() {
            { "en", "en" },
            { "zh-cn", "zh-cn" },
            { "zh-cnB", "zh-cn" },
        };
        private static readonly string custom_text = "RVBfcHJvY2Vzc19zYWRpc21fb3RoZXIJRGVhbGluZyBkYW1hZ2UgdG8gdGhlb" +
            "SBtYWtlcyBtZSBmZWVsIGdvb2QJ6K+66Im+5bCU5oSf5Y+X5Yiw5pa96JmQ55qE5b+r5LmQDQpFUF9maW5pc2hfc2FkaXNtCUkgY2x" +
            "pbWF4ZWQgZnJvbSBkZWFsaW5nIGRhbWFnZSB0byBteSBlbmVteQnmhJ/lj5fnnYDmlr3omZDnmoTlv6vkuZDovr7liLDkuobpq5jmv" +
            "a4NCkFsZXJ0X2VwX29yZ2FzbV9zYWRpc20JSSBjbGltYXhlZCBmcm9tIGRlYWxpbmcgZGFtYWdlIHRvIG15IGVuZW15CeWboOS4uua" +
            "WveiZkOeahOW/q+S5kOmrmOa9ruS6huKApg0KRVBfcHJvY2Vzc19tYXNvY2hpc21fb3RoZXIJSSBjYW4gZmVlbCB0aGUgcGxlYXN1c" +
            "mUgd2l0aCBwYWluCeivuuiJvuWwlOaEn+WPl+WIsOiiq+iZkOeahOW/q+aEnw0KRVBfZmluaXNoX21hc29jaGlzbQlJIGNsaW1heGV" +
            "kIGZyb20gdGhlIHBsZWFzdXJlIHdpdGggcGFpbgnmhJ/lj5fnnYDnlrznl5vlkozlv6vmhJ/ovr7liLDkuobpq5jmva4NCkFsZXJ0X" +
            "2VwX29yZ2FzbV9tYXNvY2hpc20JSSBjbGltYXhlZCBmcm9tIHRoZSBwYWluLi4uCeWboOS4uui6q+S9k+eahOeWvOeXm+mrmOa9ruS" +
            "6huKApg0KRW5lbXlfc2FkaXNtCVNhZGlzbQnmlr3omZANCkVuZW15X21hc29jaGlzbQlNYXNvY2hpc20J5Y+X6JmQ";
        public AddCustomText()
        {
            TryPatch(GetType());
        }
        [HarmonyPostfix, HarmonyPatch(typeof(TX), "reloadTx")]
        private static void PatchContent()
        {

            List<TX.TXFamily> tx_fam_list = new();
            foreach (string key in lang_target.Keys)
            {
                tx_fam_list.Add(TX.getFamilyByName(key));
            }
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(custom_text));
            foreach(string line in decoded.Split(char.Parse("\n")))
            {
                // text_array: tx_key, lang1, lang2, lang3, ...
                string[] text_array = line.Replace("\n", "").Replace("\r", "").Split(char.Parse("\t"));
                if (text_array.Length != custom_lang_list.Length + 1) { continue; }
                foreach (TX.TXFamily tx_fam in tx_fam_list)
                {
                    if (tx_fam == null) { continue; }
                    int index = Array.IndexOf(custom_lang_list, lang_target[tx_fam.key]);
                    if (index == -1 ) { continue; }
                    TX.getTX(text_array[0], false, false, tx_fam).replaceTextContents(text_array[index + 1]);
                }
            }
        }
    }
    public static class LocNames
    {
        public static string menu_lang = "en";
        public static readonly string[] lang_array = { "en", "zh-cn" };
        public static LocConfigSection[] config_loc_array = {
            new LocConfigSection("BasicStatus", new string[] {"", "基础属性"}, new string[] {
                "LockSwitch", "", "锁定开关",
                "HP", "", "HP",
                "LockSwitch2", "", "锁定开关2",
                "MP", "", "MP",
                "ResetMPBreak", "", "修复MP碎裂",
            }),
            new LocConfigSection("SuperNeol", new string[] {"", "超级诺艾尔"}, new string[] {
                "HPDamageMultiplier", "", "HP伤害倍数",
                "MPDamageMultiplier", "", "MP伤害倍数",
                "InvincibleToMonsters", "", "魔物攻击无效",
                "InfiniteJump", "", "无限跳跃",
                "InfiniteGroundBomb", "InfiniteGroundBomb", "无限地面炸弹",
                "DuralableShield", "", "耐久护盾",
                "DisableGasDamage", "", "水中及毒气中无限氧气，无视虫墙",
                "ImmuneToMapThorn", "", "免疫地图尖刺",
                "ImmuneToLava", "ImmuneToAcid", "免疫酸液",
            }),
            new LocConfigSection("RestrictionLift", new string[] {"", "限制解除"}, new string[] {
                "FastTravel", "", "快速旅行",
                "StorageAccess", "", "仓库使用",
                    "desc_StorageAccess", "Wheel page won't show", "转轮页面将无法显示",
                "ItemUsage", "", "物品使用",
                    "desc_ItemUsage", "Enable item usage while taking damage", "正在受到伤害时也能使用物品",
            }),
            new LocConfigSection("SetGameValues", new string[] {"", "设定数值"}, new string[] {
                "SetMoneyButton", "", "设定钱",
                "Money", "", "钱",
                "SetDangerLevelButton", "", "设定危险度",
                "DangerLevel", "", "危险度",
                "SetWeatherButton", "", "设定天气",
                "Weather", "", "天气",
                    "desc_Weather", "Use a sequence of '0' to '9' to represent the number of certain weather, " +
                        "in the order of 'Normal', 'Whirlwind', 'Thunder', 'Fog', 'Drought', 'Heavy Fog', 'Plague', " +
                        "'Heavy Fog', 'Plague'. Note, conflicted weathers won't show together.", "使用包含0-9的序列" +
                        "表示特定天气出现的次数，按顺序分别为‘通常’、‘旋风’、‘雷暴’、‘雾天’、‘干旱’、‘浓雾’" +
                        "和‘瘟疫’。注意，冲突天气不会同时出现。",
            }),
            new LocConfigSection("NonHModeEnhance", new string[] { "HideMoreSexualContent", "健全模式增强"}, new string[] {
                "ResetHExp", "", "重置经历",
                "DisableGrabAttack", "ImmuneToMonsterGrabAttack", "魔物抓取攻击无效",
                "DisableEpDamage", "", "EP攻击无效",
                    "desc_DisableEpDamage", "Include ep damage without grab attack like aphrodisiac beam", "包括无需抓取的EP攻击，比如人偶弩的紫射线",
                //"DisableEventH", "", "关闭部分事件H",
                "SkipGameOverPlay", "SkipGameOverMonsterAttack", "跳过战败鞭尸场景",
                "DisableWormTrap", "", "关闭虫墙",
            }),
            new LocConfigSection("PervertFunctions", new string[] {"", "畜生模式"}, new string[] {
                "EPLockSwitch", "", "兴奋度锁定开关",
                "EP", "", "兴奋度",
                "EPDamageMultiplier", "", "兴奋度上升倍数",
                "EnableMultipleOrgasmForAll", "", "所有H攻击都可以导致多重高潮",
                "EasierOrgasmWithHighEP", "", "高兴奋度时更容易高潮",
                "Sadism", "", "施虐癖",
                    "desc_Sadism", "Increase EP while dealing damage", "造成伤害时增加兴奋度",
                "Masochism", "", "受虐癖",
                    "desc_Masochism", "Increase EP while taking damage", "受到伤害时增加兴奋度",
                "PlantEggs", "", "一键六色条",
                "LayEggs", "", "一键排出",
                "EroBow", "EphrodisiacBeamOnly", "人偶弩只发射紫射线",
                "EpItemEffect", "ForbiddenFruitEnhance", "禁忌的苹果加强",
                    "desc_EpItemEffect", "Forbidden fruit now add frustrated condition when EP >= 700", "禁忌的苹果会在EP>=700时添加欲火中烧状态",
            }),
            new LocConfigSection("GenerateItemWhenDrop", new string[] {"", "丢弃物品时生成额外物品"}, new string[] {
                "ItemName", "", "物品名称",
                    "desc_ItemName", "Duplicate when set to empty", "设为空表示复制物品",
                "Count", "", "数量",
                "Grade", "", "星级",
            }),
            new LocConfigSection("OtherFunctions", new string[] {"", "其他"}, new string[] {
                "ImmuneToSleep", "", "免疫睡眠",
                "ImmuneToConfuse", "", "免疫混乱",
                "ImmuneToParalysis", "", "免疫麻痹",
                "ImmuneToBurned", "", "免疫着火",
                "ImmuneToFrozen", "", "免疫冻结",
                "ImmuneToJamming", "", "免疫杂念",
                "DisableMosaic", "", "关闭部分马赛克"
            }),
            new LocConfigSection("", new string[] {"", ""}, new string[] {
                "Restart", "Game restart required", "需要重启游戏",
                "option_SameGrade", "Same grade in item selection", "物品选中时的星级",
            }),
        };
        public static Dictionary<string, LocConfigSection> config_loc_dict = new();
        public static void InitializeLocNames()
        {
            int section_order = 0;
            foreach (LocConfigSection loc_section in config_loc_array)
            {
                config_loc_dict[loc_section.name] = loc_section;
                loc_section.order = section_order;
                section_order++;
            }
        }
        public static LocConfigSection GetSection(string section)
        {
            return config_loc_dict[section];
        }
        public static string GetSectionLocName(string section, string lang = "")
        {
            if (config_loc_dict.ContainsKey(section))
            {
                string loc = GetSection(section).Loc_name(lang);
                return "--------------------" +
                    $"{GetSectionOrder(section):D2}.{loc}" +
                    "--------------------";
            }
            return section;
        }
        public static string GetEntryLocName(string section, string key, string lang = "")
        {
            try
            {
                return GetSection(section).entry_dict[key].Loc_name(lang);
            }
            catch
            {
                return key;
            }
        }
        public static string GetLocDesc(string section, string key, string lang = "")
        {
            try
            {
                return GetSection(section).entry_dict[key].Loc_desc(lang);
            }
            catch
            {
                return "";
            }
        }
        public static int GetSectionOrder(string section)
        {
            try
            {
                return GetSection(section).order;
            }
            catch
            {
                return -1;
            }
        }
        public static int GetEntryOrder(string section, string key)
        {
            try
            {
                return GetSection(section).entry_dict[key].order;
            }
            catch
            {
                return -1;
            }
        }
        public static ConfigDescription TryGetEntryDesc(ConfigDefinition con_def)
        {
            try
            {
                if (AICCheat.config.TryGetEntry(con_def, out ConfigEntry<bool> bool_entry))
                {
                    return bool_entry.Description;
                }
            } catch { }
            try
            {
                if (AICCheat.config.TryGetEntry(con_def, out ConfigEntry<int> int_entry))
                {
                    return int_entry.Description;
                }
            } catch { }
            try
            {
                if (AICCheat.config.TryGetEntry(con_def, out ConfigEntry<string> string_entry))
                {
                    return string_entry.Description;
                }
            } catch { }
            try
            {
                if (AICCheat.config.TryGetEntry(con_def, out ConfigEntry<float> float_entry))
                {
                    return float_entry.Description;
                }
            } catch { }
            return null;
        }
        public static void ResetLocName(string lang = null)
        {
            lang ??= menu_lang;
            foreach (ConfigDefinition con_def in AICCheat.config.Keys)
            {
                string section = con_def.Section;
                string key = con_def.Key;
                ConfigDescription desc = TryGetEntryDesc(con_def);
                if (desc != null && desc.Tags != null && desc.Tags.Length > 0)
                {
                    ConfigurationManagerAttributes entry_attr = desc.Tags[0] as ConfigurationManagerAttributes;
                    entry_attr.Category = GetSectionLocName(section, lang);
                    entry_attr.DispName = GetEntryLocName(section, key, lang);
                    entry_attr.Description = GetLocDesc(section, key, lang);
                }
            }
        }
    }
}
