using HarmonyLib;
using TweakIt.Cheats;
using ULTRALOADER;
using UnityEngine;

namespace TweakIt
{
    [UltraLoader.Entry]
    public class TweakIt : MonoBehaviour
    {
        private Harmony _harmony;

        public void Start()
        {
            _harmony = new Harmony("xyz.jame.tweakit");
            _harmony.PatchAll();

            UltraLoaderEvents.CheatsManagerStart += instance => { instance.RegisterCheat(Invincibility.Instance, "tweakit"); };
        }
    }
}