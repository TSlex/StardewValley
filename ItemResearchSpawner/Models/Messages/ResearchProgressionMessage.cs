using System.Collections.Generic;

namespace ItemResearchSpawner.Models.Messages
{
    public class ResearchProgressionMessage
    {
        public string PlayerID { get; set; }
        public Dictionary<string, ResearchProgression> Progression { get; set; }
    }
}