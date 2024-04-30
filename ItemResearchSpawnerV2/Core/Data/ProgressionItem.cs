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
        public bool ResearchStarted => !ResearchCompleted && ResearchLeftAmount < RequiredResearch;

        public int BaseResearchAmount => GetResearchProgress(ItemQuality.Normal);
        public int BaseResearchLeftAmount => RequiredResearch - BaseResearchAmount;
        public bool BaseResearchCompleted => BaseResearchLeftAmount <= 0;
        public bool BaseResearchStarted => !BaseResearchCompleted && BaseResearchLeftAmount < RequiredResearch;

        public int ResearchPerc => (int)((CurrentResearchAmount * 1f) / (RequiredResearch * 1f) * 100f);

        public bool Favorited => SaveData.Favorite;
        public bool Forbidden => Item.Forbidden;
        public bool Missing => GameItem is MissingItem;
        public bool CannotResearch => Forbidden || Missing;

        public bool NormalQualityForced => (Item.Item as SObject)?.Quality == null;

        public int Stack { get => Item.Item.Stack; set => Item.Item.Stack = value; }

        public ItemQuality Quality => (ItemQuality)((Item.Item as SObject)?.Quality ?? 0);

        public Item GameItem => Item.Item;

        public ProgressionItem(SpawnableItem item, ItemSaveData saveData, ItemCategory category, int price) {
            Item = item;
            SaveData = saveData;
            Category = category;
            Price = price;
        }

        public Item InstanciateItem() {
            var itemInstrace = Item.CreateItem();

            if (itemInstrace is StardewValley.Objects.Clothing clothingItem) {
                clothingItem.clothesColor.Value = SaveData.ClothesColor;
            }

            if (itemInstrace is StardewValley.Tools.WateringCan can) {
                can.WaterLeft = SaveData.WaterLevel;
            }

            return itemInstrace;
        }

        public ItemSaveData GetSaveData() { 
            var saveData = SaveData;

            if (GameItem is StardewValley.Objects.Clothing clothingItem) {
                saveData.ClothesColor = clothingItem.clothesColor.Value;
            }

            if (GameItem is StardewValley.Tools.WateringCan can) {
                saveData.WaterLevel = can.WaterLeft;
            }

            return SaveData; 
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
            if (NormalQualityForced) {
                return ItemQuality.Normal;
            }

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