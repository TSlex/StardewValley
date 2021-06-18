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

        private ModMode _modModMode;

        public ModManager(IMonitor monitor, IModHelper helper)
        {
            Instance ??= this;

            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(ProgressionManager)} is created", LogLevel.Warn);
                return;
            }
            
            _monitor = monitor;
            _helper = helper;

            _helper.Events.GameLoop.Saving += OnSave;
            _helper.Events.GameLoop.DayStarted += OnLoad;
        }

        private void OnSave(object sender, SavingEventArgs e)
        {
            var save = new ModSave
            {
                ActiveMode = _modModMode
            };

            _helper.Data.WriteJsonFile($"save/{SaveHelper.DirectoryName}/progress.json", save);
        }

        private void OnLoad(object sender, DayStartedEventArgs e)
        {
            var save = _helper.Data.ReadJsonFile<ModSave>(
                $"save/{SaveHelper.DirectoryName}/progress.json") ?? new ModSave();

            _modModMode = save.ActiveMode;
        }

        public ModMode GetMode => _modModMode;

        public void SetMode(ModMode mode)
        {
            _modModMode = mode;
        }
    }
}