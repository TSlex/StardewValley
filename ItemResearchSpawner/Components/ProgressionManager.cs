using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Components
{
    internal class ProgressionManager
    {
        public static ProgressionManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly ModDataCategory[] _categories;
        private readonly SpawnableItem[] _items;

        private ResearchProgression _progression;
        
        public delegate void StackChanged(int newCount);

        public static event StackChanged OnStackChanged;

        public ProgressionManager(IMonitor monitor, IModHelper helper,
            ModDataCategory[] categories, SpawnableItem[] items)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;
            _categories = categories;
            _items = items;

            _helper.Events.GameLoop.Saving += OnSaveProgression;
            _helper.Events.GameLoop.DayStarted += OnLoadProgression;
        }

        public void ResearchItem(Item item)
        {
            var itemProgression = GetItemProgressionRaw(item);

            if (itemProgression.max <= 0 || itemProgression.current >= itemProgression.max)
            {
                return;
            }

            var needCount = itemProgression.max - itemProgression.current;

            var progressCount = item.Stack > needCount ? needCount : item.Stack;
                
            var progressionItem = TryInitAndReturnProgressionItem(item);

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            switch (itemQuality)
            {
                case ItemQuality.Silver:
                    progressionItem.ResearchCountSilver += progressCount;
                    break;
                case ItemQuality.Gold:
                    progressionItem.ResearchCountGold += progressCount;
                    break;
                case ItemQuality.Iridium:
                    progressionItem.ResearchCountIridium += progressCount;
                    break;
                default:
                    progressionItem.ResearchCount += progressCount;
                    break;
            }
            
            OnStackChanged?.Invoke(item.Stack - progressCount);

            SaveProgression();
        }

        public bool ItemResearched(Item item)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item);

            return maxProgression > 0 && itemProgression >= maxProgression;
        }

        public string GetItemProgression(Item item)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item);

            if (maxProgression <= 0)
            {
                return "???";
            }

            return $"({itemProgression} / {maxProgression})";
        }

        public (int current, int max) GetItemProgressionRaw(Item item)
        {
            var spawnableItem = GetSpawnableItem(item);

            if (spawnableItem == null)
            {
                return (0, 0);
            }

            var category = _categories.FirstOrDefault(c =>
                Helpers.EqualsCaseInsensitive(spawnableItem.Category, c.Label));

            var maxProgression = category?.ResearchCount ?? 1;

            var progressionItem = TryInitAndReturnProgressionItem(item);

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            var itemProgression = itemQuality switch
            {
                ItemQuality.Silver => progressionItem.ResearchCountSilver,
                ItemQuality.Gold => progressionItem.ResearchCountGold,
                ItemQuality.Iridium => progressionItem.ResearchCountIridium,
                _ => progressionItem.ResearchCount
            };

            itemProgression = (int) MathHelper.Clamp(itemProgression, 0, maxProgression);

            return (itemProgression, maxProgression);
        }

        private ResearchItem TryInitAndReturnProgressionItem(Item item)
        {
            var spawnableItem = GetSpawnableItem(item);

            var progressionItem = spawnableItem != null
                ? _progression.ResearchItems
                    .FirstOrDefault(ri => ri.ItemId.Equals(spawnableItem.ID))
                : null;

            if (progressionItem == null)
            {
                progressionItem = new ResearchItem
                {
                    ItemId = spawnableItem?.ID ?? -1
                };

                _progression.ResearchItems.Add(progressionItem);
            }

            return progressionItem;
        }

        private SpawnableItem GetSpawnableItem(Item item)
        {
            var spawnableItem = _items.FirstOrDefault(si => si.Name.Equals(item.Name));

            if (spawnableItem == null)
            {
                _monitor.LogOnce($"Item with ID: {item.parentSheetIndex} is missing in register!", LogLevel.Alert);
            }

            return spawnableItem;
        }

        private static string DirectoryName => $"{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}";

        private void OnSaveProgression(object sender, SavingEventArgs e)
        {
            SaveProgression();
        }

        private void OnLoadProgression(object sender, DayStartedEventArgs e)
        {
            LoadProgression();
        }

        private void SaveProgression()
        {
            _helper.Data.WriteJsonFile($"{DirectoryName}/progress.json", _progression);
            _monitor.Log("Progression saved! :)", LogLevel.Debug);
        }

        private void LoadProgression()
        {
            _progression = _helper.Data.ReadJsonFile<ResearchProgression>($"{DirectoryName}/progress.json") ??
                           new ResearchProgression();
            _monitor.Log("Progression loaded! :)", LogLevel.Debug);
        }
    }
}