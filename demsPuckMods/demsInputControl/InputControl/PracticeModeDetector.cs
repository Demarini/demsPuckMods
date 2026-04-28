namespace demsInputControl.InputControl
{
    public static class PracticeModeDetector
    {
        public static bool IsPracticeMode
        {
            get
            {
                var sm = ServerManager.Instance;
                return sm != null
                    && sm.ServerConfig != null
                    && sm.ServerConfig.name == "PRACTICE";
            }
        }
    }
}
