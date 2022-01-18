using HarmonyLib;

namespace ULTRALOADER.Patches
{
    [HarmonyPatch(typeof(CheatsManager), "Start")]
    public class CheatsManager_Start
    {
        private static void Prefix(CheatsManager __instance)
        {
            UltraLoaderEvents.OnCheatsManagerStart(__instance);
        }
    }
}