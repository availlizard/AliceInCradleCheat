using BepInEx.Configuration;
using HarmonyLib;
using nel;

namespace AliceInCradleCheat
{
    // ##############################
    // Lock basic status
    // ##############################
    public class LockStatus : BasePatchClass
    {
        private static ConfigEntry<bool> basic_switch_def;
        private static ConfigEntry<int> hp_def;
        private static ConfigEntry<int> mp_def;
        private static ConfigEntry<bool> ar_switch_def;
        private static ConfigEntry<int> ep_def;
        public LockStatus()
        {
            string section = "BasicStatus";
            basic_switch_def = TrackBindConfig(section, "LockSwitch", false);
            //basic_switch_def.Value = false;
            hp_def = TrackBindConfig(section, "HP", 100, new AcceptableValueRange<int>(0, 100), true);
            mp_def = TrackBindConfig(section, "MP", 100, new AcceptableValueRange<int>(0, 100), true);
            section = "PervertFunctions";
            ar_switch_def = TrackBindConfig(section, "EPLockSwitch", false);
            //ar_switch_def.Value = false;
            ep_def = TrackBindConfig(section, "EP", 0, new AcceptableValueRange<int>(0, 1000));
            TryPatch(GetType());
        }
        /*
        [HarmonyPostfix, HarmonyPatch(typeof(COOK), "initGameScene")]
        private static void FetchMaxiumStatus()
        {
            PRNoel noel = MainReference.GetNoel();
            if (noel != null)
            {
                hp_def.Value = (int)noel.get_maxhp();
                mp_def.Value = (int)noel.get_maxmp();
                ep_def.Value = 0;
            }
        }
        // */
        [HarmonyPostfix, HarmonyPatch(typeof(SceneGame), "Update")]
        private static void PatchContent()
        {
            PRNoel noel = MainReference.GetNoel();
            if (noel == null)
            {
                return;
            }
            if (basic_switch_def.Value)
            {
                int max_hp = (int)noel.get_maxhp();
                int max_mp = (int)noel.get_maxmp();
                int set_hp = hp_def.Value * max_hp / 100;
                int set_mp = mp_def.Value * max_mp / 100;
                max_mp -= noel.EggCon.total;
                set_mp = set_mp < max_mp ? set_mp : max_mp;
                Traverse.Create(noel).Field("hp").SetValue(set_hp);
                Traverse.Create(noel).Field("mp").SetValue(set_mp);
                if (noel.UP != null && noel.UP.isActive())
                {
                    UIStatus.Instance.fineHpRatio(true, false);
                    UIStatus.Instance.fineMpRatio(true, false);
                }
            }
            if (ar_switch_def.Value)
            {
                noel.ep = ep_def.Value;
                noel.EpCon.fineCounter();
            }
        }
    }
}
