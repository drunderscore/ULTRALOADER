namespace ULTRALOADER
{
    public static class UltraLoaderEvents
    {
        public delegate void CheatsManagerStartEventHandler(CheatsManager instance);

        public delegate void CheatsManagerRenderCheatsInfoEventHandler(ref string value);

        public static event CheatsManagerStartEventHandler CheatsManagerStart;
        public static event CheatsManagerRenderCheatsInfoEventHandler CheatsManagerRenderInfo;

        internal static void OnCheatsManagerStart(CheatsManager instance)
        {
            CheatsManagerStart?.Invoke(instance);
        }

        internal static void OnCheatsManagerRenderInfo(ref string value)
        {
            CheatsManagerRenderInfo?.Invoke(ref value);
        }
    }
}