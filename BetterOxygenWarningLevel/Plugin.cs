using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;
using BetterOxygenWarningLevel.Patches;

namespace BetterOxygenWarningLevel
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        public const string GUID = "Desperationfighter.TPC.BetterOxygenWarningLevel";
        public const string Name = "Better Oxygen Warning Level";
        public const string Version = "1.0.1.0"; //Remmber to Update Assembly Version too !

        public static float MaxValue;

        public static ConfigEntry<bool> ModisActive;
        public static ConfigEntry<bool> Debuglogging;

        public static ConfigEntry<bool> CritiaclWarning_Enable;
        public static ConfigEntry<bool> CritiaclWarning_withSignal;
        public static ConfigEntry<Single> CriticalWarning_Limit;

        public static ConfigEntry<bool> LowWarning_Enable;
        public static ConfigEntry<bool> LowWarning_withSignal;
        public static ConfigEntry<bool> LowWarning_SignalSoundChangetoCritical;
        public static ConfigEntry<Single> LowWarning_Limit;

        public static ConfigEntry<bool> SixtySecWarning_Enable;
        public static ConfigEntry<bool> SixtySecWarning_withSignal;
        public static ConfigEntry<Single> SixtySecWarning_Limit;
        public static ConfigEntry<Single> SixtySecWarningonlywhenMaxTankCapacyisover_Limit;

        public static ConfigEntry<bool> HalftimeWarning_Enable;       
        public static ConfigEntry<bool> HalftimeWarning_withSignal;
        public static ConfigEntry<Single> HalftimeWarning_onlywhenMaxTankCapacyisover_Limit;

        private void Awake()
        {
            ModisActive = Config.Bind("1_General", "ModisActive", true, "Set if the Mod should running or not. If you don't want to remove Files or for Later Ingame Menu. Please reload your Savegame after Change as there are Setting that only apply once when World is loaded up.");
            Debuglogging = Config.Bind("9_Advanced", "Debuglogging", false, "Enables Debug Logging. Should be only activated when you know what you do.");

            CritiaclWarning_Enable = Config.Bind("2_CriticalWarning", "CritiaclWarning_Enable", true, "");
            CriticalWarning_Limit = Config.Bind("2_CriticalWarning", "CriticalWarning_Limit", 18f, "Game Default = 16f");
            CritiaclWarning_withSignal = Config.Bind("2_CriticalWarning", "CritiaclWarning_withSignal", true, "");

            LowWarning_Enable = Config.Bind("3_LowWarning", "CritiaclWarning_Enable", true, "");
            LowWarning_Limit = Config.Bind("3_LowWarning", "LowWarning_Limit", 35f, "Game Default = 30f");
            LowWarning_withSignal = Config.Bind("3_LowWarning", "LowWarning_withSignal", true, "");
            LowWarning_SignalSoundChangetoCritical = Config.Bind("3_LowWarning", "LowWarning_SignalSoundChangetoCritical", true, "");

            SixtySecWarning_Enable = Config.Bind("4_SixtySeconds", "SixtySecWarning_Enable", true, "");
            SixtySecWarning_Limit = Config.Bind("4_SixtySeconds", "SixtySecWarning_Limit", 60f, "");
            SixtySecWarning_withSignal = Config.Bind("4_SixtySeconds", "SixtySecWarning_withSignal", true, "");
            SixtySecWarningonlywhenMaxTankCapacyisover_Limit = Config.Bind("4_SixtySeconds", "SixtySecWarningonlywhenMaxTankCapacyisover_Limit", 200f, "");

            HalftimeWarning_Enable = Config.Bind("5_HalftimeWarning", "HalftimeWarning_Enable", true, "");
            HalftimeWarning_withSignal = Config.Bind("5_HalftimeWarning", "HalftimeWarning_withSignal", false, "");
            HalftimeWarning_onlywhenMaxTankCapacyisover_Limit = Config.Bind("5_HalftimeWarning", "HalftimeWarning_onlywhenMaxTankCapacyisover_Limit", 150f, "");


            // set project-scoped logger instance
            Logger = base.Logger;

            List<float> list = new List<float>();
            list.Add(Plugin.CriticalWarning_Limit.Value);
            list.Add(Plugin.LowWarning_Limit.Value);
            list.Add(Plugin.SixtySecWarning_Limit.Value);
            MaxValue = list.Max();

            PlayerGaugeOxygen_GaugeVerifications_Patch.halftimealreadytiggered = false;
            PlayerGaugeOxygen_GaugeVerifications_Patch.SixtySecondWarningtriggered = false;
            PlayerGaugeOxygen_GaugeVerifications_Patch.firstrundone = false;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        public static void MyDebugLogger(string message)
        {
            if (Debuglogging.Value)
            {
                Logger.LogDebug($"[{Name}][Debug] : {message} [/Debug]");
            }
        }
    }
}
