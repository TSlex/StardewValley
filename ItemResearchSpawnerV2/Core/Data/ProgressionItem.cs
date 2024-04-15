using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Models {
    internal class ItemCategory {
        public string Label;
        public int BasePrice;
        public int BaseResearchCount;
    }

    internal class ProgressionItem {

        public SpawnableItem Item;
        public ItemSaveData SaveData;
        public ItemCategory Category;

        public int Price;
        public int RequiredResearch => Category.BaseResearchCount;
        public int CurrentResearchAmount => GetResearchProgress(Quality);
        public int ResearchLeftAmount => RequiredResearch - CurrentResearchAmount;
        public bool ResearchCompleted => ResearchLeftAmount <= 0;
        public bool Favorited => SaveData.Favorite;
        public int Stack { get => Item.Item.Stack; set => Item.Item.Stack = value; }
        public ItemQuality Quality => (ItemQuality)((Item.Item as SObject)?.Quality ?? 0);

        public Item GameItem => Item.Item;

        public ProgressionItem(SpawnableItem item, ItemSaveData saveData, ItemCategory category, int price) {
            Item = item;
            SaveData = saveData;
            Category = category;
            Price = price;
        }

        public int GetResearchProgress(ItemQuality requestedQuality) {

            return requestedQuality switch {
                ItemQuality.Silver => SaveData.ResearchCountSilver,
                ItemQuality.Gold => SaveData.ResearchCountGold,
                ItemQuality.Iridium => SaveData.ResearchCountIridium,
                _ => SaveData.ResearchCount,
            };
        }

        public ItemQuality GetAvailableQuality(ItemQuality requestedQuality) {
            while (true) {
                switch (requestedQuality) {
                    case ItemQuality.Silver:
                        if (SaveData.ResearchCountSilver >= RequiredResearch)
                            return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Gold:
                        if (SaveData.ResearchCountGold >= RequiredResearch)
                            return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Iridium:
                        if (SaveData.ResearchCountIridium >= RequiredResearch)
                            return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    default:
                        return ItemQuality.Normal;
                }
            }
        }

        public int GetAvailableQuantity(int money, ItemQuality requestedQuality, out ItemQuality maxAvailableQuality) {
            while (true) {
                int quantity;

                switch (requestedQuality) {
                    case ItemQuality.Silver:
                        if (SaveData.ResearchCountSilver >= RequiredResearch) {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0) {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    case ItemQuality.Gold:
                        if (SaveData.ResearchCountGold >= RequiredResearch) {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0) {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    case ItemQuality.Iridium:
                        if (SaveData.ResearchCountIridium >= RequiredResearch) {
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
    }
}