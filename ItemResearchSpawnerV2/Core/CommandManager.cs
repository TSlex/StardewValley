using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StardewValley.BellsAndWhistles.PlayerStatusList;

namespace ItemResearchSpawnerV2.Core {
    internal class CommandManager {

        private IModHelper Helper => ModManager.Instance.Helper;
        private IMonitor Monitor => ModManager.Instance.Monitor;

        public CommandManager() {
            Helper.ConsoleCommands.Add("rns_unlock_all", I18n.Command_UnlockAll_Desc(), UnlockAllProgression);
            Helper.ConsoleCommands.Add("rns_unlock_active", I18n.Command_UnlockActive_Desc(), UnlockActiveProgression);
            Helper.ConsoleCommands.Add("rns_set_mode", I18n.Command_SetMode_Desc(), SetMode);
            Helper.ConsoleCommands.Add("rns_set_price", I18n.Command_SetPrice_Desc(), SetPrice);
            Helper.ConsoleCommands.Add("rns_reset_price", I18n.Command_ResetPrice_Desc(), ResetPrice);

            Helper.ConsoleCommands.Add("rns_get_key", I18n.Command_SetMode_Desc(), GetUniqueKey);
            Helper.ConsoleCommands.Add("rns_dump_progression", I18n.Command_SetMode_Desc(), DumpProgression);
            Helper.ConsoleCommands.Add("rns_load_progression", I18n.Command_SetMode_Desc(), LoadProgression);
            Helper.ConsoleCommands.Add("rns_dump_pricelist", I18n.Command_SetMode_Desc(), DumpPricelist);
            Helper.ConsoleCommands.Add("rns_load_pricelist", I18n.Command_SetMode_Desc(), LoadPricelist);
            Helper.ConsoleCommands.Add("rns_dump_categories", I18n.Command_SetMode_Desc(), DumpCategories);
            Helper.ConsoleCommands.Add("rns_load_categories", I18n.Command_SetMode_Desc(), LoadCategories);
        }

        private void UnlockAllProgression(string command, string[] args) {
            if (!CheckCommandInGame())
                return;

            //ProgressionManager.Instance.UnlockAllProgression();

            Monitor.Log($"All items were researched!", LogLevel.Info);
        }

        private void UnlockActiveProgression(string command, string[] args) {
            if (!CheckCommandInGame())
                return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null) {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else {
                //_progressionManager.UnlockProgression(activeItem);
                Monitor.Log($"Item - {activeItem.DisplayName}, was unlocked! ;)", LogLevel.Info);
            }
        }

        private void SetMode(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;

            try {
                //_modManager.ModMode = (ModMode)int.Parse(args[0]);
                Monitor.Log($"Mode was changed to: {ModManager.Instance.ModMode.GetString()}", LogLevel.Info);
            }
            catch (Exception) {
                Monitor.Log($"Available modes: \n 0 - Research (Spawn) mode \n 1 - Buy/Sell mode \n 2 - Combined (Research->Sell/Buy) mode", LogLevel.Info);
            }
        }

        private void SetPrice(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;
            if (!CheckIsForceDefaults())
                return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null) {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else {
                try {
                    var price = int.Parse(args[0]);

                    if (price < 0) {
                        Monitor.Log($"Price must be a non-negative number", LogLevel.Info);
                    }
                    else {
                        //_modManager.SetItemPrice(activeItem, price);
                        Monitor.Log($"Price for {activeItem.DisplayName}, was changed to: {price}! ;)", LogLevel.Info);
                    }
                }
                catch (Exception) {
                    Monitor.Log($"Price must be a correct non-negative number", LogLevel.Info);
                }
            }
        }

        private void ResetPrice(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;
            if (!CheckIsForceDefaults())
                return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null) {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else {
                //_modManager.SetItemPrice(activeItem, -1);
                Monitor.Log($"Price for {activeItem.DisplayName}, was reset! ;)", LogLevel.Info);
            }
        }

        private void GetUniqueKey(string command, string[] args) {
            if (!CheckCommandInGame())
                return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null) {
                Monitor.Log($"Select an item first", LogLevel.Info);
            }
            else {
                Monitor.Log($"{CommonHelper.GetItemUniqueKey(activeItem)}", LogLevel.Info);
            }
        }

        private void DumpProgression(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;

            if (Context.IsMultiplayer) {
                Monitor.Log($"Waiting until all clients response", LogLevel.Info);
            }

            ModManager.ProgressionManagerInstance.DumpPlayersProgression();

            if (!Context.IsMultiplayer) {
                Monitor.Log($"Player(s) progressions were dumped", LogLevel.Info);
            }
        }

        private void LoadProgression(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;

            //ProgressionManager.Instance.LoadPlayersProgression();

            Monitor.Log($"Player(s) progressions was loaded", LogLevel.Info);
        }

        private void DumpPricelist(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;
            //ModManager.Instance.DumpPricelist();

            Monitor.Log($"Pricelist was dumped to {SaveHelper.PricelistDumpPath}", LogLevel.Info);
        }

        private void LoadPricelist(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;
            if (!CheckIsForceDefaults())
                return;
            //ModManager.Instance.LoadPricelist();

            Monitor.Log($"Pricelist was loaded", LogLevel.Info);
        }

        private void DumpCategories(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;
            //ModManager.Instance.DumpCategories();

            Monitor.Log($"Categories were dumped to {SaveHelper.CategoriesDumpPath}", LogLevel.Info);
        }

        private void LoadCategories(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;
            if (!CheckIsForceDefaults())
                return;

            //ModManager.Instance.LoadCategories();

            Monitor.Log($"Categories were loaded", LogLevel.Info);
        }

        private bool CheckCommandInGame() {
            if (!Game1.hasLoadedGame) {
                Monitor.Log($"Use this command in-game only!", LogLevel.Info);
                return false;
            }

            return true;
        }

        private bool CheckIsHostPlayer() {
            if (CheckCommandInGame() && !Context.IsMainPlayer) {
                Monitor.Log($"This command is for host player only!", LogLevel.Info);
                return false;
            }

            return true;
        }

        private bool CheckIsForceDefaults() {
            if (Helper.ReadConfig<ModConfig>().GetUseDefaultBalanceConfig()) {
                Monitor.Log(
                    $"Currently default config is used for prices and categories! You can turn this off in config, to be able to manually change :)",
                    LogLevel.Warn);
                return false;
            }

            return true;
        }
    }
}
