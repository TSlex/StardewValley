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

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _config = helper.ReadConfig<ModConfig>();
            _itemData = helper.Data.ReadJsonFile<ModItemData>("assets/item-data.json");
            // _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories.json");
            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories-progress.json");
            
            _progressionManager ??= new ProgressionManager(Monitor, _helper);

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            
            helper.ConsoleCommands.Add("research_unlock_all", "unlock all items research progression", UnlockProgression);
        }

        private void UnlockProgression(string command, string[] args)
        {
            _progressionManager.UnlockProgression();
            Monitor.Log($"All researches were completed! :D", LogLevel.Info);
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
            _items = GetSpawnableItems().ToArray(); // some items exists only after day started ;_;
            _progressionManager.InitRegistry(_items);
            
            return new SpawnMenu(_items, Helper.Content, Monitor);
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

                yield return new SpawnableItem(entry, category?.Label ?? "Misc");
            }
        }
    }
}