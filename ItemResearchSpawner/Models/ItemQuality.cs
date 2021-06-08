using System;

namespace ItemResearchSpawner.Models
{
    internal enum ItemQuality
    {
        Normal = StardewValley.Object.lowQuality,
        Silver = StardewValley.Object.medQuality,
        Gold = StardewValley.Object.highQuality,
        Iridium = StardewValley.Object.bestQuality
    }

    internal static class ItemQualityExtensions
    {
        public static ItemQuality GetNext(this ItemQuality current)
        {
            return current switch
            {
                ItemQuality.Normal => ItemQuality.Silver,
                ItemQuality.Silver => ItemQuality.Gold,
                ItemQuality.Gold => ItemQuality.Iridium,
                ItemQuality.Iridium => ItemQuality.Normal,
                _ => throw new NotSupportedException($"Unknown quality '{current}'.")
            };
        }
    }
}