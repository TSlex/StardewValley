namespace ItemResearchSpawner.Models
{
    public class SpawnableItem : SearchableItem
    {
        public string Category { get; }

        public SpawnableItem(SearchableItem item, string category) : base(item)
        {
            Category = category;
        }
    }
}