using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using StardewModdingAPI;
using Force.DeepCloner;
using StardewModdingAPI.Events;
using StardewValley;
using static ItemResearchSpawnerV2.Core.NetworkManager;

namespace ItemResearchSpawnerV2.Core {
    internal class SaveManager {

        private Dictionary<string, Dictionary<string, ItemSaveData>> Progressions;
        private Dictionary<string, ModManagerState> ModStates;
        private Dictionary<string, int> Pricelist;
        private List<string> ItemBlacklist;

        private ICollection<ItemCategoryMeta> Categories;
        private ItemCategoryMeta DefaultCategory;

        private IModHelper ModHelper => ModManager.Instance.Helper;

        public SaveManager() {
            Progressions = new Dictionary<string, Dictionary<string, ItemSaveData>>();
            ModStates = new Dictionary<string, ModManagerState>();
            Pricelist = new Dictionary<string, int>();
            Categories = new List<ItemCategoryMeta>();
        }

        #region Getter/Setter

        // ----------------------------------------------------------------------------------------------------------------

        public Dictionary<string, Dictionary<string, ItemSaveData>> GetAllProgressions() {
            if (!Context.IsMainPlayer) {
                throw new NotImplementedException();
            }

            return Progressions.DeepClone();
        }

        public Dictionary<string, ItemSaveData> GetProgression(string playerID) {
            if (Progressions.ContainsKey(playerID)) {
                return Progressions[playerID].DeepClone() ?? new Dictionary<string, ItemSaveData>();
            }

            return new Dictionary<string, ItemSaveData>();
        }

        public void CommitResearch(string playerID, string key, ItemSaveData itemProgression) {
            var progression = GetProgression(playerID);

            progression[key] = itemProgression;

            Progressions[playerID] = progression;
        }

        public void CommitProgression(string playerID, Dictionary<string, ItemSaveData> commitProgression, bool replace = false) {
            if (!replace) {
                var progression = GetProgression(playerID);
                var changedProgression = new Dictionary<string, ItemSaveData>();

                foreach (var key in commitProgression.Keys.ToArray()) {
                    var prevState = progression.ContainsKey(key) ? progression[key] : new ItemSaveData();
                    var newState = commitProgression[key];

                    if (!newState.Equals(prevState)) {
                        changedProgression[key] = newState;
                    }

                    progression[key] = newState;
                }

                Progressions[playerID] = progression;

                if (!Context.IsMainPlayer && !Context.IsSplitScreen) {
                    NetworkManager.SendNetworkModMessage(new OnCommitProgressionMessage() {
                        CommitProgression = changedProgression
                    });
                }
            }
            else {
                Progressions[playerID] = commitProgression;
            }
        }

        // ----------------------------------------------------------------------------------------------------------------

        public ModManagerState GetModState(string playerID) {
            if (ModStates.ContainsKey(playerID)) {
                return ModStates[playerID] ?? new ModManagerState() {
                    Config = ModEntry.Instance.Config
                };
            }

            return new ModManagerState() {
                Config = ModEntry.Instance.Config
            };
        }

        public void CommitModState(string playerID, ModManagerState modState) {
            if (!Context.IsMainPlayer && !Context.IsSplitScreen) {
                //var modStateC = modState.DeepClone();
                //var showMenuButton = modState.Config.ShowMenuButton.ToString();
                //modState.Config.ShowMenuButton = null;

                NetworkManager.SendNetworkModMessage(new OnCommitModStateMessage() { 
                    ModState = modState,
                    //ShowMenuButton = showMenuButton
                });
            }

            ModStates[playerID] = modState;
        }

        // ----------------------------------------------------------------------------------------------------------------

        public Dictionary<string, int> GetPricelist() {
            return Pricelist.DeepClone();
        }

        public void CommitPricelist(Dictionary<string, int> pricelist) {
            Pricelist = new Dictionary<string, int>(pricelist);
        }

        // ----------------------------------------------------------------------------------------------------------------

        public List<ItemCategoryMeta> GetCategories() {
            return Categories.ToList();
        }

        public ItemCategoryMeta GetDefaultCategory() {
            return DefaultCategory;
        }

        public void CommitCategories(List<ItemCategoryMeta> categories) {
            Categories = categories;
        }

        // ----------------------------------------------------------------------------------------------------------------

        public List<string> GetBannedItems() {
            return ItemBlacklist;
        }

        // ----------------------------------------------------------------------------------------------------------------

        #endregion

        #region Load

        public void OnLoad() {
            if (!Context.IsMainPlayer && !Context.IsSplitScreen)
                throw new NotImplementedException();

            LoadProgression();
            LoadModState();
            LoadPricelist();
            LoadCategories();
            LoadItemBlacklist();
        }

        public void OnRemoteLoadRequested() {
            NetworkManager.SendNetworkModMessage(new NetworkManager.OnLoadRequestedMessage());
        }

        public void OnRemoteLoadSucceed(ICollection<ItemCategoryMeta> categories, ItemCategoryMeta defaultCategory, List<string> itemBlacklist, Dictionary<string, int> pricelist, Dictionary<string, ModManagerState> modStates, Dictionary<string, Dictionary<string, ItemSaveData>> progressions) {
            Categories = categories;
            DefaultCategory = defaultCategory;
            ItemBlacklist = itemBlacklist;
            Pricelist = pricelist;
            ModStates = modStates;
            Progressions = progressions;

            ModManager.Instance.OnLoadReady();
        }

        private void LoadProgression() {
            try {
                Progressions =
                    ModHelper.Data.ReadSaveData<Dictionary<string, Dictionary<string, ItemSaveData>>>(SaveHelper
                        .ProgressionsKey)
                    ?? new Dictionary<string, Dictionary<string, ItemSaveData>>();
            }
            catch (Exception _) {
                Progressions = new Dictionary<string, Dictionary<string, ItemSaveData>>();
            }
        }

        private void LoadModState() {

            try {
                ModStates = ModHelper.Data.ReadSaveData<Dictionary<string, ModManagerState>>(SaveHelper.ModStatesKey) ??
                             new Dictionary<string, ModManagerState>();
            }
            catch (Exception _) {
                ModStates = new Dictionary<string, ModManagerState>();
            }

        }

        private void LoadPricelist() {

            //if (!ModHelper.ReadConfig<ModConfig>().GetUseDefaultBalanceConfig()) {

            //    try {
            //        PriceList = ModHelper.Data.ReadGlobalData<Dictionary<string, int>>(SaveHelper.PriceConfigKey);
            //    }
            //    catch (Exception _) {
            //        PriceList = null;
            //    }

            //    PriceList ??= ModHelper.Data.ReadJsonFile<Dictionary<string, int>>(SaveHelper.PricelistConfigPath) ??
            //                   new Dictionary<string, int>();
            //}

            //else {
            //    PriceList = ModHelper.Data.ReadJsonFile<Dictionary<string, int>>(SaveHelper.PricelistConfigPath) ??
            //                 new Dictionary<string, int>();
            //}

            Dictionary<string, int> pricelist = null;

            try {
                pricelist = ModHelper.Data.ReadJsonFile<Dictionary<string, int>>(SaveHelper.PricelistConfigPath);

                if (pricelist == null) {
                    throw new Exception();
                }
            }
            catch (Exception _) {
                ModManager.Instance.Monitor.LogOnce("One of the mod files (assets/pricelist.json) is missing or invalid. Some features may not work correctly; consider reinstalling the mod.", LogLevel.Error);
            }

            Pricelist = pricelist ?? new Dictionary<string, int>();
        }

        private void LoadCategories() {

            //if (!ModHelper.ReadConfig<ModConfig>().GetUseDefaultBalanceConfig()) {

            //    try {
            //        Categories = ModHelper.Data.ReadGlobalData<List<ItemCategoryMeta>>(SaveHelper.CategoriesConfigKey);
            //    }
            //    catch (Exception _) {
            //        Categories = null;
            //    }

            //    Categories ??= ModHelper.Data.ReadJsonFile<List<ItemCategoryMeta>>(SaveHelper.CategoriesConfigPath) ??
            //                    new List<ItemCategoryMeta>();
            //}

            //else {
            //    Categories = ModHelper.Data.ReadJsonFile<List<ItemCategoryMeta>>(SaveHelper.CategoriesConfigPath) ??
            //                  new List<ItemCategoryMeta>();
            //}

            List<ItemCategoryMeta> categories = null;

            try {
                categories = ModManager.Instance.Helper.Data.ReadJsonFile<List<ItemCategoryMeta>>(SaveHelper.CategoriesConfigPath);

                if (categories == null) {
                    throw new Exception();
                }
            }
            catch (Exception _) {
                ModManager.Instance.Monitor.LogOnce("One of the mod files (assets/categories.json) is missing or invalid. Some features may not work correctly; consider reinstalling the mod.", LogLevel.Error);
            }

            Categories = categories ?? new List<ItemCategoryMeta>();
            DefaultCategory = new ItemCategoryMeta("category.misc", 1, 1, null, null);
            Categories.Add(DefaultCategory);
        }

        private void LoadItemBlacklist() {

            //try {
            //    ItemBlacklist = ModHelper.Data.ReadJsonFile<List<string>>(SaveHelper.BannedItemsConfigPath) ??
            //                 new List<string>();
            //}
            //catch (Exception _) {
            //    ItemBlacklist = new List<string>();
            //}

            List<string> blacklist = null;

            try {
                blacklist = ModHelper.Data.ReadJsonFile<List<string>>(SaveHelper.BannedItemsConfigPath);

                if (blacklist == null) {
                    throw new Exception();
                }
            }
            catch (Exception _) {
                ModManager.Instance.Monitor.LogOnce("One of the mod files (assets/banlist.json) is missing or invalid. Some features may not work correctly; consider reinstalling the mod.", LogLevel.Error);
            }

            ItemBlacklist = blacklist ?? new List<string>();
        }

        #endregion

        #region Save

        public void OnSave() {
            if (!Context.IsMainPlayer)
                return;

            SaveProgression();
            SaveModState();
            SavePricelist();
            SaveCategories();
        }

        private void SaveProgression() {
            ModHelper.Data.WriteSaveData(SaveHelper.ProgressionsKey, Progressions);
        }

        private void SaveModState() {
            ModHelper.Data.WriteSaveData(SaveHelper.ModStatesKey, ModStates);
        }

        private void SavePricelist() {
            if (!ModHelper.ReadConfig<ModConfig>().GetUseDefaultBalanceConfig()) {
                ModHelper.Data.WriteGlobalData(SaveHelper.PriceConfigKey, Pricelist);
            }
        }

        private void SaveCategories() {
            if (!ModHelper.ReadConfig<ModConfig>().GetUseDefaultBalanceConfig()) {
                ModHelper.Data.WriteGlobalData(SaveHelper.CategoriesConfigKey, Categories);
            }
        }

        #endregion
    }
}
