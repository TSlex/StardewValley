using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
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

            _categories = helper.Data.ReadJsonFile<ModDataCategory[]>("assets/categories-progress.json");

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
                var key = Utils.Helpers.GetItemUniqueKey(spawnableItem.Item);

                _itemRegistry[key] = spawnableItem;
            }
        }

        public void ResearchItem(Item item)
        {
            var (itemProgression, progressionMax) = GetItemProgressionRaw(item, out var progressionItem);

            if (progressionMax <= 0 || itemProgression >= progressionMax)
            {
                return;
            }

            var needCount = progressionMax - itemProgression;

            var progressCount = item.Stack > needCount ? needCount : item.Stack;

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            if (itemQuality >= ItemQuality.Normal)
            {
                progressionItem.ResearchCount += progressCount;
            }

            if (itemQuality >= ItemQuality.Silver)
            {
                progressionItem.ResearchCountSilver += progressCount;
            }

            if (itemQuality >= ItemQuality.Gold)
            {
                progressionItem.ResearchCountGold += progressCount;
            }

            if (itemQuality >= ItemQuality.Iridium)
            {
                progressionItem.ResearchCountIridium += progressCount;
            }

            OnStackChanged?.Invoke(item.Stack - progressCount);

            if (item.Stack >= progressCount)
            {
                OnResearchCompleted?.Invoke();
            }
        }

        public bool ItemResearched(Item item)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item, out _);

            return maxProgression > 0 && itemProgression >= maxProgression;
        }
        
        public void UnlockProgression()
        {
            var progression = _progression.DeepClone();

            foreach (var key in _progression.Keys)
            {
                var temp = progression[key];

                temp.ResearchCount = 999;
                temp.ResearchCountSilver = 999;
                temp.ResearchCountGold = 999;
                temp.ResearchCountIridium = 999;

                progression[key] = temp;
            }

            _progression = progression;
        }

        public string GetItemProgression(Item item, bool itemActive = false)
        {
            var (itemProgression, maxProgression) = GetItemProgressionRaw(item, out _, itemActive);

            if (maxProgression <= 0)
            {
                return "???";
            }

            return $"({itemProgression} / {maxProgression})";
        }

        private (int current, int max) GetItemProgressionRaw(Item item, 
            out ResearchProgression progressionItem, bool itemActive = false)
        {
            var spawnableItem = GetSpawnableItem(item);

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);
            
            return GetItemProgressionRaw(spawnableItem, out progressionItem, itemQuality, itemActive);
        }

        private (int current, int max) GetItemProgressionRaw(SpawnableItem item,
            out ResearchProgression progressionItem, ItemQuality quality = ItemQuality.Normal, bool itemActive = false)
        {
            var category = _categories.FirstOrDefault(c => item.Category.Equals(c.Label));

            if (itemActive)
            {
                _monitor.Log($"Current item - name: {item.Name}, ID: {item.ID}, category: {item.Category}",
                    LogLevel.Alert);
                _monitor.Log($"Unique key: {Utils.Helpers.GetItemUniqueKey(item.Item)}",
                    LogLevel.Alert);
                
                // _monitor.Log($"\"{Utils.Helpers.GetItemUniqueKey(item.Item)}\",",
                // LogLevel.Alert);
            }

            var maxProgression = category?.ResearchCount ?? 1;

            progressionItem = TryInitAndReturnProgressionItem(item.Item);

            var itemProgression = quality switch
            {
                ItemQuality.Silver => progressionItem.ResearchCountSilver,
                ItemQuality.Gold => progressionItem.ResearchCountGold,
                ItemQuality.Iridium => progressionItem.ResearchCountIridium,
                _ => progressionItem.ResearchCount
            };

            itemProgression = (int) MathHelper.Clamp(itemProgression, 0, maxProgression);

            return (itemProgression, maxProgression);
        }

        public IEnumerable<ResearchedItem> GetResearchedItems()
        {
            return _itemRegistry.Values
                .Select(item => new ResearchedItem
                {
                    Item = item,
                    NeededProgression = GetItemProgressionRaw(item, out var progression).max,
                    Progression = progression
                })
                .Where(item => item.Progression.ResearchCount >= item.NeededProgression && item.NeededProgression > 0);
        }

        private ResearchProgression TryInitAndReturnProgressionItem(Item item)
        {
            var key = Utils.Helpers.GetItemUniqueKey(item);

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
            var key = Utils.Helpers.GetItemUniqueKey(item);
            
            if (!_itemRegistry.TryGetValue(key, out var spawnableItem))
            {
                _monitor.LogOnce($"Item with - name: {item.Name}, ID: {item.parentSheetIndex}, key: {key} is missing in register!",
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