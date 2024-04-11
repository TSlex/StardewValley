using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using StardewModdingAPI;

namespace ItemResearchSpawnerV2.Core {
    internal class ProgressionManager {
        public List<ItemCategoryMeta> Categories { get; private set; }
        public ItemCategoryMeta DefaultCategory { get; private set; }

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

        //public void ResearchItem(Item item) {

        //    var itemProgressionRaw = GetItemProgressionRaw(item, out var progressionItem);

        //    if (itemProgressionRaw.max < 0) {
        //        return;
        //    }

        //    if (itemProgressionRaw.current >= itemProgressionRaw.max) {

        //        switch (modManager.modMode) {
        //            case ModMode.BuySell:
        //            case ModMode.Combined:
        //                //OnStackChanged?.Invoke(0);
        //                break;
        //            default:
        //                break;
        //        }

        //        return;
        //    }

        //    var needCount = itemProgressionRaw.max - itemProgressionRaw.current;

        //    var progressCount = item.Stack > needCount ? needCount : item.Stack;

        //    var itemQuality = (ItemQuality)((item as SObject)?.Quality ?? 0);

        //    if (itemQuality >= ItemQuality.Normal) {
        //        progressionItem.ResearchCount += progressCount;
        //    }

        //    if (itemQuality >= ItemQuality.Silver) {
        //        progressionItem.ResearchCountSilver += progressCount;
        //    }

        //    if (itemQuality >= ItemQuality.Gold) {
        //        progressionItem.ResearchCountGold += progressCount;
        //    }

        //    if (itemQuality >= ItemQuality.Iridium) {
        //        progressionItem.ResearchCountIridium += progressCount;
        //    }


        //    switch (modManager.modMode) {
        //        case ModMode.BuySell:
        //        case ModMode.Combined:
        //            //OnStackChanged?.Invoke(0);
        //            break;
        //        case ModMode.Research:
        //            //OnStackChanged?.Invoke(item.Stack - progressCount);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public IEnumerable<ProgressionItem> GetProgressionItems() {
            foreach (var item in ItemRepository.GetAll()) {

                var category = Categories.FirstOrDefault(rule => rule.IsMatch(item));
                category ??= DefaultCategory;

                var itemCategory = new ItemCategory {
                    Label = I18n.GetByKey(category.Label),
                    BasePrice = category.BaseCost,
                    BaseResearchCount = category.ResearchCount
                };

                var itemPrice = ModManager.Instance.GetItemBuyPrice(item.Item);
                itemPrice = itemPrice <= 0 ? category.BaseCost : itemPrice;

                yield return new ProgressionItem(item, new ItemSaveData(), itemCategory, itemPrice);
            }
        }
    }
}
