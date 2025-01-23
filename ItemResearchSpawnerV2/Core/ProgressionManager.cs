using Force.DeepCloner;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;


namespace ItemResearchSpawnerV2.Core {
    internal class ProgressionManager {

        //public List<ItemCategoryMeta> Categories { get; private set; }
        //public ItemCategoryMeta DefaultCategory { get; private set; }

        public Dictionary<string, ItemSaveData> ResearchProgressions { get; set; }

        private IModHelper Helper => ModManager.Instance.Helper;
        private IMonitor Monitor => ModManager.Instance.Monitor;
        private IManifest Manifest => ModManager.Instance.Manifest;

        //public Dictionary<string, int> Pricelist { get; internal set; }

        // ========================================================================================================

        public ProgressionManager() {
        }

        //public void LoadCategories() {
        //    var categories = ModManager.Instance.Helper.Data.ReadJsonFile<List<ItemCategoryMeta>>(SaveHelper.CategoriesConfigPath);
        //    if (categories == null) {
        //        ModManager.Instance.Monitor.LogOnce("One of the mod files (assets/categories.json) is missing or invalid. Some features may not work correctly; consider reinstalling the mod.", LogLevel.Warn);
        //    }

        //    Categories = categories ?? new List<ItemCategoryMeta>();
        //    DefaultCategory = new ItemCategoryMeta("category.misc", 1, 1, null, null);
        //    Categories.Add(DefaultCategory);
        //}

        //public void LoadPricelist() {
        //    var categories = ModManager.Instance.Helper.Data.ReadJsonFile<List<ItemCategoryMeta>>(SaveHelper.PricelistConfigPath);
        //    if (categories == null) {
        //        ModManager.Instance.Monitor.LogOnce("One of the mod files (assets/pricelist.json) is missing or invalid. Some features may not work correctly; consider reinstalling the mod.", LogLevel.Warn);
        //    }

        //    Categories = categories ?? new List<ItemCategoryMeta>();
        //    DefaultCategory = new ItemCategoryMeta("category.misc", 1, 1, null, null);
        //    Categories.Add(DefaultCategory);
        //}

        // ========================================================================================================

        public void ResearchItem(ProgressionItem item, out int leftAmount) {
            leftAmount = item.Stack;

            if (item.GameItem is FishingRod fishingRod) {
                foreach(var attachement in fishingRod.attachments.ToList()) {
                    if (attachement != null) {
                        CommonHelper.TryReturnItemToInventory(attachement);
                    }
                }
            }
            if (item.GameItem is Slingshot slingshot) {
                foreach (var attachement in slingshot.attachments.ToList()) {
                    if (attachement != null) {
                        CommonHelper.TryReturnItemToInventory(attachement);
                    }
                }
            }

            if (item.RequiredResearch < 0) {
                ResearchProgressions[CommonHelper.GetItemUniqueKey(item.GameItem)] = item.GetSaveData();
                ModManager.SaveManagerInstance.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(), ResearchProgressions);
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

            if (ModManager.Instance.ModMode.HasPriceBehaviour() && needAmount > 0) {
                var itemToSell = item.GameItem;
                itemToSell.Stack = progressCount;

                ModManager.Instance.SellItem(itemToSell);

                if (ModManager.Instance.Config.GetEnableSounds()) {
                    var purchaseSound = ModManager.Instance.ModMode switch {
                        ModMode.JunimoMagicTrade => "junimoMeep1",
                        ModMode.JunimoMagicTradePlus => "junimoMeep1",
                        _ => "purchase",
                    };

                    Game1.playSound(purchaseSound);
                }
            }

            leftAmount -= progressCount;

            ResearchProgressions[CommonHelper.GetItemUniqueKey(item.GameItem)] = item.GetSaveData();
            ModManager.SaveManagerInstance.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(), ResearchProgressions);
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

        public SpawnableItem GetSpawnableItem(Item item) {
            var key = CommonHelper.GetItemUniqueKey(item);

            //var possibleItem = ModManager.Instance.ItemRegistry
            //    .Where(p => p.Value.QualifiedItemId == item.QualifiedItemId)
            //    .Select(p => p.Value).FirstOrDefault();

            SpawnableItem possibleItem = null;

            var possibleItems = ModManager.Instance.ItemRegistry
                .Where(p => p.Key == key)
                .Select(p => p.Value).ToList();

            if (possibleItems.Count == 1) {
                possibleItem = possibleItems[0];
            }
            else {
                possibleItem = possibleItems
                    .Where(p => p.QualifiedItemId == item.QualifiedItemId)
                    .Select(p => p).FirstOrDefault();
            }

            // in case unique key matching failed, use QualifiedItemId
            possibleItem ??= ModManager.Instance.ItemRegistry
                .Where(p => p.Value.QualifiedItemId == item.QualifiedItemId)
                .Select(p => p.Value).FirstOrDefault();

            //var possibleItem = ModManager.Instance.ItemRegistry
            //    .Where(p => p.Key == key)
            //    .Select(p => p.Value).FirstOrDefault();

            //// in case unique key matching failed, use QualifiedItemId
            //possibleItem ??= ModManager.Instance.ItemRegistry
            //    .Where(p => p.Value.QualifiedItemId == item.QualifiedItemId || p.Key == key)
            //    .Select(p => p.Value).FirstOrDefault();

            //possibleItem ??= new SpawnableItem("", "", (item) => new MissingItem(key));

            if (possibleItem == null) {
                return new SpawnableItem("", "", (item) => new MissingItem(key));
            }

            // fix pre SDV1.6 items with different unique key...
            item.Name = possibleItem.Item.Name;
            item.ParentSheetIndex = possibleItem.Item.ParentSheetIndex;

            return new SpawnableItem(possibleItem) {
                Item = item,
                UniqueKey = possibleItem.UniqueKey
            };
        }

        public ProgressionItem GetProgressionItem(Item item) {

            return GetProgressionItem(GetSpawnableItem(item));
        }

        public ProgressionItem GetProgressionItem(SpawnableItem item) {

            //var category = Categories.FirstOrDefault(rule => rule.IsMatch(item));
            //category ??= DefaultCategory;

            var category = ModManager.SaveManagerInstance.GetCategories().FirstOrDefault(rule => rule.IsMatch(item));
            category ??= ModManager.SaveManagerInstance.GetDefaultCategory();

            //var pricelistItem = ModManager.SaveManagerInstance.GetPricelist().FirstOrDefault(p => p.Key == item.UniqueKey);
            var pricelistItem = ModManager.SaveManagerInstance.GetPricelist()
                .Where(p => p.Key == item.UniqueKey)
                .Select(e => (KeyValuePair<string, int>?) e)
                .FirstOrDefault();

            var itemBaseResearchCount = (int) (category.ResearchCount * ModManager.Instance.Config.GetResearchAmountMultiplier());
            itemBaseResearchCount = itemBaseResearchCount >= 1 ? itemBaseResearchCount : 1;

            // prevents items from catalogues (free infinite items) to be used as money generator :)
            var cannotSold = category.Label switch {
                "category.house-decor" => true,
                "category.furniture" => true,
                _ => false,
            };

            var itemCategory = new ItemCategory {
                Label = I18n.GetByKey(category.Label),
                BasePrice = category.BaseCost,
                BaseResearchCount = item.Forbidden ? -1 : itemBaseResearchCount,
                CannotBeSold = cannotSold
            };

            itemCategory.BaseResearchCount = ModManager.Instance.ModMode switch {
                ModMode.Research => itemCategory.BaseResearchCount,
                ModMode.BuySell => 1,
                ModMode.Combined => itemCategory.BaseResearchCount,
                ModMode.ResearchPlus => 0,
                ModMode.BuySellPlus => 0,
                ModMode.JunimoMagicTrade => 1,
                ModMode.JunimoMagicTradePlus => itemCategory.BaseResearchCount,
                _ => itemCategory.BaseResearchCount,
            };

            //if (ModManager.Instance.ModMode != ModMode.Research && ModManager.Instance.ModMode != ModMode.Combined) {
            //    itemCategory.BaseResearchCount = 0;
            //}

            var itemPrice = Utility.getSellToStorePriceOfItem(item.Item, false);
            itemPrice = itemPrice <= 0 ? category.BaseCost : itemPrice;
            itemPrice = pricelistItem != null ? pricelistItem.Value.Value : itemPrice;

            if (ModManager.Instance.ModMode == ModMode.BuySellPlus) {
                itemPrice = itemPrice * 2;

                if (itemPrice <= 100) {
                    itemPrice *= 50;
                }
                else if (itemPrice <= 500) {
                    itemPrice *= 15;
                }
                else if (itemPrice <= 1000) {
                    itemPrice *= 10;
                }
                else if (itemPrice <= 5000) {
                    itemPrice *= 5;
                }
                else {
                    itemPrice *= 2;
                }
                if (itemPrice > 100000) {
                    itemPrice = 100000;
                }
            }

            if (ModManager.Instance.ModMode != ModMode.BuySellPlus && itemCategory.CannotBeSold) {
                itemPrice = 0;
            }

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
            //var onlinePlayers = Game1.getOnlineFarmers()
            //    .ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            //var offlinePlayers = Game1.getAllFarmers()
            //    .Where(farmer => !onlinePlayers.Keys.Contains(farmer.UniqueMultiplayerID.ToString()))
            //    .ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            //DumpPlayerProgression(Game1.player, ResearchProgressions);

            //if (Context.IsMultiplayer) {
            //    Helper.Multiplayer.SendMessage("", MessageKeys.PROGRESSION_DUMP_REQUIRED,
            //        new[] { Manifest.UniqueID });
            //}

            var progressions = ModManager.SaveManagerInstance.GetAllProgressions();
            var players = Game1.getAllFarmers().ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());

            foreach (var player in players) {
                DumpPlayerProgression(player.Value,
                    progressions.ContainsKey(player.Key)
                        ? progressions[player.Key]
                        : new Dictionary<string, ItemSaveData>());
            }
        }

        private void DumpPlayerProgression(Farmer player, Dictionary<string, ItemSaveData> progression) {
            //Monitor.Log(
            //    $"Dumping progression - player: {player.Name}, location: {SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString())}",
            //    LogLevel.Info);

            Helper.Data.WriteJsonFile(SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString()),
                progression.Where(p => p.Value.ResearchCount > 0).ToList());
        }

        public void LoadPlayersProgression() {
            var players = Game1.getAllFarmers().ToDictionary(farmer => farmer.UniqueMultiplayerID.ToString());
            //var progressions = ModManager.SaveManagerInstance.GetAllProgressions();

            foreach (var player in players) {
                var playerProgression = (Helper.Data.ReadJsonFile<List<KeyValuePair<string, ItemSaveData>>>(SaveHelper.ProgressionDumpPath(player.Key)) ??
                    new List<KeyValuePair<string, ItemSaveData>>()).ToDictionary(p => p.Key, p => p.Value);

                ModManager.SaveManagerInstance.CommitProgression(player.Key, playerProgression, replace: true);

                if (player.Key != Game1.player.UniqueMultiplayerID.ToString()) {
                    NetworkManager.SendNetworkModMessage(new NetworkManager.OnReplaceProgressionMessage() {
                        CommitProgression = playerProgression
                    }, playerID: player.Value.UniqueMultiplayerID);
                }
            }

            //foreach (var player in players) {
            //    if (!progressions.ContainsKey(player.Key)) {
            //        progressions[player.Key] = new Dictionary<string, ItemSaveData>();
            //    }
            //}

            //foreach (var playerID in progressions.Keys) {
            //    var playerData = (Helper.Data.ReadJsonFile<List<KeyValuePair<string, ItemSaveData>>>(SaveHelper.ProgressionDumpPath(playerID)) ?? new List<KeyValuePair<string, ItemSaveData>>()).ToDictionary(p => p.Key, p => p.Value);

            //    //foreach (var item in ModManager.Instance.ItemRegistry.Values) {
            //    //    if (!playerData.ContainsKey(item.UniqueKey)) {
            //    //        playerData[item.UniqueKey] = new ItemSaveData();
            //    //    }
            //    //}

            //    ModManager.SaveManagerInstance.CommitProgression(playerID, playerData, replace: true);

            //    if (playerID != Game1.player.UniqueMultiplayerID.ToString())
            //}

            Game1.activeClickableMenu = null;
            ResearchProgressions = ModManager.SaveManagerInstance.GetProgression(Game1.player.UniqueMultiplayerID.ToString());
        }

        public void UnlockAllProgression() {
            foreach (var item in ModManager.Instance.ItemRegistry.Values) {
                UnlockProgression(item.Item, false, doCommit: false);
            }
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("stardrop");
            }

            Game1.activeClickableMenu = null;
            ModManager.SaveManagerInstance.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(), ResearchProgressions);
        }

        public void UnlockProgression(Item item, bool playSound = true, bool doCommit = true) {
            var pI = GetProgressionItem(item);
            var saveData = pI.SaveData;

            saveData.ResearchCount = (int) MathF.Max(saveData.ResearchCount, pI.RequiredResearch);
            saveData.ResearchCountSilver = (int) MathF.Max(saveData.ResearchCountSilver, pI.RequiredResearch);
            saveData.ResearchCountGold = (int) MathF.Max(saveData.ResearchCountGold, pI.RequiredResearch);
            saveData.ResearchCountIridium = (int) MathF.Max(saveData.ResearchCountIridium, pI.RequiredResearch);

            ResearchProgressions[CommonHelper.GetItemUniqueKey(item)] = saveData;

            if (playSound && ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("stardrop");
            }

            if (doCommit) {
                Game1.activeClickableMenu = null;
                ModManager.SaveManagerInstance.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(), ResearchProgressions);
            }
        }
    }
}
