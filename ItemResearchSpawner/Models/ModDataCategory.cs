namespace ItemResearchSpawner.Models
{
    internal class ModDataCategory
    {
        public string Label { get; set; }

        internal ModDataCategoryRule When { get; set; }
        internal ModDataCategoryRule Except { get; set; }

        public int ResearchCount { get; set; }

        public bool IsMatch(SearchableItem item)
        {
            return When != null && When.IsMatch(item) && Except?.IsMatch(item) != true;
        }
    }
}