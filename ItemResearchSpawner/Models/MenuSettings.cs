namespace ItemResearchSpawner.Models
{
    internal class MenuSettings
    {
        public ItemQuality Quality { get; set; }
        public ItemSortOption SortOption { get; set; }
        public string SearchText { get; set; }
        public string Category { get; set; }

        public MenuSettings()
        {
            Quality = ItemQuality.Normal;
            SortOption = ItemSortOption.Category;
            SearchText = "";
        }
    }
}