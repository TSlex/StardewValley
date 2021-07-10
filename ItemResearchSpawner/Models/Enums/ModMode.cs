namespace ItemResearchSpawner.Models.Enums
{
    public enum ModMode
    {
        Spawn,
        Buy
    }
    
    internal static class ModModeExtensions{
        
        public static string GetString(this ModMode current)
        {
            return current switch
            {
                ModMode.Spawn => "Spawn mode",
                ModMode.Buy => "Buy/Sell mode",
                _ => "???"
            };
        }
    }
}