using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Core {

    internal class ModCommand {
        public readonly string command;
        public readonly Func<string> description;
        public readonly Action<string, string[]> action;
        public readonly bool hidden;

        public ModCommand(string command, Func<string> description, Action<string, string[]> action, bool hidden = false) {
            this.command = command;
            this.description = description;
            this.action = action;
            this.hidden = hidden;
        }
    }

    internal class CommandManager {

        private IModHelper Helper => ModManager.Instance.Helper;
        private IMonitor Monitor => ModManager.Instance.Monitor;

        private Dictionary<string, ModCommand> Commands;

        public CommandManager() {
            Commands = new Dictionary<string, ModCommand> {
                { "rns_get_key", new ModCommand("rns_get_key", () => I18n.Command_UniqueKey_Desc(), GetUniqueKey) },
                { "rns_unlock_all", new ModCommand("rns_unlock_all", () => I18n.Command_UnlockAll_Desc(), UnlockAllProgression) },
                { "rns_unlock_active", new ModCommand("rns_unlock_active", () => I18n.Command_UnlockActive_Desc(), UnlockActiveProgression) },
                { "rns_dump_progression", new ModCommand("rns_dump_progression", () => I18n.Command_DumpProgressions_Desc(), DumpProgression) },
                { "rns_load_progression", new ModCommand("rns_load_progression", () => I18n.Command_LoadProgressions_Desc(), LoadProgression) },
                { "rns_jmt_rich", new ModCommand("rns_jmt_rich", () => I18n.Command_JMTRich_Desc(), JMTAdd100k) },
                { "rns_jmt_broke", new ModCommand("rns_jmt_broke", () => I18n.Command_JMTBroke_Desc(), JMTVoid) },
                { "rns_check_recipes", new ModCommand("rns_check_recipes", () => "", CheckRecipes, true) }
            };
        }

        // =====================================================================================

        public void HandleChatCommand(string message) {
            //ReplyToChat($"Got command: {message}");

            var splitted = message.Split(" ");
            string command = splitted[0];
            string[] args = splitted.Skip(1).ToArray();

            if (command == "rns_help") {
                var targetCommand = args.Length > 0 ? args[0] : "";

                if (targetCommand == "") {
                    foreach (var item in Commands) {
                        if (item.Value.hidden) {
                            continue;
                        }
                        GiveCommandDescription(item.Value);
                    }
                }
                else if (Commands.TryGetValue(targetCommand, out var modCommand)) {
                    GiveCommandDescription(modCommand);
                }
                else {
                    ReplyToChat(string.Format(I18n.Command_GetHelpFail(), targetCommand));
                }
            }

            else if (Commands.TryGetValue(command, out var modCommand)) {

                //ReplyToChat($"Got command -> {command} \nParameters were:");

                //foreach (var arg in args) {
                //    ReplyToChat($"{arg}");
                //}

                modCommand.action(command, args);
            }
        }

        public void GiveCommandDescription(ModCommand command) {
            ReplyToChat(string.Format(I18n.Command_GetHelp(), command.command, command.description()));
        }

        public void ReplyToChat(string message, int chatKind = 2, Color? color = null) {
            Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, message, Game1.player.UniqueMultiplayerID);
            ReceiveСhatModMessage(Game1.chatBox, 0, chatKind, LocalizedContentManager.CurrentLanguageCode, message, color: color);
        }

        private void ReceiveСhatModMessage(ChatBox chatBox, long sourceFarmer, int chatKind, LocalizedContentManager.LanguageCode language, string message, Color? color = null) {
            string text = message;
            ChatMessage chatMessage = new();

            //List<ChatMessage> messages = (List<ChatMessage>) typeof(ChatBox).GetField("messages", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(chatBox);
            var messages = Game1.chatBox.messages;

            //MethodInfo messageColor = typeof(ChatBox).GetMethod("messageColor", BindingFlags.NonPublic | BindingFlags.Instance);

            string text2 = Game1.parseText(text, chatBox.chatBox.Font, chatBox.chatBox.Width - 16);
            //var c = (Color) (color != null ? color : (Color) messageColor.Invoke(chatBox, new object[] { chatKind }));

            var c = (Color) (color != null ? color : chatBox.messageColor(chatKind));

            chatMessage.timeLeftToDisplay = 600;
            chatMessage.verticalSize = (int) chatBox.chatBox.Font.MeasureString(text2).Y + 4;
            chatMessage.color = c;
            chatMessage.language = language;

            chatMessage.message = new List<ChatSnippet>() {
                new("[RNS", language),
                new("", language) {
                    emojiIndex = 106,
                    myLength = 4 * 9
                },
                new("]: ", language),
                new(text2, language)
            };

            messages.Add(chatMessage);
            if (messages.Count > chatBox.maxMessages) {
                messages.RemoveAt(0);
            }
        }

        // =====================================================================================

        private void GetUniqueKey(string command, string[] args) {
            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null) {
                ReplyToChat(I18n.Command_UnlockActive_ErrNoItem());
            }
            else {
                //Monitor.Log($"{CommonHelper.GetItemUniqueKey(activeItem)}", LogLevel.Info);
                ReplyToChat(CommonHelper.GetItemUniqueKey(activeItem));
            }
        }

        private void UnlockAllProgression(string command, string[] args) {

            ModManager.ProgressionManagerInstance.UnlockAllProgression();

            //Monitor.Log($"All items were researched!", LogLevel.Info);
            ReplyToChat(I18n.Command_UnlockAll_Succ());
        }

        private void UnlockActiveProgression(string command, string[] args) {
            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null) {
                //Monitor.Log($"Select an item first", LogLevel.Info);
                ReplyToChat(I18n.Command_UnlockActive_ErrNoItem());
            }
            else {
                //_progressionManager.UnlockProgression(activeItem);
                ModManager.ProgressionManagerInstance.UnlockProgression(activeItem);

                //Monitor.Log($"Item - {activeItem.DisplayName}, was unlocked! ;)", LogLevel.Info);
                ReplyToChat(string.Format(I18n.Command_UnlockActive_Succ(), activeItem.DisplayName));
            }
        }

        private void DumpProgression(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;

            //if (Context.IsMultiplayer) {
            //    Monitor.Log($"Waiting until all clients response", LogLevel.Info);
            //}

            ModManager.ProgressionManagerInstance.DumpPlayersProgression();

            //if (!Context.IsMultiplayer) {
            //    Monitor.Log($"Player(s) progressions were dumped", LogLevel.Info);
            //}

            //SaveHelper.ProgressionDumpPath(player.UniqueMultiplayerID.ToString())

            //ReplyToChat(I18n.Command_DumpProgressions_Succ());
            ReplyToChat(string.Format(I18n.Command_DumpProgressions_Succ(), $"...mods/ItemResearchSpawner/{SaveHelper.DumpBasePath}/{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}"));
        }

        private void LoadProgression(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;

            ModManager.ProgressionManagerInstance.LoadPlayersProgression();

            //Monitor.Log($"Player(s) progressions was loaded", LogLevel.Info);
            ReplyToChat(I18n.Command_LoadProgressions_Succ());
        }

        private bool CheckIsHostPlayer() {
            if (!Context.IsMainPlayer) {
                //Monitor.Log($"This command is for host player only!", LogLevel.Info);
                ReplyToChat(I18n.Command_Multiplayer_HostOnly());
                return false;
            }

            return true;
        }

        private void JMTAdd100k(string command, string[] args) {
            ModManager.Instance.JMTMoney += 100000;

            ReplyToChat(I18n.Command_JMTRich_Succ());
        }

        private void JMTVoid(string command, string[] args) {
            ModManager.Instance.JMTMoney = 0;

            ReplyToChat(I18n.Command_JMTBroke_Succ());
        }

        private void CheckRecipes(string command, string[] args) {
            if (!CheckIsHostPlayer())
                return;

            Monitor.Log("Checking crafting prices logic...", LogLevel.Info);

            var craftingRecipes = CraftingRecipe.craftingRecipes.Select(p => new CraftingRecipe(p.Key, isCookingRecipe: false));

            foreach (var recipe in craftingRecipes) {
                var itemsNeeded = recipe.recipeList
                    .Select(ri => (
                    item: ModManager.ProgressionManagerInstance.GetProgressionItem(ItemRegistry.Create(ItemRegistry.GetDataOrErrorItem(ri.Key).QualifiedItemId, 1)),
                    amount: ri.Value));

                var craftingPrice = itemsNeeded.Sum(p => p.item.Price * p.amount);

                var recipeStr = string.Join(" ",
                    itemsNeeded.Select(p => $"{CommonHelper.GetItemUniqueKey(p.item.GameItem)} ({p.amount}) [{p.item.Price * p.amount}$]").ToList()
                    );

                var producedItems = recipe.itemToProduce;

                foreach (var producedItem in producedItems) {
                    var producedItemData = ItemRegistry.GetDataOrErrorItem(recipe.bigCraftable ? ItemRegistry.ManuallyQualifyItemId(producedItem, "(BC)") : producedItem);
                    var producedItemInstance = ItemRegistry.Create(producedItemData.QualifiedItemId, 1);
                    var producedItemPI = ModManager.ProgressionManagerInstance.GetProgressionItem(producedItemInstance);

                    var sellPrice = producedItemPI.Price * recipe.numberProducedPerCraft;

                    var logLevel = sellPrice > craftingPrice ? LogLevel.Error : LogLevel.Info;

                    Monitor.Log($"{CommonHelper.GetItemUniqueKey(producedItemPI.GameItem)} ({recipe.numberProducedPerCraft}) {sellPrice}$ | {craftingPrice}$ {recipeStr}", logLevel);
                }
            }

            Monitor.Log("Checking cooking prices logic...", LogLevel.Info);

            var cookingRecipes = CraftingRecipe.cookingRecipes.Select(p => new CraftingRecipe(p.Key, isCookingRecipe: true));

            foreach (var recipe in cookingRecipes) {
                var itemsNeeded = recipe.recipeList
                    .Select(ri => (
                    item: ModManager.ProgressionManagerInstance.GetProgressionItem(ItemRegistry.Create(ItemRegistry.GetDataOrErrorItem(ri.Key).QualifiedItemId, 1)),
                    amount: ri.Value));

                var craftingPrice = itemsNeeded.Sum(p => p.item.Price * p.amount);

                var recipeStr = string.Join(" ",
                    itemsNeeded.Select(p => $"{CommonHelper.GetItemUniqueKey(p.item.GameItem)} ({p.amount}) [{p.item.Price * p.amount}$]").ToList()
                    );

                var producedItems = recipe.itemToProduce;

                foreach (var producedItem in producedItems) {
                    var producedItemData = ItemRegistry.GetDataOrErrorItem(recipe.bigCraftable ? ItemRegistry.ManuallyQualifyItemId(producedItem, "(BC)") : producedItem);
                    var producedItemInstance = ItemRegistry.Create(producedItemData.QualifiedItemId, 1);
                    var producedItemPI = ModManager.ProgressionManagerInstance.GetProgressionItem(producedItemInstance);

                    var sellPrice = producedItemPI.Price * recipe.numberProducedPerCraft;

                    var logLevel = sellPrice > craftingPrice ? LogLevel.Error : LogLevel.Info;

                    Monitor.Log($"{CommonHelper.GetItemUniqueKey(producedItemPI.GameItem)} ({recipe.numberProducedPerCraft}) {sellPrice}$ | {craftingPrice}$ {recipeStr}", logLevel);
                }
            }

            Monitor.Log("Checking completed!", LogLevel.Info);
        }
    }
}
