using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using nel;

namespace AliceInCradleCheat
{
    // ##############################
    // Set game values
    // ##############################
    public class SetGameValue : BasePatchClass
    {
        private static NelM2DBase m2d;
        private static PRNoel noel;
        private static TimedFlag set_money_button;
        private static ConfigEntry<int> money_def;
        private static TimedFlag set_dangerlevel_button;
        private static ConfigEntry<int> danger_level_def;
        private static TimedFlag set_weather_button;
        private static ConfigEntry<string> weather_def;
        private static TimedFlag reset_MP_break;
        private static TimedFlag reset_H_exp;
        private static TimedFlag plant_eggs;
        private static TimedFlag lay_eggs;
        public SetGameValue()
        {
            string section = "SetGameValues";
            set_money_button = new(TrackBindConfig(section, "SetMoneyButton", false));
            money_def = TrackBindConfig(section, "Money", 99999,
                new AcceptableValueRange<int>(0, 99999));
            set_dangerlevel_button = new(TrackBindConfig(section, "SetDangerLevelButton", false));
            danger_level_def = TrackBindConfig(section, "DangerLevel", 0,
                new AcceptableValueRange<int>(0, 160));
            set_weather_button = new(TrackBindConfig(section, "SetWeatherButton", false));
            weather_def = TrackBindConfig(section, "Weather", "0110111");
            reset_MP_break = new(TrackBindConfig("BasicStatus", "ResetMPBreak", false));
            reset_H_exp = new(TrackBindConfig("NonHModeEnhance", "ResetHExp", false));
            plant_eggs = new(TrackBindConfig("PervertFunctions", "PlantEggs", false));
            lay_eggs = new(TrackBindConfig("PervertFunctions", "LayEggs", false));
            TryPatch(GetType());
        }
        [HarmonyPostfix, HarmonyPatch(typeof(SceneGame), "Update")]
        private static void PatchContent()
        {
            m2d = MainReference.GetM2D();
            noel = MainReference.GetNoel();
            if (m2d == null || noel == null) { return; }
            if (set_money_button.Check()) { SetMoney(); }
            if (set_dangerlevel_button.Check()) { SetDangerLevel(); }
            if (set_weather_button.Check()) { SetWeather(); }
            if (reset_MP_break.Check()) {
                noel.GageBrk.reset();
                UIStatus.Instance.quitCrack();
            }
            if (reset_H_exp.Check()) { ResetHExp(); }
            if (plant_eggs.Check()) { PlantEggs(); }
            if (lay_eggs.Check()) { noel.EggCon.forcePushout(false); }
        }
        private static void SetMoney()
        {
            int diff_money = (int)(money_def.Value - CoinStorage.count);
            if (diff_money >= 0)
            {
                CoinStorage.addCount(diff_money);
            }
            else
            {
                CoinStorage.reduceCount(-diff_money);
            }
        }
        private static void SetDangerLevel()
        {
            NightController nc = m2d.NightCon;
            if (nc.M2D.isSafeArea()) { return; }
            int dlevel = Traverse.Create(nc).Field("dlevel").GetValue<int>();
            int dlevel_add = Traverse.Create(nc).Field("dlevel_add").GetValue<int>();
            int new_dlevel = danger_level_def.Value;
            int diff_level = new_dlevel - dlevel - dlevel_add;
            if (diff_level == 0) { return; }
            if (diff_level > 0)
            {
                Traverse.Create(nc).Field("dlevel").SetValue(new_dlevel);
            } else if (diff_level < 0)
            {
                // Reduce additional danger level first
                if (new_dlevel >= dlevel)
                {
                    m2d.NightCon.addAdditionalDangerLevel(diff_level, false);
                }
                else
                {
                    m2d.NightCon.addAdditionalDangerLevel(-dlevel_add, false);
                    Traverse.Create(nc).Field("dlevel").SetValue(new_dlevel);
                }
            }
            nc.showNightLevelAdditionUI();
        }
        private static void SetWeather()
        {
            NightController nc = m2d.NightCon;
            string weather_string = weather_def.Value;
            WeatherItem.WEATHER[] available_weather = (WeatherItem.WEATHER[])Enum.GetValues(typeof(WeatherItem.WEATHER));
            List<WeatherItem.WEATHER> add_weather_list = new();
            for (int i = 0; i < weather_string.Length && i < available_weather.Length - 1; i++)
            {
                if (int.TryParse(weather_string[i].ToString(), out int num))
                {
                    for (int j = 0; j < num; j++)
                    {
                        add_weather_list.Add(available_weather[i]);
                    }
                }
            }
            // destruction of old weathers
            WeatherItem[] AWeather = Traverse.Create(nc).Field("AWeather").GetValue<WeatherItem[]>();
            foreach (WeatherItem weather in AWeather)
            {
                weather.destruct();
            }
            // add new weathers
            List<WeatherItem> AList = new();
            foreach (WeatherItem.WEATHER i in add_weather_list)
            {
                if (i == WeatherItem.WEATHER.MIST && add_weather_list.Contains(WeatherItem.WEATHER.MIST_DENSE))
                {
                    continue;
                }
                if (i == WeatherItem.WEATHER.NORMAL && add_weather_list.Count > 1)
                {
                    continue;
                }
                AList.Add(new WeatherItem(i, Traverse.Create(nc).Field("dlevel").GetValue<int>()));
            }
            int cur_weather = Traverse.Create(nc).Field("cur_weather_").GetValue<int>();
            for (int j = AList.Count - 1; j >= 0; j--)
            {
                cur_weather |= 1 << (int)AList[j].weather;
            }
            Traverse.Create(nc).Field("cur_weather_").SetValue(cur_weather);
            Traverse.Create(nc).Field("AWeather").SetValue(AList.ToArray());
        }
        private static void ResetHExp()
        {
            noel.Ser.clear();
            noel.EpCon.newGame();
            noel.EggCon.clear();
            noel.GageBrk.reset();
            UIStatus.Instance.fineMpRatio(true, false);
            UIStatus.Instance.quitCrack();
            noel.recheck_emot = true;
        }
        private static void PlantEggs()
        {
            PrEggManager EggCon = noel.EggCon;
            int type_num = Enum.GetNames(typeof(PrEggManager.CATEG)).Length;
            int val = (int)(noel.get_maxmp() / type_num / 2) * 2;
            Traverse.Create(noel).Field("mp").SetValue((int)(noel.get_maxmp() - (val * (type_num - 1))));
            int len = Traverse.Create(EggCon).Field("LEN").GetValue<int>();
            bool any_flag = len > 0;
            foreach (PrEggManager.CATEG categ in Enum.GetValues(typeof(PrEggManager.CATEG)))
            {
                if (categ == PrEggManager.CATEG._ALL)
                {
                    continue;
                }
                PrEggManager.PrEggItem prEggItem = EggCon.Get(categ);
                if (prEggItem == null)
                {
                    PrEggManager.PrEggItem[] aitm = Traverse.Create(EggCon).Field("AItm").GetValue<PrEggManager.PrEggItem[]>();
                    prEggItem = aitm[len] = new PrEggManager.PrEggItem(EggCon, categ);
                    len += 1;
                }
                prEggItem.val = val;
                prEggItem.add_cushion = 0;
                prEggItem.t_cushion = 0;
                prEggItem.val_absorbed = 0;
                prEggItem.t_effect_egg = -1;
                prEggItem.effect_egg_count = 0;
                prEggItem.effect_egg_count_layed = 0;
            }
            Traverse.Create(EggCon).Field("LEN").SetValue(len);
            Traverse.Create(EggCon).Field("total_").SetValue(-1);
            Traverse.Create(EggCon).Field("status_reduce_max_").SetValue(-1);
            if (!any_flag)
            {
                EggCon.need_fine_mp = true;
                if (!Traverse.Create(EggCon).Field("lock_fine").GetValue<bool>() && UIStatus.PrIs(EggCon.Pr))
                {
                    UIStatus.Instance.finePlantedEgg();
                }
                noel.NM2D.IMNG.fineSpecialNoelRow(EggCon.Pr);
            }
            if (noel.UP != null && noel.UP.isActive())
            {
                UIStatus.Instance.fineMpRatio(true, false);
            }
            noel.recheck_emot = true;
        }
    }
}
