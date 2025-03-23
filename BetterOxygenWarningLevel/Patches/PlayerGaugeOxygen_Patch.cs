using HarmonyLib;
using SpaceCraft;
using System;

namespace BetterOxygenWarningLevel.Patches
{
    [HarmonyPatch(typeof(PlayerGaugeOxygen))]
    [HarmonyPatch(nameof(PlayerGaugeOxygen.GaugeVerifications))]
    public static class PlayerGaugeOxygen_GaugeVerifications_Patch
    {
        public static bool firstrundone;
        public static bool halftimealreadytiggered;
        public static bool SixtySecondWarningtriggered;

        [HarmonyPrefix]
        static bool Prefix(PlayerGaugeOxygen __instance) 
        {
            if(!Plugin.ModisActive.Value) return true;
            //---------------------------------------------------------------
            if (!firstrundone)
            { 
                firstrundone = true;
                return true;
            }

            //Plugin.MyDebugLogger("We are Patching Oxygen Warnings");

            //Todo - Make Check that Values are not lower then the next level
            //Check 1 Low Level can not be lower than Critical
            if (Plugin.LowWarning_Limit.Value <= Plugin.CriticalWarning_Limit.Value)
            {
                Plugin.LowWarning_Limit.Value = Plugin.CriticalWarning_Limit.Value + 5;
            }

            try
            {
                GaugeVerifications_Patched(__instance);
            }
            catch(Exception ex)
            {
                Plugin.Logger.LogError($"Error detected Fallback on Nativ Game Code. Error -> {ex.Message}");
                return true;
            }
           
            //---------------------------------------------------------------
            return false;
        }

        static void GaugeVerifications_Patched(PlayerGaugeOxygen __instance)
        {
            if (!__instance.isInited)
            {
                return;
            }

            if (__instance.gaugeValue <= 20f)
            {
                __instance.playerAudio.SetIsPanting(true);
            }
            else
            {
                __instance.playerAudio.SetIsPanting(false);
            }

            try //to prevent issues and Break the Game. Let the Player at least play. Even its not that good in terms of Unity Error Handling...
            {
                if (Plugin.HalftimeWarning_Enable.Value)
                {
                    //Plugin.MyDebugLogger("Halftime - Enabled : True");
                    if (__instance.maxValue > Plugin.HalftimeWarning_onlywhenMaxTankCapacyisover_Limit.Value && __instance.gaugeValue <= (__instance.maxValue / 2) )
                    {
                        //Plugin.MyDebugLogger($"Halftime - Innerhalf if true");
                        if (!halftimealreadytiggered)
                        {
                            //Plugin.MyDebugLogger($"Halftime - Innerhalf triggered");
                            //Managers.GetManager<BaseHudHandler>().DisplayCursorText("UI_WARN_OXYGEN_HALF", 3f, "Oxygen half Capacity");
                            Managers.GetManager<BaseHudHandler>().DisplayCursorText("", 3f, "Oxygen half Capacity");
                            //Managers.GetManager<BaseHudHandler>().DisplayInfoText
                            if (Plugin.HalftimeWarning_withSignal.Value)
                            {
                                //Plugin.MyDebugLogger($"Halftime - Innerhalf Sound");
                                __instance.globalAudioHandler.PlayAlertLow();
                            }
                            else
                            {
                                //Plugin.MyDebugLogger($"Halftime - Innerhalf without Sound");
                            }
                            halftimealreadytiggered = true;
                        }
                        else
                        {
                            //Plugin.MyDebugLogger($"Halftime - Innerhalf NOT triggered");
                        }
                    }
                    else
                    {
                        //Plugin.MyDebugLogger($"Halftime - Innerhalf if false -> reset");
                        halftimealreadytiggered = false;
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError("Error on running Halftime Oxygen Trigger");
                Plugin.Logger.LogError(e.Message);
            }
            
            try //to prevent issues and Break the Game. Let the Player at least play. Even its not that good in terms of Unity Error Handling...
            {
                if (Plugin.SixtySecWarning_Enable.Value)
                {
                    //Plugin.MyDebugLogger("Sixty - Enabled : True");
                    if (__instance.maxValue > Plugin.SixtySecWarningonlywhenMaxTankCapacyisover_Limit.Value && __instance.gaugeValue <= Plugin.SixtySecWarning_Limit.Value)
                    {
                        //Plugin.MyDebugLogger("Sixty - Sixyty Limit reached : True");
                        if (!SixtySecondWarningtriggered)
                        {
                            //Plugin.MyDebugLogger("Sixty - Sixyty Warning triggered : True");
                            //Managers.GetManager<BaseHudHandler>().DisplayCursorText("UI_WARN_OXYGEN_SIXTYSEC", 3f, "Oxygen Sixty Seconds");
                            Managers.GetManager<BaseHudHandler>().DisplayCursorText("", 3f, "Oxygen Sixty Seconds");
                            if (Plugin.SixtySecWarning_withSignal.Value)
                            {
                                //Plugin.MyDebugLogger("Sixty - With Sounds : True");
                                __instance.globalAudioHandler.PlayAlertLow();
                            }
                            else
                            {
                                //Plugin.MyDebugLogger("Sixty - With Sounds : False");
                            }
                            SixtySecondWarningtriggered = true;
                        }
                        else
                        {
                            //Plugin.MyDebugLogger("Sixty - Sixyty Warning triggered : false");
                        }
                    }
                    else
                    {
                        //Plugin.MyDebugLogger("Sixty - Sixyty Limit reached : false -> reset");
                        SixtySecondWarningtriggered = false;
                    }
                }
                else
                {
                    //Plugin.MyDebugLogger("Sixty - Enabled : false");
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError("Error on running Sixty Seconds Oxygen Trigger");
                Plugin.Logger.LogError(e.Message); 
            }


            if (__instance.gaugeValue > Plugin.LowWarning_Limit.Value)
            {
                __instance.hasAlertedLow = false;
                __instance.hasAlertedCritical = false;
                return;
            }

            if(Plugin.LowWarning_Enable.Value) 
            {
                if (__instance.gaugeValue <= Plugin.LowWarning_Limit.Value && !__instance.hasAlertedLow)
                {
                    Managers.GetManager<BaseHudHandler>().DisplayCursorText("UI_WARN_OXYGEN_LOW", 3f, "");
                    if(Plugin.LowWarning_withSignal.Value)
                    {
                        if(Plugin.LowWarning_SignalSoundChangetoCritical.Value)
                        {
                            __instance.globalAudioHandler.PlayAlertCritical();
                        }
                        else 
                        {
                            __instance.globalAudioHandler.PlayAlertLow();
                        }
                    }
                    __instance.hasAlertedLow = true;
                    return;
                }
            }

            if(Plugin.CritiaclWarning_Enable.Value)
            {
                if (__instance.gaugeValue <= Plugin.CriticalWarning_Limit.Value && !__instance.hasAlertedCritical)
                {
                    Managers.GetManager<BaseHudHandler>().DisplayCursorText("UI_WARN_OXYGEN_CRITICAL", 3f, "");
                    if(Plugin.CritiaclWarning_withSignal.Value)
                    {
                        __instance.globalAudioHandler.PlayAlertCritical();
                    }
                    __instance.hasAlertedCritical = true;
                    return;
                }
            }

            if (__instance.gaugeValue <= 0f)
            {
                __instance.playerAudio.PlayDie();
            }

        }
    }
}
