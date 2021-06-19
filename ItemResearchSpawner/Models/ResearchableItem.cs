namespace ItemResearchSpawner.Models
{
    internal class ResearchableItem
    {
        public SpawnableItem Item { get; set; }
        
        public ResearchProgression Progression { get; set; }
        
        public int NeededProgression { get; set; }

        // public int GetItemPrice()
        // {
        //     var price = Item.Item.salePrice();
        //
        //     if (price <= 0)
        //     {
        //         price = Item.Category.
        //     }
        //     
        //     return Item.Item.salePrice()
        // }
        //
        // public int GetAvailableQuantity()
        // {
        //     
        // }

        public ItemQuality GetAvailableQuality(ItemQuality requestedQuality)
        {
            while (true)
            {
                switch (requestedQuality)
                {
                    case ItemQuality.Silver:
                        if (Progression.ResearchCountSilver >= NeededProgression) return ItemQuality.Silver;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Gold:
                        if (Progression.ResearchCountGold >= NeededProgression) return ItemQuality.Gold;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Iridium:
                        if (Progression.ResearchCountIridium >= NeededProgression) return ItemQuality.Iridium;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    default:
                        return ItemQuality.Normal;
                }
            }
        }
    }
}