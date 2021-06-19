using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ItemResearchSpawner.Components
{
    internal class ModManager
    {
        public static ModManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly ModConfig _config;

        #region Proprerties

        private ItemQuality _quality;
        private ModMode _modMode;
        private ItemSortOption _sortOption;
        private string _searchText;
        private string _category;

        public ItemQuality Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                RequestMenuUpdate(false);
            }
        }

        public ModMode ModMode
        {
            get => _modMode;
            set
            {
                _modMode = value;
                RequestMenuUpdate(true);
            }
        }

        public ItemSortOption SortOption
        {
            get => _sortOption;
            set
            {
                _sortOption = value;
                RequestMenuUpdate(true);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RequestMenuUpdate(true);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                RequestMenuUpdate(true);
            }
        }

        #endregion

        public delegate void UpdateMenuView(bool rebuild);

        public event UpdateMenuView OnUpdateMenuView;

        public ModManager(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }

            _monitor = monitor;
            _helper = helper;
            _config = config;

            _helper.Events.GameLoop.Saving += OnSave;
            _helper.Events.GameLoop.DayStarted += OnLoad;
        }

        public void RequestMenuUpdate(bool rebuild)
        {
            OnUpdateMenuView?.Invoke(rebuild);
        }

        #region Save/Load

        private void OnSave(object sender, SavingEventArgs e)
        {
            var state = new ModState
            {
                ActiveMode = ModMode,
                Quality = Quality,
                SortOption = SortOption,
                SearchText = SearchText,
                Category = Category
            };

            _helper.Data.WriteJsonFile($"save/{SaveHelper.DirectoryName}/progress.json", state);
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            var state = _helper.Data.ReadJsonFile<ModState>(
                $"save/{SaveHelper.DirectoryName}/state.json") ?? new ModState
            {
                ActiveMode = _config.DefaultMode
            };

            ModMode = state.ActiveMode;
            Quality = state.Quality;
            SortOption = state.SortOption;
            SearchText = state.SearchText;
            Category = state.Category;
        }

        #endregion
    }
}