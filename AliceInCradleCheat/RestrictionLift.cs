using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using XX;
using nel;

namespace AliceInCradleCheat
{
    // ##############################
    // Restriction Lift
    // ##############################
    public class RestrictionLift
    {
        public RestrictionLift ()
        {
            _ = new EnableInfiniteJump();
            _ = new EnableFastTravel();
            _ = new EnableStorageAccess();
            _ = new EnableItemUsage();
        }
    }
    public class EnableInfiniteJump : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public EnableInfiniteJump()
        {
            switch_def = TrackBindConfig("RestrictionLift", "InfiniteJump", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "runPhysics")]
        static bool PatchContent(ref PR __instance)
        {
            if (switch_def.Value)
            {
                PR noel = __instance;
                float jump_pushing = Traverse.Create(noel).Field("jump_pushing").GetValue<float>();
                if (jump_pushing >= -1 && jump_pushing < (20000 - noel.TS) && noel.isJumpO(0))
                {
                    if (jump_pushing < 10)
                    {
                        jump_pushing = 20000;
                    }
                    else
                    {
                        jump_pushing += noel.TS;
                    }
                    Traverse.Create(noel).Field("jump_pushing").SetValue(jump_pushing);
                }
            }
            return true;
        }
    }
    public class EnableFastTravel : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public EnableFastTravel()
        {
            switch_def = TrackBindConfig("RestrictionLift", "FastTravel", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(NelM2DBase), "cantFastTravel")]
        static bool BenchTravelPatch(ref string __result)
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
        //
        // Enable switch between map edit and travel
        [HarmonyPrefix, HarmonyPatch(typeof(UiGameMenu), "runEditMap")]
        static bool MapTravelPatch(ref UiGameMenu __instance)
        {
            if (switch_def.Value)
            {
                Traverse.Create(__instance).Field("can_use_fasttravel").SetValue(true);
            }
            return true;
        }
        // Enable travel, without check box ui (runEditMap use BxCmd.addButtonMultiT)
        [HarmonyPostfix, HarmonyPatch(typeof(UiGameMenu), "runEditMap")]
        static void MapTravelPatch2(ref UiGameMenu __instance)
        {
            if (switch_def.Value)
            {
                ButtonSkinWholeMapArea WmSkin = Traverse.Create(__instance).Field("WmSkin").GetValue<ButtonSkinWholeMapArea>();
                if (!IN.isRunO(0) && IN.kettei() && Traverse.Create(__instance).Field("fasttravel").GetValue<bool>() &&
                    WmSkin.FastTravelFocused != null)
                {
                    NelChipBench BenchChip = Traverse.Create(__instance).Field("BenchChip").GetValue<NelChipBench>();
                    if (BenchChip == null || !BenchChip.IconIs(WmSkin.FastTravelFocused.get_Icon()))
                    {
                        UiBenchMenu.ExecuteFastTravel(WmSkin.FastTravelFocused, null, null, null);
                    }
                }
            }
        }
        // Set default map mode to travel
        [HarmonyPostfix, HarmonyPatch(typeof(UiGameMenu), "activateMap")]
        static void MapTravelPatch3(ref UiGameMenu __instance)
        {
            if (switch_def.Value)
            {
                Traverse.Create(__instance).Field("fasttravel").SetValue(true);
                Traverse.Create(__instance).Field("WmSkin").GetValue<ButtonSkinWholeMapArea>().fast_travel_active = true;
            }
        }
        // */
    }
    public class EnableStorageAccess : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public EnableStorageAccess()
        {
            switch_def = TrackBindConfig("RestrictionLift", "StorageAccess", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(NelM2DBase), "canAccesableToHouseInventory")]
        static bool PatchContent(ref bool __result)
        {
            if (switch_def.Value)
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public class EnableItemUsage : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public EnableItemUsage()
        {
            switch_def = TrackBindConfig("RestrictionLift", "ItemUsage", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "canUseItem")]
        static bool PatchContent(ref string __result)
        {
            if (switch_def.Value)
            {
                __result = "";
                return false;
            }
            return true;
        }
    }
}
