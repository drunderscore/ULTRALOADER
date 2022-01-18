using HarmonyLib;

namespace ULTRALOADER.Patches
{
    [HarmonyPatch(typeof(CheatsManager), "RenderCheatsInfo")]
    public class CheatsManager_RenderCheatsInfo
    {
        private static void Postfix()
        {
            var text = CheatsController.Instance.cheatsInfo.text;
            UltraLoaderEvents.OnCheatsManagerRenderInfo(ref text);
            CheatsController.Instance.cheatsInfo.text = text;
        }
    }
}