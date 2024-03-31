using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Models;
using ItemResearchSpawnerV2.Models.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.Componets {
    internal class MainMenuController : MainMenu {
        private readonly IEnumerable<SpawnableItem> spawnableItems;

        public MainMenuController(IEnumerable<SpawnableItem> items) : base() {

            spawnableItems = items;
            behaviorOnItemGrab = OnItemGrab;

            UpdateView();
        }

        // ===============================================================================================

        private void UpdateView(bool rebuild = false, bool resetScroll = true) {

            var totalRows = (int)Math.Ceiling(spawnableItems.Count() / (CreativeMenu.ItemsPerRow * 1m));
            MaxTopRowIndex = Math.Max(0, totalRows - 3);

            if (rebuild) {
                // TODO: rebuld menu 

                if (resetScroll || TopRowIndex > MaxTopRowIndex) {
                    TopRowIndex = 0;
                }
            }

            ScrollView(0, updateView: false);

            CreativeMenu.actualInventory.Clear();

            foreach (var prefab in spawnableItems.Skip(TopRowIndex * CreativeMenu.ItemsPerRow).Take(CreativeMenu.ItemsPerView)) {
                var item = prefab.CreateItem();

                var quality = ItemQuality.Normal;

                switch (ModManager.Instance.ModMode) {
                    case ModMode.Combined:
                    case ModMode.BuySell:
                        item.Stack = item.maximumStackSize();
                        break;
                    default:
                        item.Stack = item.maximumStackSize();

                        quality = item is SObject
                            ? ItemQuality.Iridium
                            : ItemQuality.Normal;

                        break;
                }


                if (item is SObject obj) {
                    obj.Quality = (int)quality;
                }

                CreativeMenu.actualInventory.Add(item);
            }
        }

        // ------------------------------------------------------------------------------------------------

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (trashCan.containsPoint(x, y) && heldItem != null) {
                TrashHeldItem();
            }

            //else if (this.SortButton.bounds.Contains(x, y)) {

            //    this.SortBy = this.SortBy.GetNext();
            //    this.SortButton.label = this.SortButton.name = this.GetSortLabel(this.SortBy);

            //    UpdateView(rebuild: true);
            //}

            else if (QualityButton.Bounds.Contains(x, y)) {
                //this.Quality = this.Quality.GetNext();
                //this.ResetItemView();
            }

            else if (LeftArrow.bounds.Contains(x, y)) {
                receiveScrollWheelAction(1);
                //Game1.playSound("select");
                //Game1.playSound("secret1");
                //Game1.playSound("reward");
                //Game1.playSound("newRecipe");
                //Game1.playSound("qi_shop_purchase");
            }

            else if (RightArrow.bounds.Contains(x, y)) {
                receiveScrollWheelAction(-1);
                //Game1.playSound("newRecipe");
            }

            //else if (CategoryDropdown.TryClick(x, y, out bool itemClicked, out bool dropdownToggled)) {
            //    if (dropdownToggled) {
            //        this.SetDropdown(CategoryDropdown.IsExpanded);
            //    }
            //    if (itemClicked) {
            //        this.SetCategory(CategoryDropdown.Selected);
            //    }

            //}

            else if (SearchBar.Contains(x, y)) {
                SearchBar.HandleLeftClick(x, y);
            }

            else if (QualityButton.HoveredOver) {
                QualityButton.HandleLeftClick(x, y);
            }

            else if (FavoriteButton.HoveredOver) {
                FavoriteButton.HandleLeftClick(x, y);
            }

            else if (DisplayButton.HoveredOver) {
                DisplayButton.HandleLeftClick(x, y);
            }

            else if (SettingsButton.HoveredOver) {
                SettingsButton.HandleLeftClick(x, y);
            }

            else if (ItemResearchArea.ResearchButton.HoveredOver) {
                ItemResearchArea.ResearchButton.HandleLeftClick(x, y);
            }

            else {
                if (SearchBar.Selected) {
                    SearchBar.Blur();
                }

                base.receiveLeftClick(x, y, playSound);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            base.receiveRightClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y) {
            if (!SearchBar.PersistFocus && !Game1.options.gamepadControls && Game1.lastCursorMotionWasMouse) {
                var overSearchBox = SearchBar.Contains(x, y);

                if (SearchBar.Selected != overSearchBox) {
                    if (overSearchBox) {
                        SearchBar.Focus(false);
                    }
                    else {
                        SearchBar.Blur();
                    }

                }
            }

            QualityButton.HandleHover(x, y);
            FavoriteButton.HandleHover(x, y);
            DisplayButton.HandleHover(x, y);
            SettingsButton.HandleHover(x, y);
            ItemResearchArea.ResearchButton.HandleHover(x, y);

            base.performHoverAction(x, y);
        }

        public override void receiveScrollWheelAction(int direction) {
            //base.receiveScrollWheelAction(direction);
            ScrollView(-direction);
        }

        public void ScrollView(int direction, bool updateView = true) {
            if (direction < 0) {
                TopRowIndex--;
                Game1.playSound("newRecipe");
            }
            else if (direction > 0) {
                TopRowIndex++;
                Game1.playSound("newRecipe");
            }

            TopRowIndex = MathHelper.Clamp(TopRowIndex, 0, MaxTopRowIndex);

            if (updateView) {
                UpdateView();
            }

        }

        // --------------------------------------------------------------------------------------

        private void OnItemGrab(Item item, Farmer player) {
            //if (ModManager.Instance.ModMode != ModMode.Research && ModManager.Instance.GetItemBuyPrice(item, true) <= Game1.player._money) {
            //    ModManager.Instance.BuyItem(item);
            //}

            UpdateView();
        }

        private void TrashHeldItem() {
            var item = heldItem;

            if (item is null) {
                return;
            }

            Utility.trashItem(item);

            heldItem = null;
        }

    }
}
