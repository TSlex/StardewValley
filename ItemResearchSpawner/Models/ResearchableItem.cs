namespace ItemResearchSpawner.Models
{
    internal class ResearchableItem
    {
        public SpawnableItem Item { get; set; }
        
        public ResearchProgression Progression { get; set; }

        public ItemQuality GetAvailableQuality(ItemQuality requestedQuality)
        {
            while (true)
            {
                switch (requestedQuality)
                {
                    case ItemQuality.Silver:
                        if (Progression.ResearchCountSilver >= Item.ProgressionLimit) return ItemQuality.Silver;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Gold:
                        if (Progression.ResearchCountGold >= Item.ProgressionLimit) return ItemQuality.Gold;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Iridium:
                        if (Progression.ResearchCountIridium >= Item.ProgressionLimit) return ItemQuality.Iridium;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    default:
                        return ItemQuality.Normal;
                }
            }
        }
    }
}