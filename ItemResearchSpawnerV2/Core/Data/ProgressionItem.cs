using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Models.Enums;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Models {
    internal class ProgressionItem {

        public SpawnableItem Item;
        public ProgressionData Progression;
        public ItemCategory Category;

        public int Price;

        public ProgressionItem(SpawnableItem item, ProgressionData progression, ItemCategory category, int price) {
            Item = item;
            Progression = progression;
            Category = category;
            Price = price;
        }

        public int GetAvailableQuantity(int money, ItemQuality requestedQuality, out ItemQuality maxAvailableQuality) {
            while (true) {
                int quantity;

                switch (requestedQuality) {
                    case ItemQuality.Silver:
                        if (Progression.ResearchCountSilver >= Category.BaseResearchCount) {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0) {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    case ItemQuality.Gold:
                        if (Progression.ResearchCountGold >= Category.BaseResearchCount) {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0) {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    case ItemQuality.Iridium:
                        if (Progression.ResearchCountIridium >= Category.BaseResearchCount) {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0) {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    default: {
                        maxAvailableQuality = requestedQuality;
                        return GetQuantityForQuality(money, ItemQuality.Normal);
                    }
                }
            }
        }

        private int GetQuantityForQuality(int money, ItemQuality requestedQuality) {
            var item = Item.CreateItem();

            if (item is SObject obj) {
                obj.Quality = (int)requestedQuality;
            }

            try {
                return money / ModManager.Instance.GetItemBuyPrice(item);
            }

            catch (Exception) {
                return item.maximumStackSize();
            }
        }

        public ItemQuality GetAvailableQuality(ItemQuality requestedQuality) {
            while (true) {
                switch (requestedQuality) {
                    case ItemQuality.Silver:
                        if (Progression.ResearchCountSilver >= Category.BaseResearchCount)
                            return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Gold:
                        if (Progression.ResearchCountGold >= Category.BaseResearchCount)
                            return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Iridium:
                        if (Progression.ResearchCountIridium >= Category.BaseResearchCount)
                            return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    default:
                        return ItemQuality.Normal;
                }
            }
        }
    }
}