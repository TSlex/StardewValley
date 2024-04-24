using Force.DeepCloner;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;


namespace ItemResearchSpawnerV2.Core {
    internal class ProgressionManager {

        public List<ItemCategoryMeta> Categories { get; private set; }
        public ItemCategoryMeta DefaultCategory { get; private set; }

        public Dictionary<string, ItemSaveData> ResearchProgressions { get; set; }

        private IModHelper Helper => ModManager.Instance.Helper;
        private IMonitor Monitor => ModManager.Instance.Monitor;
        private IManifest Manifest => ModManager.Instance.Manifest;

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
                ResearchProgressions[CommonHelper.GetItemUniqueKey(item.GameItem)] = item.GetSaveData();
                ModManager.Instance.UpdateMenu(rebuild: true);
                return;
            }

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

            leftAmount -= needAmount;

            ResearchProgressions[CommonHelper.GetItemUniqueKey(item.GameItem)] = item.GetSaveData();
            ModManager.Instance.UpdateMenu(rebuild: true);
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

            foreach (var item in GetMissingItems()) {
                yield return GetProgressionItem(item);
            }
        }

        public ProgressionItem GetProgressionItem(Item item) {
            var key = CommonHelper.GetItemUniqueKey(item);

            var possibleItem = ModManager.Instance.ItemRegistry
                .Where(p => p.Value.QualifiedItemId == item.QualifiedItemId)
                .Select(p => p.Value).FirstOrDefault();

            possibleItem ??= new SpawnableItem("", "", (item) => new MissingItem(key));

            if (possibleItem == null) {
                return GetProgressionItem(new SpawnableItem("", "", (item) => new MissingItem(key)));
            }

            // fix pre SDV1.6 items with different unique key...
            item.Name = possibleItem.Item.Name;
            item.ParentSheetIndex = possibleItem.Item.ParentSheetIndex;

            var spawnableItem = new SpawnableItem(possibleItem) {
                Item = item,
            };

            return GetProgressionItem(spawnableItem);
        }
        public ProgressionItem GetProgressionItem(SpawnableItem item) {

            var category = Categories.FirstOrDefault(rule => rule.IsMatch(item));
            category ??= DefaultCategory;

            var itemCategory = new ItemCategory {
                Label = I18n.GetByKey(category.Label),
                BasePrice = category.BaseCost,
                BaseResearchCount = item.Forbidden ? -1 : category.ResearchCount
            };

            var itemPrice = ModManager.Instance.GetItemBuyPrice(item.Item);
            itemPrice = itemPrice <= 0 ? category.BaseCost : itemPrice;

            var progressionData = GetProgressionDataOrDefault(item.Item);

            return new ProgressionItem(item, progressionData, itemCategory, itemPrice);

        }

        public ItemSaveData GetProgressionDataOrDefault(Item item) {

            var key = CommonHelper.GetItemUniqueKey(item);

            ItemSaveData progressionItem;

            if (item is MissingItem) {
                return new ItemSaveData() {
                    ResearchCount = 999,
                };
            }

            if (ResearchProgressions.ContainsKey(key)) {
                progressionItem = ResearchProgressions[key].DeepClone();
            }
            else {
                progressionItem = new ItemSaveData();
                ResearchProgressions[key] = progressionItem.DeepClone();
            }

            return progressionItem;
        }

        public IEnumerable<SpawnableItem> GetMissingItems() {
            var progressionStarted = ResearchProgressions.Where(p => p.Value.ResearchCount > 0).ToList();
            var existingItems = ModManager.Instance.ItemRegistry.Select(p => p.Key).ToList();

            var missingKeys = progressionStarted.Where(p => !existingItems.Contains(p.Key)).Select(p => p.Key);

            foreach (var key in missingKeys) {
                yield return new SpawnableItem("", "", (item) => new MissingItem(key));
            }
        }

        public void DumpPlayersProgression() {
            var onlinePlayers = Game1.getOnlineFarmers()
                .ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            var offlinePlayers = Game1.getAllFarmers()
                .Where(farmer => !onlinePlayers.Keys.Contains(farmer.UniqueMultiplayerID.ToString()))
                .ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            DumpPlayerProgression(Game1.player, ResearchProgressions);

            //if (Context.IsMultiplayer) {
            //    Helper.Multiplayer.SendMessage("", MessageKeys.PROGRESSION_DUMP_REQUIRED,
            //        new[] { Manifest.UniqueID });
            //}

            var progressions = ModManager.SaveManagerInstance.GetAllProgressions();

            foreach (var player in offlinePlayers) {
                DumpPlayerProgression(player.Value,
                    progressions.ContainsKey(player.Key)
                        ? progressions[player.Key]
                        : new Dictionary<string, ItemSaveData>());
            }
        }

        private void DumpPlayerProgression(Farmer player, Dictionary<string, ItemSaveData> progression) {
            Monitor.Log(
                $"Dumping progression - player: {player.Name}, location: {SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString())}",
                LogLevel.Info);

            Helper.Data.WriteJsonFile(SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString()),
                progression.Where(p => p.Value.ResearchCount > 0).ToList());
        }
    }
}
