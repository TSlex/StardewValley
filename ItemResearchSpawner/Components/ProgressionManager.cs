using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Models.Messages;
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
        private readonly IManifest _modManifest;

        private Dictionary<string, ResearchProgression> _progression = new Dictionary<string, ResearchProgression>();

        public delegate void StackChanged(int newCount);

        public static event StackChanged OnStackChanged;

        public ProgressionManager(IMonitor monitor, IModHelper helper, IManifest modManifest)
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

        public void ResearchItem(Item item)
        {
            var itemProgressionRaw = GetItemProgressionRaw(item, out var progressionItem);

            if (itemProgressionRaw.max < 0)
            {
                return;
            }
            
            if (itemProgressionRaw.current >= itemProgressionRaw.max)
            {
                if (ModManager.Instance.ModMode == ModMode.Buy)
                {
                    OnStackChanged?.Invoke(0);
                }
            
                return;
            }

            var needCount = itemProgressionRaw.max - itemProgressionRaw.current;

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
            
            if (item.Stack >= progressCount)
            {
                OnResearchCompleted();
            }

            if (ModManager.Instance.ModMode == ModMode.Buy)
            {
                OnStackChanged?.Invoke(0);
            }
            else
            {
                OnStackChanged?.Invoke(item.Stack - progressCount);
            }
        }

        public bool ItemResearched(Item item)
        {
            var itemProgressionRaw = GetItemProgressionRaw(item, out _);

            return itemProgressionRaw.max > 0 && itemProgressionRaw.current >= itemProgressionRaw.max;
        }

        public void UnlockAllProgression()
        {
            foreach (var item in ModManager.Instance.ItemRegistry.Values)
            {
                var progression = TryInitAndReturnProgressionItem(item.Item);

                progression.ResearchCount = 999;

                if (item.Item is Object)
                {
                    progression.ResearchCountSilver = 999;
                    progression.ResearchCountGold = 999;
                    progression.ResearchCountIridium = 999;
                }

                OnResearchCompleted();
            }
        }

        public void UnlockProgression(Item activeItem)
        {
            var progression = TryInitAndReturnProgressionItem(activeItem);

            progression.ResearchCount = 999;

            if (activeItem is Object)
            {
                progression.ResearchCountSilver = 999;
                progression.ResearchCountGold = 999;
                progression.ResearchCountIridium = 999;
            }

            OnResearchCompleted();
        }

        public IEnumerable<ResearchableItem> GetResearchedItems()
        {
            return ModManager.Instance.ItemRegistry.Values
                .Select(i => new ResearchableItem
                {
                    Item = i,
                    Progression = TryInitAndReturnProgressionItem(i.Item)
                })
                .Where(i =>
                    i.Progression.ResearchCount >= i.Item.ProgressionLimit && i.Item.ProgressionLimit > 0);
        }

        public string GetItemProgression(Item item, bool itemActive = false)
        {
            var itemProgressionRaw = GetItemProgressionRaw(item, out _, itemActive);

            if (itemProgressionRaw.max <= 0)
            {
                return "(X)";
            }
            
            if (ModManager.Instance.ModMode == ModMode.Buy)
            {
                return "($$$)";
            }

            return $"({itemProgressionRaw.current} / {itemProgressionRaw.max})";
        }

        private static void OnResearchCompleted()
        {
            ModManager.Instance.RequestMenuUpdate(true);
        }

        private ItemProgressionRaw GetItemProgressionRaw(Item item,
            out ResearchProgression progressionItem, bool itemActive = false)
        {
            var spawnableItem = ModManager.Instance.GetSpawnableItem(item, out _);

            if (spawnableItem == null)
            {
                progressionItem = null;
                
                return new ItemProgressionRaw
                {
                    current = -1,
                    max = -1
                };
            }

            var itemQuality = (ItemQuality) ((item as Object)?.Quality ?? 0);

            return GetItemProgressionRaw(spawnableItem, out progressionItem, itemQuality, itemActive);
        }

        private ItemProgressionRaw GetItemProgressionRaw(SpawnableItem item,
            out ResearchProgression progressionItem, ItemQuality quality = ItemQuality.Normal, bool itemActive = false)
        {
            var category =
                ModManager.Instance.AvailableCategories.FirstOrDefault(c =>
                    I18n.GetByKey(c.Label).ToString().Equals(item.Category));

            if (itemActive)
            {
                _monitor.Log($"Current item - name: {item.Name}, ID: {item.ID}, category: {item.Category}",
                    LogLevel.Alert);
                _monitor.Log($"Unique key: {Helpers.GetItemUniqueKey(item.Item)}",
                    LogLevel.Alert);
            }

            var maxProgression = ModManager.Instance.ModMode switch
            {
                ModMode.Buy => 1,
                _ => category?.ResearchCount ?? 1
            };

            progressionItem = TryInitAndReturnProgressionItem(item.Item);

            var itemProgression = quality switch
            {
                ItemQuality.Silver => progressionItem.ResearchCountSilver,
                ItemQuality.Gold => progressionItem.ResearchCountGold,
                ItemQuality.Iridium => progressionItem.ResearchCountIridium,
                _ => progressionItem.ResearchCount
            };

            itemProgression = (int) MathHelper.Clamp(itemProgression, 0, maxProgression);

            return new ItemProgressionRaw
            {
                current = itemProgression,
                max = maxProgression
            };
        }

        private ResearchProgression TryInitAndReturnProgressionItem(Item item)
        {
            var key = Helpers.GetItemUniqueKey(item);

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

        public void DumpPlayersProgression()
        {
            var progressions = SaveManager.Instance.GetProgressions();

            var onlinePlayers = Game1.getOnlineFarmers()
                .Where(farmer => progressions.Keys.Contains(farmer.uniqueMultiplayerID.ToString()))
                .ToDictionary(farmer => farmer.uniqueMultiplayerID.ToString());

            var offlinePlayers = Game1.getAllFarmers()
                .Where(farmer => !onlinePlayers.Keys.Contains(farmer.UniqueMultiplayerID.ToString()))
                .ToDictionary(farmer => farmer.uniqueMultiplayerID.ToString());

            if (!Context.IsMultiplayer && Context.IsMainPlayer)
            {
                _monitor.Log(
                    $"Dumping progression - player: {Game1.player.name}, location: {SaveHelper.ProgressionDumpPath(Game1.player.uniqueMultiplayerID.ToString())}",
                    LogLevel.Info);

                _helper.Data.WriteJsonFile(SaveHelper.ProgressionDumpPath(Game1.player.uniqueMultiplayerID.ToString()),
                    _progression);
            }

            _helper.Multiplayer.SendMessage("", MessageKeys.PROGRESSION_DUMP_REQUIRED,
                new[] {_modManifest.UniqueID});

            foreach (var player in offlinePlayers)
            {
                _monitor.Log(
                    $"Dumping progression - player: {player.Value.name}, location: {SaveHelper.ProgressionDumpPath(player.Key)}",
                    LogLevel.Info);

                _helper.Data.WriteJsonFile(SaveHelper.ProgressionDumpPath(player.Key),
                    progressions.ContainsKey(player.Key)
                        ? progressions[player.Key]
                        : new Dictionary<string, ResearchProgression>());
            }
        }

        public void LoadPlayersProgression()
        {
            var progressions = SaveManager.Instance.GetProgressions();
            var progressToLoad = new Dictionary<string, Dictionary<string, ResearchProgression>>();

            foreach (var playerID in progressions.Keys)
            {
                var playerData = _helper.Data.ReadJsonFile<Dictionary<string, ResearchProgression>>(
                    SaveHelper.ProgressionDumpPath(playerID));

                if (playerData != null)
                {
                    progressToLoad[playerID] = playerData;
                }
                else
                {
                    progressToLoad[playerID] = progressions[playerID];
                }
            }

            SaveManager.Instance.LoadProgressions(progressToLoad);

            if (Context.IsMultiplayer)
            {
                _helper.Multiplayer.SendMessage("", MessageKeys.PROGRESSION_MANAGER_SYNC,
                    new[] {_modManifest.UniqueID});
            }
            else
            {
                OnLoad(null, null);
            }
        }

        #region SaveLoad

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == _modManifest.UniqueID)
            {
                ResearchProgressionMessage message;
                switch (e.Type)
                {
                    case MessageKeys.PROGRESSION_SAVE_REQUIRED:
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        message = e.ReadAs<ResearchProgressionMessage>();
                        SaveManager.Instance.CommitProgression(message.PlayerID, message.Progression);
                        break;

                    case MessageKeys.PROGRESSION_LOAD_REQUIRED:
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        var playerID = e.ReadAs<string>();
                        message = new ResearchProgressionMessage
                        {
                            Progression = SaveManager.Instance.GetProgression(playerID),
                            PlayerID = playerID
                        };
                        _helper.Multiplayer.SendMessage(message, MessageKeys.PROGRESSION_LOAD_ACCEPTED,
                            new[] {_modManifest.UniqueID}, new[] {long.Parse(message.PlayerID)});
                        break;

                    case MessageKeys.PROGRESSION_LOAD_ACCEPTED:
                        message = e.ReadAs<ResearchProgressionMessage>();
                        OnLoadProgression(message.Progression);
                        break;

                    case MessageKeys.PROGRESSION_DUMP_REQUIRED:
                        if (Context.IsMainPlayer)
                        {
                            _monitor.Log(
                                $"Dumping progression - player: {Game1.player.name}, " +
                                $"location: {SaveHelper.ProgressionDumpPath(Game1.player.uniqueMultiplayerID.ToString())}",
                                LogLevel.Info);

                            _helper.Data.WriteJsonFile(
                                SaveHelper.ProgressionDumpPath(Game1.player.uniqueMultiplayerID.ToString()),
                                _progression);
                        }
                        else
                        {
                            message = new ResearchProgressionMessage
                            {
                                Progression = _progression,
                                PlayerID = Game1.player.UniqueMultiplayerID.ToString()
                            };

                            _helper.Multiplayer.SendMessage(message, MessageKeys.PROGRESSION_DUMP_ACCEPTED,
                                new[] {_modManifest.UniqueID});
                        }

                        break;

                    case MessageKeys.PROGRESSION_DUMP_ACCEPTED:
                        if (!Context.IsMainPlayer)
                        {
                            break;
                        }

                        message = e.ReadAs<ResearchProgressionMessage>();
                        var farmer = Game1.getAllFarmers()
                            .FirstOrDefault(f => f.UniqueMultiplayerID.ToString().Equals(message.PlayerID));

                        _monitor.Log(
                            $"Dumping progression - player: {farmer?.name ?? "???"}, " +
                            $"location: {SaveHelper.ProgressionDumpPath(message.PlayerID)}",
                            LogLevel.Info);

                        _helper.Data.WriteJsonFile(SaveHelper.ProgressionDumpPath(message.PlayerID),
                            message.Progression);
                        break;

                    case MessageKeys.PROGRESSION_MANAGER_SYNC:
                        OnLoad(null, null);
                        break;
                }
            }
        }

        private void OnSave(object sender, DayEndingEventArgs dayEndingEventArgs)
        {
            if (Context.IsMainPlayer)
            {
                SaveManager.Instance.CommitProgression(Game1.player.uniqueMultiplayerID.ToString(), _progression);
            }
            else
            {
                var message = new ResearchProgressionMessage()
                {
                    Progression = _progression,
                    PlayerID = Game1.player.uniqueMultiplayerID.ToString()
                };

                _helper.Multiplayer.SendMessage(message, MessageKeys.PROGRESSION_SAVE_REQUIRED,
                    new[] {_modManifest.UniqueID});
            }
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                var progression =
                    SaveManager.Instance.GetProgression(Game1.player.uniqueMultiplayerID.ToString());

                OnLoadProgression(progression);
            }
            else
            {
                _helper.Multiplayer.SendMessage(Game1.player.uniqueMultiplayerID, MessageKeys.PROGRESSION_LOAD_REQUIRED,
                    new[] {_modManifest.UniqueID});
            }
        }

        private void OnLoadProgression(Dictionary<string, ResearchProgression> progression)
        {
            _progression = progression;
            ModManager.Instance.RequestMenuUpdate(true);
        }

        #endregion
    }
}