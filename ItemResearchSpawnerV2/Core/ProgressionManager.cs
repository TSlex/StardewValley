using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Models;
using ItemResearchSpawnerV2.Models.Enums;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core {
    internal class ProgressionManager {

        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IManifest manifest;

        private readonly IEnumerable<SpawnableItem> allItems;

        // ========================================================================================================

        public ProgressionManager(IModHelper helper, IMonitor monitor, IManifest manifest) {
            this.helper = helper;
            this.monitor = monitor;
            this.manifest = manifest;

            this.allItems = GetSpawnableItems();
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

        public IEnumerable<SpawnableItem> GetSpawnableItems() {
            foreach (var item in new ItemRepository().GetAll()) {

                //var category = _categories?.FirstOrDefault(rule => rule.IsMatch(entry));
                //var label = category != null
                //    ? I18n.GetByKey(category.Label).Default(category.Label)
                //    : I18n.Category_Misc();

                //int baseResearchCount = category?.ResearchCount ?? 1;
                ////float researchMultiplier = _helper.ReadConfig<ModConfig>().ResearchAmountMultiplier;

                //yield return new SpawnableItem(entry, label ?? I18n.Category_Misc(), category?.BaseCost ?? 100,
                //    (int)((float)baseResearchCount * _config.ResearchAmountMultiplier));

                yield return new SpawnableItem(item, "_CATEGORY_", 100, 10);
            }
        }
    }
}
