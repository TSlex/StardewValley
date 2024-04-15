using Force.DeepCloner;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;


namespace ItemResearchSpawnerV2.Core {
    internal class ProgressionManager {
        public List<ItemCategoryMeta> Categories { get; private set; }
        public ItemCategoryMeta DefaultCategory { get; private set; }
        public Dictionary<string, ItemSaveData> ResearchProgressions { get; set; }

        //private readonly IEnumerable<SpawnableItem> Items;

        // ========================================================================================================

        public ProgressionManager() {
        }

        public void LoadCategories() {
            var categories = ModManager.Instance.Helper.Data.ReadJsonFile<List<ItemCategoryMeta>>(SaveHelper.CategoriesConfigPath);
            if (categories == null) {
                ModManager.Instance.Monitor.LogOnce("One of the mod files (assets/categories.json) is missing or invalid. Some features may not work correctly; consider reinstalling the mod.", LogLevel.Warn);
            }

            Categories = categories ?? new List<ItemCategoryMeta>();
            DefaultCategory = new ItemCategoryMeta("category.misc", 1, 1, null, null);
            Categories.Add(DefaultCategory);
        }

        // ========================================================================================================

        public void ResearchItem(ProgressionItem item, out int leftAmount) {
            leftAmount = item.Stack;


            if (item.RequiredResearch < 0) {
                return;
            }

            //if (itemProgressionRaw.current >= itemProgressionRaw.max) {

            //    switch (ModManager.Instance.ModMode) {
            //        case ModMode.BuySell:
            //        case ModMode.Combined:
            //            //OnStackChanged?.Invoke(0);
            //            break;
            //        default:
            //            break;
            //    }

            //    return;
            //}

            var needAmount = item.RequiredResearch - item.CurrentResearchAmount;

            var progressCount = item.Stack > needAmount ? needAmount : item.Stack;

            if (item.Quality >= ItemQuality.Normal) {
                item.SaveData.ResearchCount += progressCount;
            }

            if (item.Quality >= ItemQuality.Silver) {
                item.SaveData.ResearchCountSilver += progressCount;
            }

            if (item.Quality >= ItemQuality.Gold) {
                item.SaveData.ResearchCountGold += progressCount;
            }

            if (item.Quality >= ItemQuality.Iridium) {
                item.SaveData.ResearchCountIridium += progressCount;
            }

            leftAmount -= leftAmount;

            ResearchProgressions[CommonHelper.GetItemUniqueKey(item.GameItem)] = item.SaveData;


            switch (ModManager.Instance.ModMode) {
                case ModMode.BuySell:
                case ModMode.Combined:
                    //OnStackChanged?.Invoke(0);
                    break;
                case ModMode.Research:
                    //OnStackChanged?.Invoke(item.Stack - progressCount);
                    break;
                default:
                    break;
            }
        }

        public void FavoriteItem(ProgressionItem item) {
            item.SaveData.Favorite = !item.SaveData.Favorite;
            ResearchProgressions[CommonHelper.GetItemUniqueKey(item.GameItem)] = item.SaveData;
            ModManager.Instance.UpdateMenu(filter: true);
        }

        public IEnumerable<ProgressionItem> GetProgressionItems() {
            foreach (var item in ModManager.Instance.ItemRegistry.Values) {
                yield return GetProgressionItem(item);
            }
        }

        public ProgressionItem GetProgressionItem(Item item) {
            var key = CommonHelper.GetItemUniqueKey(item);
            var succ = ModManager.Instance.ItemRegistry.TryGetValue(key, out var spawnableItem);

            spawnableItem.Item = item;

            return GetProgressionItem(spawnableItem);
        }
        public ProgressionItem GetProgressionItem(SpawnableItem item) {

            var category = Categories.FirstOrDefault(rule => rule.IsMatch(item));
            category ??= DefaultCategory;

            var itemCategory = new ItemCategory {
                Label = I18n.GetByKey(category.Label),
                BasePrice = category.BaseCost,
                BaseResearchCount = category.ResearchCount
            };

            var itemPrice = ModManager.Instance.GetItemBuyPrice(item.Item);
            itemPrice = itemPrice <= 0 ? category.BaseCost : itemPrice;

            var progressionData = GetProgressionDataOrDefault(item.Item);

            return new ProgressionItem(item, progressionData, itemCategory, itemPrice);

        }

        public ItemSaveData GetProgressionDataOrDefault(Item item) {

            var key = CommonHelper.GetItemUniqueKey(item);

            ItemSaveData progressionItem;

            if (ResearchProgressions.ContainsKey(key)) {
                progressionItem = ResearchProgressions[key].DeepClone();
            }
            else {
                progressionItem = new ItemSaveData();
                ResearchProgressions[key] = progressionItem.DeepClone();
            }

            return progressionItem;
        }
    }
}
