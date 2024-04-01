using ItemResearchSpawnerV2.Core.Componets;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Models.Enums;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawnerV2.Core {
    internal class ModManager {
        public static ModManager Instance;

        public readonly IModHelper helper;
        public readonly IMonitor monitor;
        public readonly IManifest manifest;
        public readonly ModConfig config;

        public readonly ProgressionManager ProgressionManager;

        public ModMode ModMode = ModMode.Research;
        public ItemQuality ItemQuality = ItemQuality.Normal;
        public ProgressionDisplayMode ProgressionDisplay = ProgressionDisplayMode.ResearchedOnly;
        public FavoriteDisplayMode FavoriteDisplay = FavoriteDisplayMode.All;

        // ===========================================================================================

        public ModManager(IModHelper helper, ModConfig config, IMonitor monitor, IManifest manifest) {

            // ---------------------------------------------------------------------

            Instance ??= this;
            if (Instance != this) {
                monitor.Log($"Another instance of {nameof(ModManager)} exists!", LogLevel.Warn);
                return;
            }

            // ---------------------------------------------------------------------

            this.helper = helper;
            this.config = config;
            this.monitor = monitor;
            this.manifest = manifest;

            ProgressionManager = new ProgressionManager();
        }

        // ===========================================================================================

        public void OpenMenu() {
            Game1.activeClickableMenu = new MainMenuController();
        }

        public IEnumerable<Models.SpawnableItem> GetProgressionItems() {
            return ProgressionManager.GetSpawnableItems();
        }

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

            var buyPrice = (int)MathF.Round(price * config.BuyPriceMultiplier);
            buyPrice = buyPrice >= 0 ? buyPrice : 0;

            var sellPrice = (int)MathF.Round(price * config.SellPriceMultiplier);
            sellPrice = sellPrice >= 0 ? sellPrice : 0;

            if (countStack) {
                buyPrice *= item.Stack;
                sellPrice *= item.Stack;
            }


            return new(buyPrice, sellPrice);
        }

        public int GetItemBuyPrice(Item item, bool countStack = false) {
            var buyPrice = GetItemPrice(item, countStack, config.BuyPriceMultiplier);

            return buyPrice >= 0 ? buyPrice : 0;
        }

        public int GetItemSellPrice(Item item, bool countStack = false) {
            var sellPrice = GetItemPrice(item, countStack, config.SellPriceMultiplier);

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
    }
}
