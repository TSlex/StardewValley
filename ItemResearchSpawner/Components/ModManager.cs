using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Models.Messages;
using ItemResearchSpawner.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ItemResearchSpawner.Components
{
    internal class ModManager
    {
        public static ModManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly IManifest _modManifest;

        public readonly Dictionary<string, SpawnableItem> ItemRegistry = new();

        private Dictionary<string, int> _pricelist;
        private ModDataCategory[] _categories;

        #region Proprerties

        private ItemQuality _quality;
        private ModMode _modMode;
        private ItemSortOption _sortOption;
        private string _searchText;
        private string _category;

        public ItemQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                RequestMenuUpdate(false);
            }
        }

        public ModMode ModMode
        {
            get => _modMode;
            set
            {
                _modMode = value;
                RequestMenuUpdate(true);
            }
        }

        public ItemSortOption SortOption
        {
            get => _sortOption;
            set
            {
                _sortOption = value;
                RequestMenuUpdate(true);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RequestMenuUpdate(true);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                RequestMenuUpdate(true);
            }
        }

        public ModDataCategory[] AvailableCategories => _categories;

        #endregion

        public delegate void UpdateMenuView(bool rebuild);

        public event UpdateMenuView OnUpdateMenuView;

        public ModManager(IMonitor monitor, IModHelper helper, IManifest modManifest)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;
            _modManifest = modManifest;

            _helper.Events.GameLoop.DayEnding += OnSave;
            _helper.Events.GameLoop.DayStarted += OnLoad;
            _helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
        }

        public void InitRegistry(SpawnableItem[] items)
        {
            foreach (var spawnableItem in items)
            {
                var key = Helpers.GetItemUniqueKey(spawnableItem.Item);
        
                // fix copper pan
                if (key.Equals("Copper Pan:-1") && spawnableItem.Type == ItemType.Hat)
                {
                    continue;
                }
        
                ItemRegistry[key] = spawnableItem;
            }
        }

        public void RequestMenuUpdate(bool rebuild)
        {
            OnUpdateMenuView?.Invoke(rebuild);
        }

        public void BuyItem(Item item)
        {
            var price = GetItemPrice(item, true);

            if (price > Game1.player._money)
            {
                Game1.player._money = 0;
            }
            else
            {
                Game1.player._money -= price;
            }
        }

        public void SellItem(Item item)
        {
            Game1.player._money += GetItemPrice(item, true);
        }

        public int GetItemPrice(Item item, bool countStack = false)
        {
            item.Stack = item.Stack > 0 ? item.Stack : 1;

            var spawnableItem = GetSpawnableItem(item, out var key);
            var price = -1;

            if (_pricelist.ContainsKey(key))
            {
                price = _pricelist[key];
            }

            if (price < 0)
            {
                price = Utility.getSellToStorePriceOfItem(item, false);
            }

            if (price <= 0 && !_pricelist.ContainsKey(key))
            {
                price = spawnableItem.CategoryPrice;
            }

            if (countStack)
            {
                price *= item.Stack;
            }

            return price;
        }

        public void SetItemPrice(Item activeItem, int price)
        {
            var key = Helpers.GetItemUniqueKey(activeItem);

            if (price < 0 && _pricelist.ContainsKey(key))
            {
                _pricelist.Remove(key);
            }
            else
            {
                _pricelist[key] = price;
            }

            _helper.Data.WriteJsonFile($"price-config.json", _pricelist);
        }

        public SpawnableItem GetSpawnableItem(Item item, out string key)
        {
            key = Helpers.GetItemUniqueKey(item);

            if (!ItemRegistry.TryGetValue(key, out var spawnableItem))
            {
                _monitor.LogOnce(
                    $"Item with - name: {item.Name}, ID: {item.parentSheetIndex}, key: {key} is missing in register!",
                    LogLevel.Alert);
            }

            return spawnableItem;
        }
        
        public void DumpPricelist(){
        
        }
        
        public void LoadPricelist(){
        
        }
        
        public void DumpCategories(){
        
        }
        
        public void LoadCategories(){
        
        }

        #region Save/Load

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == _modManifest.UniqueID)
            {
                ModStateMessage modStateMessage;
                PricelistMessage pricelistMessage;
                CategoriesMessage categoriesMessage;
                string playerID;
                
                switch (e.Type)
                {
                    /*ModState messages*/
                    case "ModState:SaveRequired":
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        modStateMessage = e.ReadAs<ModStateMessage>();
                        SaveManager.Instance.CommitModState(modStateMessage.PlayerID, modStateMessage.ModState);
                        break;
                    case "ModState:LoadRequired":
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        playerID = e.ReadAs<string>();
                        modStateMessage = new ModStateMessage
                        {
                            ModState = SaveManager.Instance.GetModState(playerID),
                            PlayerID = playerID
                        };
                        _helper.Multiplayer.SendMessage(modStateMessage, "ModState:LoadAccepted",
                            new[] {_modManifest.UniqueID}, new[] {long.Parse(modStateMessage.PlayerID)});
                        break;
                    case "ModState:LoadAccepted":
                        modStateMessage = e.ReadAs<ModStateMessage>();
                        OnLoadState(modStateMessage.ModState);
                        break;
                    /*Pricelist messages*/
                    case "Pricelist:LoadRequired":
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        playerID = e.ReadAs<string>();
                        pricelistMessage = new PricelistMessage
                        {
                            Pricelist = SaveManager.Instance.GetPricelist(),
                            PlayerID = playerID
                        };
                        _helper.Multiplayer.SendMessage(pricelistMessage, "Pricelist:LoadAccepted",
                            new[] {_modManifest.UniqueID}, new[] {long.Parse(pricelistMessage.PlayerID)});
                        break;
                    case "Pricelist:LoadAccepted":
                        pricelistMessage = e.ReadAs<PricelistMessage>();
                        OnLoadPrices(pricelistMessage.Pricelist);
                        break;
                    /*Categories messages*/
                    case "Categories:LoadRequired":
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        playerID = e.ReadAs<string>();
                        categoriesMessage = new CategoriesMessage
                        {
                            Categories = SaveManager.Instance.GetCategories(),
                            PlayerID = playerID
                        };
                        _helper.Multiplayer.SendMessage(categoriesMessage, "Categories:LoadAccepted",
                            new[] {_modManifest.UniqueID}, new[] {long.Parse(categoriesMessage.PlayerID)});
                        break;
                    case "Categories:LoadAccepted":
                        categoriesMessage = e.ReadAs<CategoriesMessage>();
                        OnLoadCategories(categoriesMessage.Categories);
                        break;
                }
            }
        }

        private void OnSave(object sender, DayEndingEventArgs dayEndingEventArgs)
        {
            var state = new ModState
            {
                ActiveMode = ModMode,
                Quality = Quality,
                SortOption = SortOption,
                SearchText = SearchText,
                Category = Category
            };

            if (Context.IsMainPlayer)
            {
                SaveManager.Instance.CommitModState(Game1.player.uniqueMultiplayerID.ToString(), state);
            }
            else
            {
                var message = new ModStateMessage
                {
                    ModState = state,
                    PlayerID = Game1.player.uniqueMultiplayerID.ToString()
                };

                _helper.Multiplayer.SendMessage(message, "ModState:SaveRequired", new[] {_modManifest.UniqueID});
            }

            if (Context.IsMainPlayer)
            {
                SaveManager.Instance.CommitPricelist(_pricelist);
            }
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                OnLoadState(SaveManager.Instance.GetModState(Game1.player.uniqueMultiplayerID.ToString()));
                OnLoadPrices(SaveManager.Instance.GetPricelist());
                OnLoadCategories(SaveManager.Instance.GetCategories());
            }
            else
            {
                _helper.Multiplayer.SendMessage(Game1.player.uniqueMultiplayerID, "ModState:LoadRequired",
                    new[] {_modManifest.UniqueID});
                _helper.Multiplayer.SendMessage(Game1.player.uniqueMultiplayerID, "Pricelist:LoadRequired",
                    new[] {_modManifest.UniqueID});
                _helper.Multiplayer.SendMessage(Game1.player.uniqueMultiplayerID, "Categories:LoadRequired",
                    new[] {_modManifest.UniqueID});
            }
        }

        private void OnLoadState(ModState state)
        {
            ModMode = state.ActiveMode;
            Quality = state.Quality;
            SortOption = state.SortOption;
            SearchText = state.SearchText;
            Category = state.Category;
        }

        private void OnLoadPrices(Dictionary<string, int> pricelist)
        {
            _pricelist = pricelist;
        }
        
        private void OnLoadCategories(ModDataCategory[] categories)
        {
            _categories = categories;
            InitRegistry(GetSpawnableItems().ToArray());
        }

        #endregion
        
        private IEnumerable<SpawnableItem> GetSpawnableItems()
        {
            var items = new ItemRepository().GetAll();
        
            // if (_itemData?.ProblematicItems?.Any() == true)
            // {
            //     var problematicItems =
            //         new HashSet<string>(_itemData.ProblematicItems, StringComparer.OrdinalIgnoreCase);
            //
            //     items = items.Where(item => !problematicItems.Contains($"{item.Type}:{item.ID}"));
            // }
        
            foreach (var entry in items)
            {
                var category = _categories?.FirstOrDefault(rule => rule.IsMatch(entry));
                var label = category != null
                    ? I18n.GetByKey(category.Label).Default(category.Label)
                    : I18n.Category_Misc();
        
                yield return new SpawnableItem(entry, label ?? I18n.Category_Misc(), category?.BaseCost ?? 100,
                    category?.ResearchCount ?? 1);
            }
        }
    }
}