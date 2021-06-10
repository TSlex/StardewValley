using System.Collections.Generic;

namespace ItemResearchSpawner.Models
{
    internal class ResearchProgression
    {
        public ICollection<ResearchItem> ResearchItems { get; set; }
    }

    internal class ResearchItem
    {
        public int ItemId { get; set; }
        public int ResearchCount { get; set; }
        public int ResearchCountGold { get; set; }
        public int ResearchCountSilver { get; set; }
        public int ResearchCountIridium { get; set; }
    }
}