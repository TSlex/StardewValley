using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ItemResearchSpawner.Models;
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

        private readonly Dictionary<string, SpawnableItem> _itemRegistry =
            new Dictionary<string, SpawnableItem>();

        private Dictionary<string, ResearchProgression> _progression =
            new Dictionary<string, ResearchProgression>();

        public delegate void StackChanged(int newCount);

        public static event StackChanged OnStackChanged;

        public delegate void ResearchCompleted();

        public static event ResearchCompleted OnResearchCompleted;

        public ProgressionManager(IMonitor monitor, IModHelper helper)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;

            // _categories = helper.Data.ReadJsonFile<CategoryProgress[]>("assets/category-progress.json");
            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories.json");

            _helper.Events.GameLoop.Saving += OnSaveProgression;
            _helper.Events.GameLoop.DayStarted += OnLoadProgression;
        }

        public void InitRegistry(SpawnableItem[] items)
        {
            if (_itemRegistry.Count > 0)
            {
                return;
            }

            foreach (var spawnableItem in items)
            {
                var key = GetItemUniqueKey(spawnableItem.Item);

                _itemRegistry[key] = spawnableItem;
            }
        }

        private static string GetItemUniqueKey(Item item)
        {
            return $"{item.category}:" + $"{item.Name}:" + $"{item.ParentSheetIndex}";
        }

        public void ResearchItem(Item item)
        {
            var (itemProgression, progressionMax) = GetItemProgressionRaw(item);

            if (progressionMax <= 0 || itemProgression >= progressionMax)
            {
                return;
            }

            var needCount = progressionMax - itemProgression;

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

            if (item.Stack >= progressCount)
            {
                OnResearchCompleted?.Invoke();
            }
        }

        public bool ItemResearched(Item item)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item);

            return maxProgression > 0 && itemProgression >= maxProgression;
        }

        public string GetItemProgression(Item item, bool itemActive = false)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item, itemActive);

            if (maxProgression <= 0)
            {
                return "???";
            }

            return $"({itemProgression} / {maxProgression})";
        }

        private (int current, int max) GetItemProgressionRaw(Item item, bool itemActive = false)
        {
            var spawnableItem = GetSpawnableItem(item);

            return GetItemProgressionRaw(spawnableItem, itemActive);
        }

        private (int current, int max) GetItemProgressionRaw(SpawnableItem item, bool itemActive = false)
        {
            var category = _categories.FirstOrDefault(c => item.Category.Equals(c.Label));

            if (itemActive)
            {
                _monitor.Log($"Current item - name: {item.Name}, ID: {item.ID}, category: {item.Category}",
                    LogLevel.Alert);
            }

            var maxProgression = category?.ResearchCount ?? 1;

            var progressionItem = TryInitAndReturnProgressionItem(item.Item);

            var itemQuality = (ItemQuality) ((item.Item as Object)?.Quality ?? 0);

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

        public IEnumerable<SpawnableItem> GetResearchedItems()
        {
            return _itemRegistry.Values.Where(item =>
            {
                var progression = GetItemProgressionRaw(item);

                return progression.max > 0 && progression.current >= progression.max;
            });
        }

        private ResearchProgression TryInitAndReturnProgressionItem(Item item)
        {
            var key = GetItemUniqueKey(item);

            ResearchProgression progressionItem;

            if (_progression.ContainsKey(key))
            {
                progressionItem = _progression[key];
            }
            else
            {
                progressionItem = new ResearchProgression();
                _progression[key] = new ResearchProgression();
            }

            return progressionItem;
        }

        private SpawnableItem GetSpawnableItem(Item item)
        {
            if (!_itemRegistry.TryGetValue(GetItemUniqueKey(item), out var spawnableItem))
            {
                _monitor.LogOnce($"Item with  - name: {item.Name}, ID: {item.parentSheetIndex} is missing in register!",
                    LogLevel.Alert);
            }

            return spawnableItem;
        }

        private SpawnableItem GetSpawnableItem(string key)
        {
            if (!_itemRegistry.TryGetValue(key, out var spawnableItem))
            {
                _monitor.LogOnce($"Key: {key} is missing in register!",
                    LogLevel.Alert);
            }

            return spawnableItem;
        }

        #region SaveLoad

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
            _helper.Data.WriteJsonFile($"save/{DirectoryName}/progress.json", _progression);
            _monitor.Log("Progression saved! :)", LogLevel.Debug);
        }

        private void LoadProgression()
        {
            _progression =
                _helper.Data.ReadJsonFile<Dictionary<string, ResearchProgression>>(
                    $"save/{DirectoryName}/progress.json") ?? new Dictionary<string, ResearchProgression>();

            _monitor.Log("Progression loaded! :)", LogLevel.Debug);
        }

        #endregion
    }
}