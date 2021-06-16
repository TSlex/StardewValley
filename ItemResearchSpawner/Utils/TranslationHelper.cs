using StardewModdingAPI;

namespace ItemResearchSpawner.Utils
{
    public class TranslationHelper
    {
        public static TranslationHelper Instance;
        
        private readonly IMonitor _monitor;

        private readonly IModHelper _helper;

        public TranslationHelper(IMonitor monitor, IModHelper helper)
        {
            Instance ??= this;
            
            if (Instance != this)
            {
                monitor.Log($"Another instance of {nameof(TranslationHelper)} is created", LogLevel.Warn);
                return;
            }
            
            _monitor = monitor;
            _helper = helper;
        }

        public string GetString(string key)
        {
            return _helper.Translation.Get(key).Default("???");
        }
    }
}