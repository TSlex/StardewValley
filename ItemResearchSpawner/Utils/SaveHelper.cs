using StardewValley;

namespace ItemResearchSpawner.Utils
{
    internal static class SaveHelper
    {
        public static string DirectoryName => $"{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}";
    }
}