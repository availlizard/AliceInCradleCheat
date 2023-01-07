using BepInEx.Configuration;
using HarmonyLib;
using nel;
using m2d;

namespace AliceInCradleCheat
{
    // ##############################
    // Other functions
    // ##############################
    public class OtherFuncs
    {
        public OtherFuncs()
        {
            _ = new StatusImmunity();
            _ = new DisableMosaic();
        }
    }
    public class StatusImmunity : BasePatchClass
    {
        private static ConfigEntry<bool> sleep_def;
        private static ConfigEntry<bool> confuse_def;
        private static ConfigEntry<bool> paralysis_def;
        private static ConfigEntry<bool> burned_def;
        private static ConfigEntry<bool> frozen_def;
        private static ConfigEntry<bool> jamming_def;
        public StatusImmunity()
        {
            sleep_def = TrackBindConfig("OtherFunctions", "ImmuneToSleep", false);
            confuse_def = TrackBindConfig("OtherFunctions", "ImmuneToConfuse", false);
            paralysis_def = TrackBindConfig("OtherFunctions", "ImmuneToParalysis", false);
            burned_def = TrackBindConfig("OtherFunctions", "ImmuneToBurned", false);
            frozen_def = TrackBindConfig("OtherFunctions", "ImmuneToFrozen", false);
            jamming_def = TrackBindConfig("OtherFunctions", "ImmuneToJamming", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(M2Ser), "Add")]
        private static bool PatchContent(ref SER ser, ref M2SerItem __result)
        {
            bool cure_flag = false;
            if (ser == SER.SLEEP && sleep_def.Value)
            {
                cure_flag = true;
            }
            else if (ser == SER.CONFUSE && confuse_def.Value)
            {
                cure_flag = true;
            }
            else if (ser == SER.PARALYSIS && paralysis_def.Value)
            {
                cure_flag = true;
            }
            else if (ser == SER.BURNED && burned_def.Value)
            {
                cure_flag = true;
            }
            else if (ser == SER.FROZEN && frozen_def.Value)
            {
                cure_flag = true;
            }
            else if (ser == SER.JAMMING && jamming_def.Value)
            {
                cure_flag = true;
            }
            if (cure_flag)
            {
                __result = null;
                return false;
            } else
            {
                return true;
            }
        }
    }
    public class DisableMosaic : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DisableMosaic()
        {
            switch_def = TrackBindConfig("OtherFunctions", "DisableMosaic", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(MosaicShower), "FnDrawMosaic")]
        private static bool PatchContent(ref bool __result)
        {
            if (switch_def.Value)
            {
                __result = false;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
