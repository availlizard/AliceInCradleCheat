using BepInEx.Configuration;
using HarmonyLib;
using nel;

namespace AliceInCradleCheat
{
    // ##############################
    // Non H mode enhance
    // ##############################
    public class NonHModeEnhance
    {
        public NonHModeEnhance()
        {
            _ = new DisableGrabAttack();
            _ = new DisableEpDamage();
            _ = new SkipGameOverPlay();
            _ = new DisableWormTrap();
        }
    }
    public class DisableGrabAttack : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DisableGrabAttack()
        {
            switch_def = TrackBindConfig("NonHModeEnhance", "DisableGrabAttack", false);
            TryPatch(GetType());
        }
        /* Some enemies' absorb attack will cause little to no damage,
        patch readTicket to change their behavior, others rely on these
        attacks to move, disable initAbsorb instead        */
        [HarmonyPrefix, HarmonyPatch(typeof(NelNGolem), "readTicketOd")]
        private static bool DisableGolem(ref NaTicket Tk)
        {
            if (switch_def.Value && Tk.type == NAI.TYPE.PUNCH_1)
            {
                Tk.type = NAI.TYPE.PUNCH;
            }
            return true;
        }
        /* Disable runAsborb will also diable HP damage, make game less challengable
        [HarmonyPrefix, HarmonyPatch(typeof(NelNFox), "runAbsorb"),
            HarmonyPatch(typeof(NelNGolem), "runAbsorb"), HarmonyPatch(typeof(NelNMush), "runAbsorb"),
            HarmonyPatch(typeof(NelNPuppy), "runAbsorb"), HarmonyPatch(typeof(NelNSlime), "runAbsorb"),
            HarmonyPatch(typeof(NelNSlime), "runAbsorbOverDrive"),
            HarmonyPatch(typeof(NelNSnake), "runAbsorb"), HarmonyPatch(typeof(NelNUni), "runAbsorb")]
        //*/
        /* Disable individual absorb action
        [HarmonyPrefix, HarmonyPatch(typeof(NelNFox), "initAbsorb"),
            HarmonyPatch(typeof(NelNGolem), "initAbsorb"), HarmonyPatch(typeof(NelNMush), "initAbsorb"),
            HarmonyPatch(typeof(NelNPuppy), "initAbsorb"), HarmonyPatch(typeof(NelNSlime), "initAbsorb"),
            HarmonyPatch(typeof(NelNSnake), "initAbsorb"), HarmonyPatch(typeof(NelNUni), "initAbsorb")]
        public static bool DisableAbsorbInit(ref bool __result)
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
        //*/
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "initAbsorb")]
        private static bool DisableAbsorbInit(ref bool __result, ref AbsorbManager Abm)
        {
            if (switch_def.Value)
            {
                __result = false;
                Abm?.destruct();
                return false;
            }
            else
            {
                return true;
            }
        }

    }
    public class DisableEpDamage : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DisableEpDamage()
        {
            switch_def = TrackBindConfig("NonHModeEnhance", "DisableEpDamage", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(EpManager), "applyEpDamage")]
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
    public class SkipGameOverPlay : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public SkipGameOverPlay()
        {
            switch_def = TrackBindConfig("NonHModeEnhance", "SkipGameOverPlay", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UiGO), "runGiveup")]
        private static bool PatchContent(ref UiGO __instance)
        {
            if (switch_def.Value)
            {
                Traverse.Create(__instance).Field("t").SetValue(90);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public class DisableWormTrap : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DisableWormTrap()
        {
            switch_def = TrackBindConfig("NonHModeEnhance", "DisableWormTrap", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "canPullByWorm")]
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
        [HarmonyPrefix, HarmonyPatch(typeof(NelChipWormHead), "initAction")]
        private static bool DisableWormAreaDrawer2()
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
        // */
    }
}
