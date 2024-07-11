

using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Data.Serializable;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using static ItemResearchSpawnerV2.Core.NetworkManager;

namespace ItemResearchSpawnerV2.Core {

    internal class NetworkManager {

        public interface IModMessage {
            string Type { get; }
        }

        public class OnLoadRequestedMessage : IModMessage {
            public string Type => "RNS_OnLoadRequested";
        }

        public class OnLoadSucceedMessage : IModMessage {
            public string Type => "RNS_OnLoadSucceed";

            public Dictionary<string, Dictionary<string, ItemSaveData>> Progressions;
            public Dictionary<string, ModManagerState> ModStates;
            public Dictionary<string, int> Pricelist;
            public List<string> ItemBlacklist;

            public ICollection<ItemCategoryMeta> Categories;
            public ItemCategoryMeta DefaultCategory;

            //public string ShowMenuButton;
        }

        public class OnCommitProgressionMessage : IModMessage {
            public string Type => "RNS_OnCommitProgression";

            internal Dictionary<string, ItemSaveData> CommitProgression;
        }

        public class OnReplaceProgressionMessage : IModMessage {
            public string Type => "RNS_OnReplaceProgression";

            internal Dictionary<string, ItemSaveData> CommitProgression;
        }

        public class OnCommitModStateMessage : IModMessage {
            public string Type => "RNS_OnCommitModState";

            public ModManagerState ModState;

            //public string ShowMenuButton;
        }

        public class OnHostConfigChangedMessage : IModMessage {
            public string Type => "RNS_OnHostConfigChanged";

            public ModConfig Config;
        }

        public class OnNonHostConfigChangedMessage : IModMessage {
            public string Type => "RNS_OnNonHostConfigChanged";

            public ModConfig Config;

            //public string ShowMenuButton;
        }

        //public class SerializableModState {
        //    public ModMode ActiveMode;
        //    public ItemQuality Quality;
        //    public string SearchText;
        //    public ItemSortOption SortOption;
        //    public string Category;
        //    public FavoriteDisplayMode FavoriteDisplayMode;
        //    public ProgressionDisplayMode ProgressionDisplayMode;
        //    public SerializableModConfig Config;

        //    public static SerializableModState FromModConfig(ModManagerState config) {
        //        return new SerializableModState {
        //            UseDefaultBalanceConfig = config.UseDefaultBalanceConfig,
        //            ShowMissingItems = config.ShowMissingItems,
        //            EnableSounds = config.EnableSounds,
        //            ResearchAmountMultiplier = config.ResearchAmountMultiplier,
        //            SellPriceMultiplier = config.SellPriceMultiplier,
        //            BuyPriceMultiplier = config.BuyPriceMultiplier,
        //            ResearchTimeSeconds = config.ResearchTimeSeconds,
        //            UseCustomUIColor = config.UseCustomUIColor,
        //            CustomUIColor = config.CustomUIColor,
        //            ShowMenuButton = config.ShowMenuButton.ToString()
        //        };
        //    }
        //}

        //public class SerializableModConfig {
        //    public bool UseDefaultBalanceConfig;
        //    public bool ShowMissingItems;
        //    public bool EnableSounds;
        //    public float ResearchAmountMultiplier;
        //    public float SellPriceMultiplier;
        //    public float BuyPriceMultiplier;
        //    public float ResearchTimeSeconds;
        //    public bool UseCustomUIColor;
        //    public Color CustomUIColor;
        //    public string ShowMenuButton;

        //    public static SerializableModConfig FromModConfig(ModConfig config) {
        //        return new SerializableModConfig {
        //            UseDefaultBalanceConfig = config.UseDefaultBalanceConfig,
        //            ShowMissingItems = config.ShowMissingItems,
        //            EnableSounds = config.EnableSounds,
        //            ResearchAmountMultiplier = config.ResearchAmountMultiplier,
        //            SellPriceMultiplier = config.SellPriceMultiplier,
        //            BuyPriceMultiplier = config.BuyPriceMultiplier,
        //            ResearchTimeSeconds = config.ResearchTimeSeconds,
        //            UseCustomUIColor = config.UseCustomUIColor,
        //            CustomUIColor = config.CustomUIColor,
        //            ShowMenuButton = config.ShowMenuButton.ToString()
        //        };
        //    }

        //    public ModConfig ToModConfig() {
        //        return new ModConfig {
        //            UseDefaultBalanceConfig = UseDefaultBalanceConfig,
        //            ShowMissingItems = ShowMissingItems,
        //            EnableSounds = EnableSounds,
        //            ResearchAmountMultiplier = ResearchAmountMultiplier,
        //            SellPriceMultiplier = SellPriceMultiplier,
        //            BuyPriceMultiplier = BuyPriceMultiplier,
        //            ResearchTimeSeconds = ResearchTimeSeconds,
        //            UseCustomUIColor = UseCustomUIColor,
        //            CustomUIColor = CustomUIColor,
        //            ShowMenuButton = KeybindList.Parse(ShowMenuButton)
        //        };
        //    }
        //}


        // ======================================================================

        public static void SendNetworkModMessage(IModMessage message, long? playerID = null) {
            ModManager.Instance.Helper.Multiplayer.SendMessage(message, message.Type,
                modIDs: new[] { ModManager.Instance.Manifest.UniqueID },
                playerIDs: playerID != null ? new long[] { (long) playerID } : null);
        }

        public static void RecieveNetworkModMessage(ModMessageReceivedEventArgs e) {
            if (e.FromModID == ModManager.Instance.Manifest.UniqueID) {
                switch (e.Type) {

                    case "RNS_OnLoadRequested":
                        OnLoadRequested(e.FromPlayerID);
                        break;

                    case "RNS_OnLoadSucceed":
                        OnLoadSucceed(e.ReadAs<OnLoadSucceedMessage>());
                        break;

                    case "RNS_OnCommitProgression":
                        OnCommitProgression(e.FromPlayerID, e.ReadAs<OnCommitProgressionMessage>());
                        break;

                    case "RNS_OnReplaceProgression":
                        OnReplaceProgression(e.ReadAs<OnReplaceProgressionMessage>());
                        break;

                    case "RNS_OnCommitModState":
                        OnCommitModState(e.FromPlayerID, e.ReadAs<OnCommitModStateMessage>());
                        break;

                    case "RNS_OnHostConfigChanged":
                        OnHostConfigChanged(e.ReadAs<OnHostConfigChangedMessage>());
                        break;

                    case "RNS_OnNonHostConfigChanged":
                        OnNonHostConfigChanged(e.FromPlayerID, e.ReadAs<OnNonHostConfigChangedMessage>());
                        break;

                    default:
                        break;
                }
            }
        }


        //===================== HOST ONLY =====================

        private static void OnNonHostConfigChanged(long fromPlayerID, OnNonHostConfigChangedMessage onNonHostConfigChangedMessage) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            var playerModState = ModManager.SaveManagerInstance.GetModState(fromPlayerID.ToString());
            var hostModState = ModManager.SaveManagerInstance.GetModState(Game1.player.UniqueMultiplayerID.ToString());

            playerModState.Config = onNonHostConfigChangedMessage.Config;
            //playerModState.Config.ShowMenuButton = KeybindList.Parse(onNonHostConfigChangedMessage.ShowMenuButton);

            playerModState.Config.DefaultMode = hostModState.Config.DefaultMode;
            playerModState.Config.ResearchAmountMultiplier = hostModState.Config.ResearchAmountMultiplier;
            playerModState.Config.SellPriceMultiplier = hostModState.Config.SellPriceMultiplier;
            playerModState.Config.BuyPriceMultiplier = hostModState.Config.BuyPriceMultiplier;
            playerModState.Config.ResearchTimeSeconds = hostModState.Config.ResearchTimeSeconds;

            ModManager.SaveManagerInstance.CommitModState(fromPlayerID.ToString(), playerModState);
        }

        private static void OnCommitProgression(long fromPlayerID, OnCommitProgressionMessage onCommitProgressionMessage) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            ModManager.SaveManagerInstance.CommitProgression(fromPlayerID.ToString(), onCommitProgressionMessage.CommitProgression, replace: false);
        }

        private static void OnCommitModState(long fromPlayerID, OnCommitModStateMessage onCommitModStateMessage) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            var playerModState = onCommitModStateMessage.ModState;
            var hostModState = ModManager.SaveManagerInstance.GetModState(Game1.player.UniqueMultiplayerID.ToString());

            //playerModState.Config.ShowMenuButton = KeybindList.Parse(onCommitModStateMessage.ShowMenuButton);

            playerModState.Config.DefaultMode = hostModState.Config.DefaultMode;
            playerModState.Config.ResearchAmountMultiplier = hostModState.Config.ResearchAmountMultiplier;
            playerModState.Config.SellPriceMultiplier = hostModState.Config.SellPriceMultiplier;
            playerModState.Config.BuyPriceMultiplier = hostModState.Config.BuyPriceMultiplier;
            playerModState.Config.ResearchTimeSeconds = hostModState.Config.ResearchTimeSeconds;

            ModManager.SaveManagerInstance.CommitModState(fromPlayerID.ToString(), playerModState);
        }

        private static void OnLoadRequested(long fromPlayerID) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            var playerModState = ModManager.SaveManagerInstance.GetModState(fromPlayerID.ToString());
            var hostModState = ModManager.SaveManagerInstance.GetModState(Game1.player.UniqueMultiplayerID.ToString());

            //var showMenuKey = playerModState.Config.ShowMenuButton.ToString();
            //playerModState.Config.ShowMenuButton = null;

            playerModState.Config.DefaultMode = hostModState.Config.DefaultMode;
            playerModState.Config.ResearchAmountMultiplier = hostModState.Config.ResearchAmountMultiplier;
            playerModState.Config.SellPriceMultiplier = hostModState.Config.SellPriceMultiplier;
            playerModState.Config.BuyPriceMultiplier = hostModState.Config.BuyPriceMultiplier;
            playerModState.Config.ResearchTimeSeconds = hostModState.Config.ResearchTimeSeconds;

            var reply = new OnLoadSucceedMessage() {
                Categories = ModManager.SaveManagerInstance.GetCategories(),
                DefaultCategory = ModManager.SaveManagerInstance.GetDefaultCategory(),
                ItemBlacklist = ModManager.SaveManagerInstance.GetBannedItems(),
                Pricelist = ModManager.SaveManagerInstance.GetPricelist(),

                ModStates = new Dictionary<string, ModManagerState>{
                    {fromPlayerID.ToString() , playerModState }
                },
                Progressions = new Dictionary<string, Dictionary<string, ItemSaveData>> {
                    {fromPlayerID.ToString() , ModManager.SaveManagerInstance.GetProgression(fromPlayerID.ToString()) }
                },
                //ShowMenuButton = showMenuKey
            };

            SendNetworkModMessage(reply, fromPlayerID);
        }

        // ===================== NON-HOST ONLY =====================

        private static void OnHostConfigChanged(OnHostConfigChangedMessage onHostConfigChangedMessage) {
            if (Context.IsMainPlayer) {
                return;
            }

            ModManager.Instance.Config.DefaultMode = onHostConfigChangedMessage.Config.DefaultMode;
            ModManager.Instance.Config.ResearchAmountMultiplier = onHostConfigChangedMessage.Config.ResearchAmountMultiplier;
            ModManager.Instance.Config.SellPriceMultiplier = onHostConfigChangedMessage.Config.SellPriceMultiplier;
            ModManager.Instance.Config.BuyPriceMultiplier = onHostConfigChangedMessage.Config.BuyPriceMultiplier;
            ModManager.Instance.Config.ResearchTimeSeconds = onHostConfigChangedMessage.Config.ResearchTimeSeconds;

            Game1.activeClickableMenu = null;
        }

        private static void OnReplaceProgression(OnReplaceProgressionMessage onCommitProgressionMessage) {
            if (Context.IsMainPlayer) {
                return;
            }

            ModManager.SaveManagerInstance.CommitProgression(Game1.player.UniqueMultiplayerID.ToString(), onCommitProgressionMessage.CommitProgression, replace: true);

            Game1.activeClickableMenu = null;
            ModManager.ProgressionManagerInstance.ResearchProgressions = ModManager.SaveManagerInstance.GetProgression(Game1.player.UniqueMultiplayerID.ToString());
        }

        private static void OnLoadSucceed(OnLoadSucceedMessage onLoadSucceedMessage) {
            if (Context.IsMainPlayer) {
                return;
            }

            ModManager.SaveManagerInstance.OnRemoteLoadSucceed(
                    onLoadSucceedMessage.Categories,
                    onLoadSucceedMessage.DefaultCategory,
                    onLoadSucceedMessage.ItemBlacklist,
                    onLoadSucceedMessage.Pricelist,
                    onLoadSucceedMessage.ModStates,
                    onLoadSucceedMessage.Progressions
                );
        }
    }
}
