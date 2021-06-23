using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ItemResearchSpawner.Models
{
    public class ModConfig
    {
        public KeybindList ShowMenuKey { get; set; } = KeybindList.ForSingle(SButton.R);
        public ModMode DefaultMode { get; set; } = ModMode.Spawn;
    }
}