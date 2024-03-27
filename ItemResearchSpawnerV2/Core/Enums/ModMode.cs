namespace ItemResearchSpawnerV2.Core.Enums
{
    public enum ModMode
    {
        Research,
        BuySell,
        Combined,
    }

    internal static class ModModeExtensions
    {

        public static string GetString(this ModMode current)
        {
            return current switch
            {
                ModMode.Research => "Research (Spawn) mode",
                ModMode.BuySell => "Buy/Sell mode",
                ModMode.Combined => "Combined mode",
                _ => "???"
            };
        }
    }
}