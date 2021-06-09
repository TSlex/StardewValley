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
        private ModItemData _itemData;
        private ModDataCategory[] _categories;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _itemData = helper.Data.ReadJsonFile<ModItemData>("assets/item-data.json");
            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories.json");

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
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
            var items = GetSpawnableItems().ToArray();
            return new SpawnMenu(items, Helper.Content, Monitor);
        }
        
        private IEnumerable<SpawnableItem> GetSpawnableItems()
        {
            var items = new ItemRepository().GetAll();

            if (_itemData?.ProblematicItems?.Any() == true)
            {
                var problematicItems = new HashSet<string>(_itemData.ProblematicItems, StringComparer.OrdinalIgnoreCase);
                
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