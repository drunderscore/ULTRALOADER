using HarmonyLib;
using TweakIt.Cheats;

namespace TweakIt.Patches
{
    [HarmonyPatch(typeof(NewMovement), "GetHurt")]
    public class NewMovement_GetHurt
    {
        private static bool Prefix()
        {
            return !Invincibility.Instance.IsActive;
        }
    }
}