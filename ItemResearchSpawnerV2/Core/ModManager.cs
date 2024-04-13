using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawnerV2.Core {
    internal class ModManager {
        public static ModManager Instance;
        public static ProgressionManager ProgressionManagerInstance => Instance.ProgressionManager;
        public static SaveManager SaveManagerInstance => Instance.SaveManager;


        public readonly IModHelper Helper;
        public readonly IMonitor Monitor;
        public readonly IManifest Manifest;
        public readonly ModConfig Config;

        public readonly ProgressionManager ProgressionManager;
        public readonly SaveManager SaveManager;


        public readonly Dictionary<string, SpawnableItem> ItemRegistry = new Dictionary<string, SpawnableItem>();

        public string SelectedCategory = I18n.Category_All();
        public string SortOption = ItemSortOption.CategoryDESC.GetString();
        public string SearchText = "";
        public ModMode ModMode = ModMode.Research;
        public ItemQuality ItemQuality = ItemQuality.Normal;
        public FavoriteDisplayMode FavoriteDisplay = FavoriteDisplayMode.All;
        public ProgressionDisplayMode ProgressionDisplay = ProgressionDisplayMode.ResearchedOnly;

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

            ProgressionManager = new ProgressionManager();
            SaveManager = new SaveManager();
        }

        // ===========================================================================================

        public void OpenMenu() {
            ProgressionManager.LoadCategories();
            Game1.activeClickableMenu = new MainMenuController();
        }

        public List<ProgressionItem> GetProgressionItems() {
            return ProgressionManager.GetProgressionItems().ToList();
        }

        private void InitRegistry() {
            var blacklist = SaveManager.GetBannedItems();

            foreach (var item in ItemRepository.GetAll()) {

                var key = CommonHelper.GetItemUniqueKey(item.Item);

                if (blacklist.Contains(key)) {
                    continue;
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

        public void BuyItem(Item item) {
            var price = GetItemBuyPrice(item, true);

            if (price > Game1.player._money) {
                Game1.player._money = 0;
            }
            else {
                Game1.player._money -= price;
            }
        }

        public void SellItem(Item item) {
            Game1.player._money += GetItemSellPrice(item, true);
        }

        public (int buy, int sell) GetItemPrices(Item item, bool countStack = false) {

            var price = GetItemPrice(item, false);

            var buyPrice = (int)MathF.Round(price * Config.BuyPriceMultiplier);
            buyPrice = buyPrice >= 0 ? buyPrice : 0;

            var sellPrice = (int)MathF.Round(price * Config.SellPriceMultiplier);
            sellPrice = sellPrice >= 0 ? sellPrice : 0;

            if (countStack) {
                buyPrice *= item.Stack;
                sellPrice *= item.Stack;
            }


            return new(buyPrice, sellPrice);
        }

        public int GetItemBuyPrice(Item item, bool countStack = false) {
            var buyPrice = GetItemPrice(item, countStack, Config.BuyPriceMultiplier);

            return buyPrice >= 0 ? buyPrice : 0;
        }

        public int GetItemSellPrice(Item item, bool countStack = false) {
            var sellPrice = GetItemPrice(item, countStack, Config.SellPriceMultiplier);

            return sellPrice >= 0 ? sellPrice : 0;
        }

        public int GetItemPrice(Item item, bool countStack = false, float multiplyBy = 1.0f) {

            item.Stack = item.Stack > 0 ? item.Stack : 1;

            //var spawnableItem = GetSpawnableItem(item, out var key);

            //if (spawnableItem == null) {
            //    return 0;
            //}

            var price = -1;

            //if (_pricelist.ContainsKey(key)) {
            //    price = _pricelist[key];
            //}

            if (price < 0) {
                price = Utility.getSellToStorePriceOfItem(item, false);
            }

            //if (price <= 0 && !_pricelist.ContainsKey(key)) {
            //    price = spawnableItem.CategoryPrice;
            //}

            if (multiplyBy != 1.0f) {
                price = (int)MathF.Round(price * multiplyBy);
            }

            if (countStack) {
                price *= item.Stack;
            }

            return price;
        }

        //public void SetItemPrice(Item activeItem, int price) {
        //    var key = Helpers.GetItemUniqueKey(activeItem);

        //    if (price < 0 && _pricelist.ContainsKey(key)) {
        //        _pricelist.Remove(key);
        //    }
        //    else {
        //        _pricelist[key] = price;
        //    }
        //}

        #endregion

        #region Save/Load

        public void OnSave() {
            SaveManager.OnSave();
        }

        public void OnLoad() {
            SaveManager.OnLoad();
            InitRegistry();

            // -------------------------------------------------------------------------------------

            var modState = SaveManager.GetModState(Game1.player.UniqueMultiplayerID.ToString());

            ModMode = modState.ActiveMode;
            ItemQuality = modState.Quality;
            ProgressionDisplay = modState.ProgressionDisplayMode;
            FavoriteDisplay = modState.FavoriteDisplayMode;
            SearchText = modState.SearchText;
            SelectedCategory = modState.Category;
            SortOption = modState.SortOption.GetString();

            // -------------------------------------------------------------------------------------

            ProgressionManager.ResearchProgressions = SaveManager.GetProgression(Game1.player.UniqueMultiplayerID.ToString());
        }


        #endregion`
    }
}
