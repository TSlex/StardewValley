using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ItemResearchSpawner.Components
{
    public class ProgressionManager
    {
        public static ProgressionManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;

        private ResearchProgression _progression;

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

            _helper.Events.GameLoop.Saving += OnSaveProgression;
            _helper.Events.GameLoop.DayStarted += OnLoadProgression;
        }

        public void ResearchItem(Item item)
        {
            _progression.ResearchItems ??= new List<ResearchItem>();

            var progressionItem = (_progression.ResearchItems)
                .FirstOrDefault(ri => ri.ItemId.Equals(item.parentSheetIndex));

            if (progressionItem == null)
            {
                progressionItem = new ResearchItem
                {
                    ItemId = item.parentSheetIndex,
                    ResearchCount = 0
                };

                _progression.ResearchItems.Add(progressionItem);
            }

            progressionItem.ResearchCount += item.Stack;

            _monitor.Log($"Item: {item.DisplayName} was researched!", LogLevel.Info);
            _monitor.Log($"Current progress: {progressionItem.ResearchCount}", LogLevel.Info);

            SaveProgression();
        }

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
            _helper.Data.WriteJsonFile($"{DirectoryName}/progress.json", _progression);
            _monitor.Log("Progression saved! :)", LogLevel.Debug);
        }

        private void LoadProgression()
        {
            _progression = _helper.Data.ReadJsonFile<ResearchProgression>($"{DirectoryName}/progress.json") ??
                           new ResearchProgression();
            _monitor.Log("Progression loaded! :)", LogLevel.Debug);
        }
    }
}