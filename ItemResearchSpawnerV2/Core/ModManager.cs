﻿using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawnerV2.Core {
    internal class ModManager {
        public static ModManager Instance;
        public static ProgressionManager ProgressionManagerInstance => Instance.ProgressionManager;
        public static SaveManager SaveManagerInstance => Instance.SaveManager;
        public static CommandManager CommandManagerInstance => Instance.CommandManager;

        public readonly IModHelper Helper;
        public readonly IMonitor Monitor;
        public readonly IManifest Manifest;

        public ModConfig Config;

        public readonly ProgressionManager ProgressionManager;
        public readonly SaveManager SaveManager;
        public readonly CommandManager CommandManager;

        public ProgressionItem RecentlyUnlockedItem = null;
        public int RecentlyUnlockedItemIndex = -1;

        public readonly Texture2D UITexture;
        public static Texture2D UITextureInstance => Instance.UITexture;

        public readonly Dictionary<string, SpawnableItem> ItemRegistry = new Dictionary<string, SpawnableItem>();

        public string SelectedCategory = I18n.Category_All();
        public string SortOption = ItemSortOption.CategoryDESC.GetString();
        public string SearchText = "";

        public ModMode ModMode => Config.DefaultMode;

        public ItemQuality ItemQuality = ItemQuality.Normal;
        public FavoriteDisplayMode FavoriteDisplay = FavoriteDisplayMode.All;
        public ProgressionDisplayMode ProgressionDisplay = ProgressionDisplayMode.ResearchedOnly;

        public int JMTMoney = 0;
        public bool JMTMoneySyncRequired = false;

        public bool SaveDataLoaded = false;

        // ===========================================================================================

        public ModManager(IModHelper helper, ModConfig config, IMonitor monitor, IManifest manifest) {

            // ---------------------------------------------------------------------

            Instance ??= this;
            if (Instance != this) {
                monitor.Log($"Another instance of {nameof(ModManager)} exists!", LogLevel.Warn);
                return;
            }

            // ---------------------------------------------------------------------

            Helper = helper;
            Config = config;
            Monitor = monitor;
            Manifest = manifest;

            UITexture = helper.ModContent.Load<Texture2D>(UIConstants.UISheet);

            ProgressionManager = new ProgressionManager();
            SaveManager = new SaveManager();
            CommandManager = new CommandManager();
        }

        // ===========================================================================================

        public void OpenMenu() {
            //ProgressionManager.LoadCategories();
            if (SaveDataLoaded) {
                Game1.activeClickableMenu = new MainMenuController();
            }
        }

        public void UpdateMenu(bool rebuild = false, bool filter = false, bool resetScroll = false, bool reloadCategories = false, bool clearFiltering = false) {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is MainMenuController menu) {
                menu.UpdateView(rebuild, filter, resetScroll, reloadCategories, clearFiltering: clearFiltering);
            }
        }

        public List<ProgressionItem> GetProgressionItems() {
            return ProgressionManager.GetProgressionItems().ToList();
        }

        private void InitRegistry() {
            var blacklist = SaveManager.GetBannedItems();

            foreach (var item in ItemRepository.GetAll()) {

                var key = CommonHelper.GetItemUniqueKey(item.Item);

                if (blacklist.Contains(key)) {
                    item.Forbidden = true;
                }

                ItemRegistry[key] = item;
            }
        }

        #region SortingOptions

        public IEnumerable<string> GetDisplayCategories(IEnumerable<ProgressionItem> items) {

            var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items) {
                categories.Add(item.Category.Label);
            }

            yield return I18n.Category_All();

            foreach (string category in categories.OrderBy(p => p, StringComparer.OrdinalIgnoreCase)) {
                if (category == I18n.Category_Misc()) {
                    continue;
                }

                yield return category;
            }

            yield return I18n.Category_Misc();
        }

        private bool EqualsCaseInsensitive(string a, string b) {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Price/Money

        public int PlayerMoney {
            get {
                return ModMode switch {
                    ModMode.JunimoMagicTrade => JMTMoney,
                    ModMode.JunimoMagicTradePlus => JMTMoney,
                    _ => Game1.player._money,
                };
            }
        }

        private void AddPlayerMoney(int byAmount) {
            SetPlayerMoney(PlayerMoney + byAmount);
        }

        private void WithdrawPlayerMoney(int byAmount) {
            SetPlayerMoney(PlayerMoney - byAmount);
        }

        private void SetPlayerMoney(int value) {
            if (ModMode == ModMode.JunimoMagicTrade || ModMode == ModMode.JunimoMagicTradePlus) {
                JMTMoney = value;
                JMTMoneySyncRequired = true;
            }
            else {
                Game1.player._money = value;
            }
        }

        public void SyncJTMMoney() {
            if (!Context.IsMainPlayer && SaveDataLoaded && JMTMoneySyncRequired) {
                JMTMoneySyncRequired = false;

                NetworkManager.SendNetworkModMessage(new NetworkManager.OnCommitJMTMoneyMessage() {
                    JMTMoney = JMTMoney,
                });
            }
            else {
                JMTMoneySyncRequired = false;
            }
        }

        // -----------------------------------------------------------------

        public bool ShouldDisableItemByPrice(Item item) {
            return ModMode != ModMode.Research && ModMode != ModMode.ResearchPlus && !(CanBuyItem(item) && item.Stack > 0);
        }

        public bool CanBuyItem(Item item) {
            return GetItemBuyPrice(item, true) <= PlayerMoney;
        }

        public void BuyItem(Item item) {
            var price = GetItemBuyPrice(item, true);

            if (price > PlayerMoney) {
                //Game1.player._money = 0;
                SetPlayerMoney(0);
            }
            else {
                //Game1.player._money -= price;
                WithdrawPlayerMoney(price);
            }
        }

        public void SellItem(Item item) {
            //Game1.player._money += GetItemSellPrice(item, true);
            AddPlayerMoney(GetItemSellPrice(item, true));
        }

        // -----------------------------------------------------------------

        public (int buy, int sell) GetItemPrices(Item item, bool countStack = false) {

            return GetItemPrices(ProgressionManager.GetProgressionItem(item), countStack);
        }

        public (int buy, int sell) GetItemPrices(ProgressionItem item, bool countStack = false) {

            var buyPrice = GetItemBuyPrice(item, countStack);
            var sellPrice = GetItemSellPrice(item, countStack);

            return new(buyPrice, sellPrice);
        }

        public int GetItemBuyPrice(Item item, bool countStack = false) {

            return GetItemBuyPrice(ProgressionManager.GetProgressionItem(item), countStack);
        }

        public int GetItemBuyPrice(ProgressionItem item, bool countStack = false) {
            var buyPrice = GetItemPrice(item, countStack, Config.GetBuyPriceMultiplier());

            return buyPrice >= 0 ? buyPrice : 0;
        }

        public int GetItemSellPrice(Item item, bool countStack = false) {

            return GetItemSellPrice(ProgressionManager.GetProgressionItem(item), countStack);
        }

        public int GetItemSellPrice(ProgressionItem item, bool countStack = false) {

            if (item.Category.CannotBeSold) {
                return 0;
            }

            if (ModMode == ModMode.BuySellPlus) {
                return 1;
            }

            var sellPrice = GetItemPrice(item, countStack, Config.GetSellPriceMultiplier());

            return sellPrice >= 0 ? sellPrice : 0;
        }

        public int GetItemPrice(Item item, bool countStack = false, float multiplyBy = 1.0f) {

            return GetItemPrice(ProgressionManager.GetProgressionItem(item), countStack, multiplyBy);
        }

        public int GetItemPrice(ProgressionItem item, bool countStack = false, float multiplyBy = 1.0f) {

            var price = item.Price;

            if (multiplyBy != 1.0f) {
                price = (int) MathF.Round(price * multiplyBy);
            }

            if (countStack) {
                price *= item.GameItem.Stack;
            }

            return price;
        }

        #endregion

        #region Save/Load

        public void OnSave() {

            if (!SaveDataLoaded) {
                return;
            }

            var modState = new ModManagerState() {
                Config = Config,
                JMTMoney = JMTMoney,
                Quality = ItemQuality,
                ProgressionDisplayMode = ProgressionDisplay,
                FavoriteDisplayMode = FavoriteDisplay,
                SearchText = SearchText,
                Category = SelectedCategory,
                SortOption = Enum.GetValues(typeof(ItemSortOption)).Cast<ItemSortOption>()
                    .Where(op => op.GetString() == SortOption).FirstOrDefault()
            };

            SaveManager.CommitModState(Game1.player.UniqueMultiplayerID.ToString(), modState);

            // -------------------------------------------------------------------------------------

            SaveManager.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(),
                ProgressionManager.ResearchProgressions.Where(p => p.Value.ResearchCount > 0 || p.Value.Favorite)
                .ToDictionary(p => p.Key, p => p.Value));

            // -------------------------------------------------------------------------------------

            SaveManager.OnSave();
        }

        public void OnLoad() {
            SaveDataLoaded = false;

            if (!Context.IsSplitScreen && !Context.IsMainPlayer) {
                SaveManager.OnRemoteLoadRequested();
                return;
            }
            else {
                SaveManager.OnLoad();
                OnLoadReady();
            }
        }

        public void OnLoadReady() {
            InitRegistry();

            // -------------------------------------------------------------------------------------

            var modState = SaveManager.GetModState(Game1.player.UniqueMultiplayerID.ToString());

            Config = modState.Config;
            JMTMoney = modState.JMTMoney;
            ItemQuality = modState.Quality;
            ProgressionDisplay = modState.ProgressionDisplayMode;
            FavoriteDisplay = modState.FavoriteDisplayMode;
            SearchText = modState.SearchText;
            SelectedCategory = modState.Category;
            SortOption = modState.SortOption.GetString();

            // -------------------------------------------------------------------------------------

            ProgressionManager.ResearchProgressions = SaveManager.GetProgression(Game1.player.UniqueMultiplayerID.ToString());

            SaveDataLoaded = true;
        }

        #endregion
    }
}
