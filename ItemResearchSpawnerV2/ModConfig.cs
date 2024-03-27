using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using ItemResearchSpawnerV2.Core.Enums;

namespace ItemResearchSpawnerV2
{
    internal class ModConfig
    {
        public KeybindList ShowMenuButton { get; set; } = KeybindList.ForSingle(SButton.R);
        public ModMode DefaultMode { get; set; } = ModMode.Research;
        public bool UseDefaultBalanceConfig { get; set; } = true;

        public float ResearchAmountMultiplier { get; set; } = 1.5f;

        public float SellPriceMultiplier { get; set; } = 0.9f;

        public float BuyPriceMultiplier { get; set; } = 1.1f;
    }
}
