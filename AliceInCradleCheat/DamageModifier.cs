using System;
using BepInEx.Configuration;
using HarmonyLib;
using nel;
using m2d;


namespace AliceInCradleCheat
{
    // ##############################
    // Damage enhancement
    // ##############################
    public class DamageMultiplier : BasePatchClass
    {
        private static ConfigEntry<int> hp_dmg_def;
        private static ConfigEntry<int> mp_dmg_def;
        public DamageMultiplier()
        {
            string section = "DamageEnhance";
            hp_dmg_def = TrackBindConfig(section, "HPDamageMultiplier", 1,
                new AcceptableValueRange<int>(1, 200));
            mp_dmg_def = TrackBindConfig(section, "MPDamageMultiplier", 1,
                new AcceptableValueRange<int>(1, 200));
            TryPatch(GetType());
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(M2PrSkill), "AtkMul")]
        private static bool PatchContent(ref float hpdmg, ref float mpdmg)
        {
            hpdmg *= hp_dmg_def.Value;
            mpdmg *= mp_dmg_def.Value;
            return true;
        }
    }
}
