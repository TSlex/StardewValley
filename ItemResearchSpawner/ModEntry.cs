using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Components;
using ItemResearchSpawner.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;
        private IModHelper _helper;
        private ModItemData _itemData;
        private ModDataCategory[] _categories;
        private SpawnableItem[] _items;

        private ProgressionManager _progressionManager;
        private ModManager _modManager;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _config = helper.ReadConfig<ModConfig>();
            _itemData = helper.Data.ReadJsonFile<ModItemData>("assets/item-data.json");
            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories-progress.json");

            _progressionManager ??= new ProgressionManager(Monitor, _helper);
            _modManager ??= new ModManager(Monitor, _helper, _config);

            I18n.Init(helper.Translation);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            helper.ConsoleCommands.Add("research_unlock_all", "unlock all items research progression",
                UnlockAllProgression);

            helper.ConsoleCommands.Add("research_unlock_active", "unlock currently selected item",
                UnlockActiveProgression);

            helper.ConsoleCommands.Add("research_set_mode", "change mode to \n 0 - Spawn Mode \n 1 - Buy/Sell Mode",
                SetMode);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _items = GetSpawnableItems().ToArray(); // some items exists only after day started ;_;
            _progressionManager.InitRegistry(_items);
        }

        private void UnlockAllProgression(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            _progressionManager.UnlockAllProgression();
            Monitor.Log($"All researches were completed! :D", LogLevel.Info);
        }

        private void UnlockActiveProgression(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                Monitor.Log($"Select an item to be unlocked", LogLevel.Info);
            }
            else
            {
                _progressionManager.UnlockProgression(activeItem);
                Monitor.Log($"Item - {activeItem.DisplayName}, was unlocked! ;)", LogLevel.Info);
            }
        }

        private void SetMode(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            try
            {
                _modManager.ModMode = (ModMode) int.Parse(args[0]);
                Monitor.Log($"Mode was changed to: {_modManager.ModMode.GetString()}", LogLevel.Info);
            }
            catch (Exception e)
            {
                Monitor.Log($"Available modes: \n 0 - Spawn Mode \n 1 - Buy/Sell Mode", LogLevel.Info);
            }
        }

        private bool CheckCommandInGame()
        {
            if (!Game1.hasLoadedGame)
            {
                Monitor.Log($"Use this command in-game", LogLevel.Info);
                return false;
            }

            return true;
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree)
            {
                return;
            }

            if (_config.ShowMenuKey.JustPressed())
            {
                Game1.activeClickableMenu = GetSpawnMenu();
            }
        }

        private IClickableMenu GetSpawnMenu()
        {
            return new SpawnMenu(_items, Helper.Content, _helper, Monitor);
        }

        private IEnumerable<SpawnableItem> GetSpawnableItems()
        {
            var items = new ItemRepository().GetAll();

            if (_itemData?.ProblematicItems?.Any() == true)
            {
                var problematicItems =
                    new HashSet<string>(_itemData.ProblematicItems, StringComparer.OrdinalIgnoreCase);

                items = items.Where(item => !problematicItems.Contains($"{item.Type}:{item.ID}"));
            }

            foreach (var entry in items)
            {
                var category = _categories?.FirstOrDefault(rule => rule.IsMatch(entry));
                var label = category != null
                    ? I18n.GetByKey(category.Label).Default(category.Label)
                    : I18n.Category_Misc();

                yield return new SpawnableItem(entry, label ?? I18n.Category_Misc());
            }
        }
    }
}