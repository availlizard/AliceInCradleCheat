using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using nel;
using m2d;
using XX;

namespace AliceInCradleCheat
{
    // ##############################
    // Additional items when drop from bag
    // ##############################
    public class AdditionalDrop : BasePatchClass
    {
        public static string section = "GenerateItemWhenDrop";
        private static Dictionary<string, string> NameToKey;
        private static Dictionary<string, string> KeyToName;
        private static ConfigEntry<string> item_name_def;
        private static ConfigEntry<int> count_def;
        private static ConfigEntry<string> grade_def;
        public AdditionalDrop()
        {
            item_name_def = TrackSetItemCon();
            count_def = TrackBindConfig(section, "Count", 0, new AcceptableValueRange<int>(0, 20));
            grade_def = TrackSetGradeCon();
            TryPatch(GetType());
        }
        public ConfigEntry<string> TrackSetItemCon()
        {
            ConfigEntry<string> entry = SetItemCon();
            config_list.Add(entry.Definition);
            return entry;
        }
        public static ConfigEntry<string> SetItemCon()
        {
            return BindConfig(section, "ItemName", "", new AcceptableValueList<string>(
                LoadItemGetNameArray()));
        }
        public ConfigEntry<string> TrackSetGradeCon()
        {
            ConfigEntry<string> entry = SetGradeCon();
            config_list.Add(entry.Definition);
            return entry;
        }
        public static ConfigEntry<string> SetGradeCon()
        {
            return BindConfig(section, "Grade", LocNames.GetEntryLocName("", "option_SameGrade"),
                new AcceptableValueList<string>(
                    LocNames.GetEntryLocName("", "option_SameGrade"), "1", "2", "3", "4", "5"));
        }
        public static void ResetItemNames ()
        {
            ConfigDefinition con_def = new(section, "Grade");
            if (grade_def != null && AICCheat.config.ContainsKey(con_def))
            {
                string value = grade_def.Value;
                AICCheat.config.Remove(con_def);
                grade_def = SetGradeCon();
                if (new List<string>() { "1", "2", "3", "4", "5" }.Contains(value))
                {
                    grade_def.Value = value;
                }
            }
            con_def = new(section, "ItemName");
            if (item_name_def != null && AICCheat.config.ContainsKey(con_def))
            {
                string value = item_name_def.Value;
                value = value == "" ? "" : NameToKey[value];
                AICCheat.config.Remove(con_def);
                item_name_def = SetItemCon();
                if (value != "")
                {
                    item_name_def.Value = KeyToName[value];
                }
            }
        }
        
        public static string[] LoadItemGetNameArray()
        {
            NameToKey = new() ;
            KeyToName = new();
            if (NelItem.OData == null)
            {
                return new string[1] { "" };
            }
            foreach (NelItem itm in NelItem.OData.Values)
            {
                if (itm == null || itm.key == null)
                {
                    continue;
                }
                string key = itm.key;
                if ((itm.is_precious && key != "enhancer_slot" && key != "oc_slot") ||
                    itm.is_cache_item || itm.is_enhancer || itm.is_reelmbox)
                {
                    continue;
                }
                TX tx;
                try
                {
                    tx = TX.getTX("_NelItem_name_" + key, true, true, null);
                } catch
                {
                    tx = null;
                }
                string localized_name = tx == null ? key : tx.text;
                NameToKey[localized_name] = key;
                KeyToName[key] = localized_name;
            }
            List<string> name_list = new(NameToKey.Keys);

            name_list.Sort(delegate (string x, string y)
            {
                int r = 0;
                r = NelItem.OData[NameToKey[x]].category.CompareTo(
                    NelItem.OData[NameToKey[y]].category);
                return r == 0 ? x.CompareTo(y) : r;
            });
            name_list.Insert(0, "");
            return name_list.ToArray();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UiItemManageBox), "fnClickItemCmd")]
        private static bool PatchContent(ref aBtn B, ref UiItemManageBox __instance)
        {
            int count = count_def.Value;
            if ((B.title != "drop" && B.title != "discard_row" && B.title != "discard_water") || count == 0) { return true; }
            string item_name = item_name_def.Value;
            NelItem Itm;
            if (NelItem.OData.ContainsKey(item_name))
            {
                Itm = NelItem.OData[item_name];
            }
            else if (NameToKey != null && NameToKey.ContainsKey(item_name) &&
                NelItem.OData.ContainsKey(NameToKey[item_name]))
            {
                Itm = NelItem.OData[NameToKey[item_name]];
            }
            else
            {
                Itm = __instance.UsingTarget;
            }
            if ((Itm.is_precious && Itm.key != "enhancer_slot" && Itm.key != "oc_slot") || Itm.is_cache_item || Itm.is_enhancer)
            {
                Itm = __instance.UsingTarget;
            }
            int grade;
            int new_grade = 5;
            bool retain_grade_flag;
            string grade_set =grade_def.Value;
            if (grade_set == LocNames.GetEntryLocName("", "option_SameGrade"))
            {
                retain_grade_flag = true;
            }
            else if (int.TryParse(grade_set, out new_grade))
            {
                retain_grade_flag = false;
            }
            else
            {
                retain_grade_flag = true;
            }
            grade = Itm.individual_grade ? 0 : retain_grade_flag ? __instance.get_grade_cursor() : new_grade - 1;
            if (count > 0)
            {
                NelItemManager.NelItemDrop nelItemDrop = __instance.IMNG.dropManual(Itm, count, grade,
                    __instance.Pr.x, __instance.Pr.y, X.NIXP(-0.003f, -0.07f) * (float)CAim._XD(__instance.Pr.aim, 1),
                    X.NIXP(-0.01f, -0.04f), null, false);
                nelItemDrop.discarded = true;
                nelItemDrop.fineNoelJuice(M2DBase.Instance as NelM2DBase);
            }
            return true;
        }
    }
}
