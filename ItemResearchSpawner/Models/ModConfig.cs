using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ItemResearchSpawner.Models
{
    internal class ModConfig
    {
        public KeybindList ShowMenuKey { get; } = KeybindList.ForSingle(SButton.I);
        public int ResearchCountRequired { get; } = 25;
    }
}