using Unity.Netcode;

namespace demsInputControl.InputControl
{
    public static class PracticeModeDetector
    {
        // Practice mode = the local player is hosting a server whose ServerConfig name is "PRACTICE".
        // IsHost gate is required because ServerConfig is local host state and isn't reset when a
        // client joins a remote server — without this it stays at "PRACTICE" forever after a practice session.
        public static bool IsPracticeMode
        {
            get
            {
                var nm = NetworkManager.Singleton;
                if (nm == null || !nm.IsHost) return false;

                var sm = ServerManager.Instance;
                return sm != null
                    && sm.ServerConfig != null
                    && sm.ServerConfig.name == "PRACTICE";
            }
        }
    }
}
