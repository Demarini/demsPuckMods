namespace demsInputControl.Utils
{
    public static class HarmonyUtils
    {
        public static PlayerBody GetPlayerBody(PlayerInput input)
        {
            return input?.Player?.PlayerBody;
        }
    }
}
