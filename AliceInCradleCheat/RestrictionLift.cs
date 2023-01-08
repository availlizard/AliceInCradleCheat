using BepInEx.Configuration;
using HarmonyLib;
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
            _ = new EnableFastTravel();
            _ = new EnableStorageAccess();
            _ = new EnableItemUsage();
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
        private static bool BenchTravelPatch(ref string __result)
        {
            if (switch_def.Value)
            {
                __result = null;
                return false;
            }
            return true;
        }
        //
        // Enable switch between map edit and travel
        [HarmonyPrefix, HarmonyPatch(typeof(UiGameMenu), "runEditMap")]
        private static void MapTravelPatch(ref UiGameMenu __instance)
        {
            if (!switch_def.Value) { return; }
            Traverse.Create(__instance).Field("can_use_fasttravel").SetValue(true);
        }
        // Enable travel, without check box ui (runEditMap use BxCmd.addButtonMultiT)
        [HarmonyPostfix, HarmonyPatch(typeof(UiGameMenu), "runEditMap")]
        private static void MapTravelPatch2(ref UiGameMenu __instance)
        {
            if (!switch_def.Value) { return; }
            ButtonSkinWholeMapArea WmSkin = Traverse.Create(__instance).Field("WmSkin").GetValue<ButtonSkinWholeMapArea>();
            if (IN.isRunO(0) || ! IN.kettei() || ! Traverse.Create(__instance).Field("fasttravel").GetValue<bool>() ||
                WmSkin.FastTravelFocused == null)
            {
                return;
            }
            NelChipBench BenchChip = Traverse.Create(__instance).Field("BenchChip").GetValue<NelChipBench>();
            if (BenchChip == null || !BenchChip.IconIs(WmSkin.FastTravelFocused.get_Icon()))
            {
                UiBenchMenu.ExecuteFastTravel(WmSkin.FastTravelFocused, null, null, null);
            }
        }
        // Set default map mode to travel
        [HarmonyPostfix, HarmonyPatch(typeof(UiGameMenu), "activateMap")]
        private static void MapTravelPatch3(ref UiGameMenu __instance)
        {
            if (!switch_def.Value) { return; }
            Traverse.Create(__instance).Field("fasttravel").SetValue(true);
            Traverse.Create(__instance).Field("WmSkin").GetValue<ButtonSkinWholeMapArea>().fast_travel_active = true;
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
        private static bool PatchContent(ref bool __result)
        {
            if (switch_def.Value)
            {
                __result = true;
                return false;
            }
            return true;
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
        private static bool PatchContent(ref string __result)
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
