using ItemResearchSpawner.Models.Enums;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ItemResearchSpawner
{
    public class ModConfig
    {
        public KeybindList ShowMenuKey { get; set; } = KeybindList.ForSingle(SButton.R);
        public ModMode DefaultMode { get; set; } = ModMode.Spawn;
        public bool UseDefaultConfig { get; set; } = true;
    }
}