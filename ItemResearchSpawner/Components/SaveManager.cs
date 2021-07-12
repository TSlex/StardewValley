using System;
using System.Collections.Generic;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ItemResearchSpawner.Components
{
    internal class SaveManager
    {
        public static SaveManager Instance;

        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly IManifest _modManifest;

        private Dictionary<string, Dictionary<string, ResearchProgression>> _progressions;
        private Dictionary<string, ModState> _modStates;

        public SaveManager(IMonitor monitor, IModHelper helper, IManifest modManifest)
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

            _helper.Events.GameLoop.Saving += OnSave;
            _helper.Events.GameLoop.Saved += OnLoad;
        }

        public void CommitProgression(string playerID, Dictionary<string, ResearchProgression> commitProgression)
        {
            var progression = _progressions[playerID] ?? new Dictionary<string, ResearchProgression>();

            foreach (var key in commitProgression.Keys)
            {
                progression[key] = commitProgression[key];
            }
        }

        public Dictionary<string, ResearchProgression> GetProgression(string playerID)
        {
            return _progressions[playerID] ?? new Dictionary<string, ResearchProgression>();
        }

        public void CommitModState(string playerID, ModState modState)
        {
            _modStates[playerID] = modState;
        }

        public ModState GetModState(string playerID)
        {
            return _modStates[playerID] ?? new ModState()
            {
                ActiveMode = _helper.ReadConfig<ModConfig>().DefaultMode
            };
        }

        private void OnSave(object sender, SavingEventArgs e)
        {
            _helper.Data.WriteSaveData(SaveHelper.ProgressionsKey, _progressions);
            _helper.Data.WriteSaveData(SaveHelper.ModStatesKey, _modStates);
        }

        private void OnLoad(object sender, SavedEventArgs e)
        {
            try
            {
                _progressions =
                    _helper.Data.ReadSaveData<Dictionary<string, Dictionary<string, ResearchProgression>>>(SaveHelper
                        .ProgressionsKey)
                    ?? new Dictionary<string, Dictionary<string, ResearchProgression>>();
            }
            catch (Exception _)
            {
                _progressions = new Dictionary<string, Dictionary<string, ResearchProgression>>();
            }

            try
            {
                _modStates = _helper.Data.ReadSaveData<Dictionary<string, ModState>>(SaveHelper.ModStatesKey) ??
                             new Dictionary<string, ModState>();
            }
            catch (Exception _)
            {
                _modStates = new Dictionary<string, ModState>();
            }
        }
    }
}