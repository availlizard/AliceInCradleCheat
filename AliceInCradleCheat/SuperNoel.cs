using System;
using BepInEx.Configuration;
using HarmonyLib;
using m2d;
using nel;


namespace AliceInCradleCheat
{
    // ##############################
    // Super Noel
    // ##############################
    public class SuperNoel
    {
        public SuperNoel()
        {
            _ = new DamageMultiplier();
            _ = new Invincible();
            _ = new InfiniteJump();
            _ = new InfiniteBomb();
            _ = new DuralableShield();
            _ = new DisableGasDamage();
            _ = new ImmuneToMapThorn();
            _ = new ImmuneToLava();
            //_ = new TestFunction();
        }
    }
    public class DamageMultiplier : BasePatchClass
    {
        private static ConfigEntry<int> hp_dmg_def;
        private static ConfigEntry<int> mp_dmg_def;
        public DamageMultiplier()
        {
            string section = "SuperNeol";
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
    public class Invincible : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public Invincible()
        {
            switch_def = TrackBindConfig("SuperNeol", "InvincibleToMonsters", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "applyDamage", new Type[] { typeof(NelAttackInfo), typeof(bool) })]
        private static bool PatchContent(ref int __result)
        {
            if (switch_def.Value)
            {
                __result = 0;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public class InfiniteJump : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public InfiniteJump()
        {
            switch_def = TrackBindConfig("SuperNeol", "InfiniteJump", false);
            TryPatch(GetType());
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PR), "runPhysics")]
        private static void PatchContent(ref PR __instance)
        {
            if (!switch_def.Value) { return; }
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
            return;
        }
    }
    public class InfiniteBomb : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public InfiniteBomb()
        {
            switch_def = TrackBindConfig("SuperNeol", "InfiniteGroundBomb", false);
            TryPatch(GetType());
        }
        [HarmonyPostfix, HarmonyPatch(typeof(M2PrSkill), "explodeMagic")]
        private static void PatchContent(ref M2PrSkill __instance)
        {
            if (!switch_def.Value) { return; }
            MGContainer mgc = __instance.NM2D.MGC;
            MagicItem cur_mg = Traverse.Create(__instance).Field("CurMg").GetValue<MagicItem>();
            int bomb_count = 0;
            for (int i = mgc.Length - 1; i >= 0; i--)
            {
                MagicItem mg = mgc.getMg(i);
                if (mg != cur_mg && mg.isActive(__instance.Pr, MGKIND.DROPBOMB) && ++bomb_count > 4 && mg.phase == 9)
                {
                    mg.phase = 3;
                }
            }
        }
    }
    public class DuralableShield : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DuralableShield()
        {
            switch_def = TrackBindConfig("SuperNeol", "DuralableShield", false);
            TryPatch(GetType());
        }
        // durability change with time going by
        [HarmonyPrefix, HarmonyPatch(typeof(M2Shield), "run")]
        private static void PatchContent(ref float power_progress_level)
        {
            if (!switch_def.Value) { return; }
            power_progress_level = 0;
        }
        // durability change while being attacked
        [HarmonyPrefix, HarmonyPatch(typeof(M2Shield), "checkShield")]
        private static void PatchContent2(ref float val)
        {
            if (!switch_def.Value) { return; }
            val = 0;
        }
    }
    public class DisableGasDamage : BasePatchClass
    {
        private static ConfigEntry<bool> switch_def;
        public DisableGasDamage()
        {
            switch_def = TrackBindConfig("SuperNeol", "DisableGasDamage", false);
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
            switch_def = TrackBindConfig("SuperNeol", "ImmuneToMapThorn", false);
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
            switch_def = TrackBindConfig("SuperNeol", "ImmuneToLava", false);
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
    //---------------------------------------------------
    // /*
    public class TestFunction : BasePatchClass
    {
        public TestFunction()
        {
            try
            {
                Harmony.CreateAndPatchAll(GetType());
            }
            catch //(Exception ex)
            {
                AICCheat.cheat_logger.LogError($"Patch {GetType()} failed!");
            }
        }
        [HarmonyPrefix, HarmonyPatch(typeof(M2PrSkill), "explodeMagic")]
        private static void PatchContent(ref M2PrSkill __instance)
        {
            string log = "";
            MGContainer mgc = __instance.NM2D.MGC;
            //MagicItem cur_mg = Traverse.Create(__instance).Field("CurMg").GetValue<MagicItem>();
            //if (cur_mg == null) { return; }
            for (int i = mgc.Length - 1; i >= 0; i--)
            {
                MagicItem mg = mgc.getMg(i);
                log += $"{mg.kind}: {mg.phase}\t";
            }
            AICCheat.cheat_logger.LogInfo(log);
            AICCheat.cheat_logger.LogInfo($"Pre------------------");
        }
        [HarmonyPostfix, HarmonyPatch(typeof(M2PrSkill), "explodeMagic")]
        private static void PatchContent2(ref M2PrSkill __instance)
        {
            string log = "";
            MGContainer mgc = __instance.NM2D.MGC;
            //MagicItem cur_mg = Traverse.Create(__instance).Field("CurMg").GetValue<MagicItem>();
            //if (cur_mg == null) { return; }
            for (int i = mgc.Length - 1; i >= 0; i--)
            {
                MagicItem mg = mgc.getMg(i);
                log += $"{mg.kind}: {mg.phase}\t";
            }
            AICCheat.cheat_logger.LogInfo(log);
            AICCheat.cheat_logger.LogInfo($"After------------------");
        }
    }
    // */
}
