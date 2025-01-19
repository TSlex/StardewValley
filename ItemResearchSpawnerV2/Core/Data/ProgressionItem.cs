using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Models {
    internal class ItemCategory {
        public string Label;
        public int BasePrice;
        public int BaseResearchCount;
        public bool CannotBeSold;
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
        public bool ResearchStarted => ResearchLeftAmount < RequiredResearch && ResearchLeftAmount > 0;

        public int RequestedResearchAmount => GetResearchProgress(ModManager.Instance?.ItemQuality ?? ItemQuality.Normal);
        public int RequestedResearchLeftAmount => RequiredResearch - RequestedResearchAmount;
        public bool RequestedResearchCompleted => RequestedResearchLeftAmount <= 0;
        public bool RequestedResearchStarted => RequestedResearchLeftAmount < RequiredResearch && RequestedResearchLeftAmount > 0;

        public int BaseResearchAmount => GetResearchProgress(ItemQuality.Normal);
        public int BaseResearchLeftAmount => RequiredResearch - BaseResearchAmount;
        public bool BaseResearchCompleted => BaseResearchLeftAmount <= 0;
        public bool BaseResearchStarted => BaseResearchLeftAmount < RequiredResearch && (BaseResearchLeftAmount > 0 || RequiredResearch == 1) && !BaseResearchCompleted;

        public int ResearchPerc => (int) ((CurrentResearchAmount * 1f) / (RequiredResearch * 1f) * 100f);

        public bool Favorited => SaveData.Favorite;
        public bool Forbidden => Item.Forbidden;
        public bool Missing => GameItem is MissingItem;
        public bool CannotResearch => Forbidden || Missing;

        public bool NormalQualityForced => (Item.Item as SObject)?.Quality == null;

        public int Stack { get => Item.Item.Stack; set => Item.Item.Stack = value; }

        public ItemQuality Quality => (ItemQuality) ((Item.Item as SObject)?.Quality ?? 0);

        public Item GameItem => Item.Item;

        public ProgressionItem(SpawnableItem item, ItemSaveData saveData, ItemCategory category, int price) {
            Item = item;
            SaveData = saveData;
            Category = category;
            Price = price;
        }

        public Item InstanciateItem() {
            var itemInstrace = Item.CreateItem();

            // leave for now for backwards compatibility
            if (itemInstrace is StardewValley.Objects.Clothing clothingItem) {
                clothingItem.clothesColor.Value = SaveData.ClothesColor;
            }

            // leave for now for backwards compatibility
            if (itemInstrace is StardewValley.Tools.WateringCan can) {
                can.WaterLeft = SaveData.WaterLevel;
            }

            //if (itemInstrace is StardewValley.Objects.ColoredObject coloredObject) {

            //    coloredObject.color.Value = SaveData.TryGetMetaPropery("Color", coloredObject.color.Value);
            //}

            if (SaveData.Meta.ContainsKey("Color")) {
                if (StardewValley.Objects.ColoredObject.TrySetColor(itemInstrace, SaveData.TryGetMetaPropery("Color", Color.White), out var coloredItemInstance)){
                    itemInstrace = coloredItemInstance;
                }
            }

            if (itemInstrace is Tool tool) {
                var loadedEnchantments = SaveData.TryGetMetaPropery("Enchantments", new List<ToolEnchantment>());
                //var availableEnchantments = BaseEnchantment.GetAvailableEnchantmentsForItem(tool).Select(ne => (Base: ne, Name: ne.GetType().Name)).ToList();
                var availableEnchantments = CommonHelper.GetAllEnchantments()
                    .Where(ne => ne.CanApplyTo(GameItem))
                    .Select(ne => (Base: ne, Name: ne.GetType().Name))
                    .ToList();


                var join = availableEnchantments.Join(loadedEnchantments, a => a.Name, b => b.Name, (a, b) => (a.Base, a.Name, b.Level)).ToList();

                foreach (var enchantment in join) {
                    var enchInstance = enchantment.Base;
                    enchInstance.Level = enchantment.Level;

                    tool.AddEnchantment(enchInstance);
                }
            }

            if (itemInstrace is CombinedRing cRing) {
                var ringsKeys = SaveData.TryGetMetaPropery("CombinedRings", new List<string>());

                try {
                    var ringsItems = ringsKeys.Select(rk => ModManager.Instance.ItemRegistry[rk].Item).ToList();

                    foreach(var ringsItem in ringsItems) {
                        if (ringsItem is Ring ring) {
                            cRing.combinedRings.Add(ring);
                        }
                    }

                }
                catch { }

            }

            if (itemInstrace is Trinket trinket) {
                trinket.RerollStats(SaveData.TryGetMetaPropery("TrinketSeed", trinket.generationSeed.Value));
            }

            return itemInstrace;
        }

        public ItemSaveData GetSaveData() {
            var saveData = SaveData;

            if (GameItem is StardewValley.Objects.Clothing clothingItem) {

                // leave for now for backwards compatibility
                saveData.ClothesColor = clothingItem.clothesColor.Value;
                saveData.Meta["ClothesColor"] = JsonConvert.SerializeObject(clothingItem.clothesColor.Value); // new way of storing item's meta
            }

            if (GameItem is StardewValley.Tools.WateringCan can) {

                // leave for now for backwards compatibility
                saveData.WaterLevel = can.WaterLeft;
                saveData.Meta["WaterLevel"] = JsonConvert.SerializeObject(can.WaterLeft); // new way of storing item's meta
            }

            if (GameItem is StardewValley.Objects.ColoredObject coloredObject) {
                saveData.Meta["Color"] = JsonConvert.SerializeObject(coloredObject.color.Value);
            }

            if (GameItem is Tool tool) {
                var enchantments = tool.enchantments.Select(ne => new ToolEnchantment(Level: ne.Level, Name: ne.GetType().Name)).ToList();

                saveData.Meta["Enchantments"] = JsonConvert.SerializeObject(enchantments);
            }

            if (GameItem is CombinedRing cRing) {
                var rings = cRing.combinedRings.Select(cr => ModManager.ProgressionManagerInstance.GetSpawnableItem(cr).UniqueKey).ToList();

                saveData.Meta["CombinedRings"] = JsonConvert.SerializeObject(rings);
            }

            if (GameItem is Trinket trinket) {
                saveData.Meta["TrinketSeed"] = JsonConvert.SerializeObject(trinket.generationSeed.Value);
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
                obj.Quality = (int) requestedQuality;
            }

            var itemBuyPrice = ModManager.Instance.GetItemBuyPrice(item);

            if (itemBuyPrice <= 0) {
                return item.maximumStackSize();
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