using BepInEx.Configuration;
using HarmonyLib;
using nel;
using m2d;

namespace AliceInCradleCheat
{
    // ##############################
    // Immune to environment damage
    // ##############################
    public class EnvironmentDamage
    {
        public EnvironmentDamage()
        {
            _ = new DisableGasDamage();
            _ = new ImmuneToMapThorn();
            _ = new ImmuneToLava();
        }
    }
    public class DisableGasDamage : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DisableGasDamage()
        {
            switch_def = TrackBindConfig("EnvironmentDamage", "DisableGasDamage", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "canApplyGasDamage")]
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
    public class ImmuneToMapThorn : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public ImmuneToMapThorn()
        {
            switch_def = TrackBindConfig("EnvironmentDamage", "ImmuneToMapThorn", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "applyDamageFromMap")]
        private static bool PatchContent(ref AttackInfo __result)
        {
            if (switch_def.Value)
            {
                __result = null;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public class ImmuneToLava : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public ImmuneToLava()
        {
            switch_def = TrackBindConfig("EnvironmentDamage", "ImmuneToLava", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "checkLavaExecute")]
        private static bool PatchContent()
        {
            if (switch_def.Value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
