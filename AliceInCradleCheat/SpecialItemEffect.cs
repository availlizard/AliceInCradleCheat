using BepInEx.Configuration;
using HarmonyLib;
using nel;
using m2d;
using System.Reflection.Emit;

namespace AliceInCradleCheat
{
    // ##############################
    // Restriction Lift
    // ##############################
    public class SpecialItemEffect : BasePatchClass
    {
        private static ConfigEntry<bool> ep_item_effect_def;
        public SpecialItemEffect()
        {
            ep_item_effect_def = TrackBindConfig("PervertFunctions", "EpItemEffect", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(NelItem), "Use")]
        private static void PatchContent(ref NelItem __instance, ref PR Pr)
        {
            if (ep_item_effect_def.Value)
            {
                if (__instance.key == "fruit_epdmg_apple0" && Pr.ep >= 700)
                {
                    Pr.Ser.Add(SER.FRUSTRATED);
                }
            }
            return;
        }
    }
}
