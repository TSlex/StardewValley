using System.Collections.Generic;

namespace ItemResearchSpawner.Models
{
    public class ResearchProgression
    {
        public ICollection<ResearchItem> ResearchItems;
    }

    public class ResearchItem
    {
        public int ItemId;
        public int ResearchCount;
    }
}