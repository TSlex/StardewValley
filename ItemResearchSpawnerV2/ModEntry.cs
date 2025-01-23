using ItemResearchSpawnerV2.Api;
using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;


namespace ItemResearchSpawnerV2 {

    public class ModEntry : Mod {

        internal static ModEntry Instance;

        internal ModConfig Config;
        internal ModManager Manager;
        internal IModHelper Helper;

        internal bool IsSaveActive = false;

        internal ModConfig ActiveConfig => IsSaveActive ? Manager.Config : Config;

        public override void Entry(IModHelper helper) {

            Instance ??= this;

            // -----------------------------------------------

            I18n.Init(helper.Translation);

            // -----------------------------------------------

            Helper = helper;
            ReadConfig();

            // -----------------------------------------------

            Manager = new ModManager(helper, Config, Monitor, ModManifest);

            // -----------------------------------------------

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.Saving += OnSave;
            helper.Events.GameLoop.SaveLoaded += OnLoad;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        // =======================================================================================================

        private void ReadConfig() {
            try {
                Config = Helper.ReadConfig<ModConfig>();

                // fixing whatever values come from .json
                Config.SetResearchAmountMultiplier(Config.ResearchAmountMultiplier);
                Config.SetSellPriceMultiplier(Config.SellPriceMultiplier);
                Config.SetBuyPriceMultiplier(Config.BuyPriceMultiplier);
                Config.SetResearchTimeSeconds(Config.ResearchTimeSeconds);
            }
            catch (Exception e) {
                Config = new ModConfig();

                Helper.WriteConfig(Config);
                Monitor.LogOnce("Failed to load config.json, replaced with default one");
            }
        }

        private void HandleChatMessage(TextBox sender) {
            //var messages = (List<ChatMessage>)typeof(ChatBox).GetField("messages", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.chatBox);
            var messages = Game1.chatBox.messages;

            if (messages.Count <= 0) {
                return;
            }

            var lastMessage = messages.Select((m, i) => (message: m.message.FirstOrDefault()?.message ?? "", index: i)).Last();
            var formattedMessage = lastMessage.message.Trim().Split(":").Last().Trim();

            if (formattedMessage.StartsWith("!rns")) {
                ModManager.CommandManagerInstance.HandleChatCommand(formattedMessage.Replace("!rns", "rns"));
            }

        }


        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {
            NetworkManager.RecieveNetworkModMessage(e);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (e.IsMultipleOf(15)) { // ~every 1/4 of a second
                Manager.SyncJTMMoney();
            }
        }

        public void OnConfigChange() {
            if (Manager.SaveDataLoaded) {
                // Parcing fix
                //var c = Manager.Config.DeepClone();
                //c.ShowMenuButton = null;

                if (Context.IsMainPlayer) {
                    NetworkManager.SendNetworkModMessage(new NetworkManager.OnHostConfigChangedMessage() {
                        Config = Manager.Config,
                    });
                }
                else {
                    NetworkManager.SendNetworkModMessage(new NetworkManager.OnNonHostConfigChangedMessage() {
                        Config = Manager.Config,
                        //ShowMenuButton = Manager.Config.ShowMenuButton.ToString()
                    });
                }
            }
        }

        public void ResetConfig() {

            if (IsSaveActive) {
                if (!Manager.SaveDataLoaded) {
                    return;
                }
                Manager.Config = new ModConfig();
            }

            else {
                Config = new ModConfig();
            }

            OnConfigChange();
        }

        public void SaveConfig() {
            if (!IsSaveActive) {
                Helper.WriteConfig(Config);
            }

            OnConfigChange();
        }

        // =======================================================================================================

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            InitConfigMenu();
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e) {
            ReadConfig();

            Manager.Config = Config;
            IsSaveActive = false;

            if (Game1.chatBox != null) {
                Game1.chatBox.chatBox.OnEnterPressed -= HandleChatMessage;
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            IsSaveActive = true;
        }

        private void OnSave(object sender, SavingEventArgs saveLoadedEventArgs) {
            //if (!Context.IsMainPlayer) {
            //    return;
            //}

            Manager.OnSave();
        }

        private void OnLoad(object sender, SaveLoadedEventArgs saveLoadedEventArgs) {
            IsSaveActive = true;

            Manager.OnLoad();

            if (Game1.chatBox != null) {
                Game1.chatBox.chatBox.OnEnterPressed += HandleChatMessage;
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !Context.CanPlayerMove)
                return;

            //if (Game1.options.chatButton.Any(k => KeybindList.ForSingle(k.ToSButton()).JustPressed())){
            //}

            //if (KeybindList.ForSingle(SButton.Enter).JustPressed()) {
            //    Monitor.Log(Game1.chatBox.chatBox.Text);
            //}

            if (Game1.player.ActiveItem != null) {
                Monitor.Log(GetItemUniqueKey(Game1.player.ActiveItem));
            }

            // print button presses to the console window
            // this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            if (ActiveConfig.GetShowMenuButton().JustPressed()) {
                Manager.OpenMenu();
            }
        }

        private static string GetItemUniqueKey(Item item) {
            //return $"{item.Name}:" + $"{item.ParentSheetIndex} | {item.ItemId} | {item.QualifiedItemId} | {item.GetType().Name}";
            return $"{CommonHelper.GetItemUniqueKey(item)}";
        }

        // ---------------------------------------------------------------------------------------

        private void InitConfigMenu() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => ResetConfig(),
                save: () => SaveConfig()
            );

            // ------------------------------------------------------------

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, true);

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Attention());

            configMenu.AddParagraph(
                ModManifest,
                () => I18n.Config_AttentionNote()
            );

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, false);

            // ------------------------------------------------------------

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Main());

            var availableModes = Enum.GetValues(typeof(ModMode)).Cast<ModMode>().Select(m => m.ToString()).ToList();

            configMenu.AddTextOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetDefaultMode().ToString(),
                setValue: mode => ActiveConfig.SetDefaultMode((ModMode)availableModes.IndexOf(mode)),
                allowedValues: availableModes.ToArray(),
                formatAllowedValue: (mode) => ((ModMode)availableModes.IndexOf(mode)).GetString(),
                name: () => I18n.Config_DefaultModeName(),
                tooltip: () => I18n.Config_DefaultModeDesc()
            );

            //configMenu.AddParagraph(
            //    ModManifest,
            //    () => I18n.Config_DefaultModeNote()
            //);

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetShowMenuButton(),
                setValue: keybind => ActiveConfig.SetShowMenuButton(keybind),
                name: () => I18n.Config_OpenMenuKeyName(),
                tooltip: () => I18n.Config_OpenMenuKeyDesc()
            );

            // ------------------------------------------------------------

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Multiplayer());

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetShareProgression(),
                setValue: value => ActiveConfig.SetShareProgression(value),
                name: () => I18n.Config_ShareProgressionEnabledName(),
                tooltip: () => I18n.Config_ShareProgressionEnabledDesc()
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetDisableNonHostCommands(),
                setValue: value => ActiveConfig.SetDisableNonHostCommands(value),
                name: () => I18n.Config_DissableNonHostCommandsName(),
                tooltip: () => I18n.Config_DissableNonHostCommandsDesc()
            );

            // ------------------------------------------------------------

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Balance());

            //configMenu.AddBoolOption(
            //    mod: ModManifest,
            //    getValue: () => ActiveConfig.GetUseDefaultBalanceConfig(),
            //    setValue: value => ActiveConfig.SetUseDefaultBalanceConfig(value),
            //    name: () => I18n.Config_DefaultBalanceName(),
            //    tooltip: () => I18n.Config_DefaultBalanceDesc()
            //);

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetResearchAmountMultiplier(),
                setValue: value => ActiveConfig.SetResearchAmountMultiplier(value),
                name: () => I18n.Config_ResearchMultName(),
                tooltip: () => I18n.Config_ResearchMultDesc(),
                min: ModConfigConstraints.ResearchAmountMultipliterMin,
                max: ModConfigConstraints.ResearchAmountMultipliterMax,
                interval: 0.1f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetBuyPriceMultiplier(),
                setValue: value => ActiveConfig.SetBuyPriceMultiplier(value),
                name: () => I18n.Config_BuyPriceMultName(),
                tooltip: () => I18n.Config_BuyPriceMultDesc(),
                min: ModConfigConstraints.BuyPriceMultiplierMin,
                max: ModConfigConstraints.BuyPriceMultiplierMax,
                interval: 0.1f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetSellPriceMultiplier(),
                setValue: value => ActiveConfig.SetSellPriceMultiplier(value),
                name: () => I18n.Config_SellPriceMultName(),
                tooltip: () => I18n.Config_SellPriceMultDesc(),
                min: ModConfigConstraints.SellPriceMultiplierMin,
                max: ModConfigConstraints.SellPriceMultiplierMax,
                interval: 0.1f
            );

            configMenu.AddParagraph(
                ModManifest,
                () => I18n.Config_PriceMarginNote()
            );

            // ------------------------------------------------------------

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Misc());

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetEnableSounds(),
                setValue: value => ActiveConfig.SetEnableSounds(value),
                name: () => I18n.Config_EnableSoundsName(),
                tooltip: () => I18n.Config_EnableSoundsDesc()
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetShowMissingItems(),
                setValue: value => ActiveConfig.SetShowMissingItems(value),
                name: () => I18n.Config_ShowMissingName(),
                tooltip: () => I18n.Config_ShowMissingDesc()
            );


            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetResearchTimeSeconds(),
                setValue: value => ActiveConfig.SetResearchTimeSeconds(value),
                name: () => I18n.Config_ResearchDelayName(),
                tooltip: () => I18n.Config_ResearchDelayDesc(),
                min: ModConfigConstraints.ResearchTimeSecondsMin,
                max: ModConfigConstraints.ResearchTimeSecondsMax,
                interval: 0.1f
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetAutoResearch(),
                setValue: value => ActiveConfig.SetAutoResearch(value),
                name: () => I18n.Config_AutoResearchEnabledName(),
                tooltip: () => I18n.Config_AutoResearchEnabledDesc()
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetUseCustomUIColor(),
                setValue: value => ActiveConfig.SetUseCustomUIColor(value),
                name: () => I18n.Config_UiUseCustomColorName(),
                tooltip: () => I18n.Config_UiUseCustomColorDesc()
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetCustomUIColor().R,
                setValue: value => ActiveConfig.SetCustomUIColor(R: value),
                name: () => I18n.Config_UiCustomColorRName(),
                tooltip: () => I18n.Config_UiCustomColorRDesc(),
                min: 0,
                max: 255,
                interval: 1
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetCustomUIColor().G,
                setValue: value => ActiveConfig.SetCustomUIColor(G: value),
                name: () => I18n.Config_UiCustomColorGName(),
                tooltip: () => I18n.Config_UiCustomColorGDesc(),
                min: 0,
                max: 255,
                interval: 1
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetCustomUIColor().B,
                setValue: value => ActiveConfig.SetCustomUIColor(B: value),
                name: () => I18n.Config_UiCustomColorBName(),
                tooltip: () => I18n.Config_UiCustomColorBDesc(),
                min: 0,
                max: 255,
                interval: 1
            );

            // ------------------------------------------------------------
        }

    }
}
