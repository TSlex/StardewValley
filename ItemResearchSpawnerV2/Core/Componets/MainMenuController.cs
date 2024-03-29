using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Models;
using ItemResearchSpawnerV2.Models.Enums;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.Componets {
    internal class MainMenuController : MainMenu {
        private readonly IEnumerable<SpawnableItem> spawnableItems;

        public MainMenuController(IEnumerable<SpawnableItem> spawnableItems) : base() {
            this.spawnableItems = spawnableItems;
            UpdateView();
            //foreach (var item in spawnableItems) {
            //    ItemsToGrabMenu.actualInventory.Add(item);
            //}
        }

        private void UpdateView(bool rebuild = false, bool resetScroll = true) {
            //var totalRows = (int)Math.Ceiling(_filteredItems.Count / (ItemsPerRow * 1m));

            //_maxTopRowIndex = Math.Max(0, totalRows - 3);

            //if (rebuild) {
            //    _filteredItems.Clear();
            //    _filteredItems.AddRange(GetFilteredItems());

            //    if (resetScroll || _topRowIndex > _maxTopRowIndex) {
            //        _topRowIndex = 0;
            //    }
            //}

            //ScrollView(0, resetItemView: false);

            CreativeMenu.actualInventory.Clear();

            foreach (var prefab in spawnableItems.Skip(0 * CreativeMenu.ItemsPerRow).Take(CreativeMenu.ItemsPerView)) {
                var item = prefab.CreateItem();

                var quality = ItemQuality.Normal;

                switch (ModManager.Instance.ModMode) {
                    case ModMode.Combined:
                    case ModMode.BuySell:
                        item.Stack = item.maximumStackSize();
                        //quality = availableQuality;
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
    }
}
