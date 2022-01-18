namespace TweakIt.Cheats
{
    public class Invincibility : ICheat
    {
        public string LongName => "Invincibility";
        public string Identifier => "xyz.jame.tweakit.invincibility";
        public string ButtonEnabledOverride => null;

        public string ButtonDisabledOverride => null;

        public string Icon => null;
        public bool IsActive { get; private set; }
        public bool DefaultState => false;
        public StatePersistenceMode PersistenceMode => StatePersistenceMode.Persistent;

        public static Invincibility Instance { get; } = new Invincibility();

        private Invincibility()
        {
        }

        public void Enable()
        {
            IsActive = true;
        }

        public void Disable()
        {
            IsActive = false;
        }

        public void Update()
        {
        }
    }
}