using System;
using StardewValley;

namespace ItemResearchSpawner.Models
{
    public class SearchableItem
    {
        public ItemType Type { get; }
        public Item Item { get; }
        public Func<Item> CreateItem { get; }
        public int ID { get; }
        public string Name => Item.Name;
        public string DisplayName => Item.DisplayName;

        public SearchableItem(ItemType type, int id, Func<SearchableItem, Item> createItem)
        {
            Type = type;
            ID = id;
            CreateItem = () => createItem(this);
            Item = createItem(this);
        }

        public SearchableItem(SearchableItem item)
        {
            Type = item.Type;
            ID = item.ID;
            CreateItem = item.CreateItem;
            Item = item.Item;
        }

        public bool NameContains(string substring)
        {
            return Name.ToLower().Contains(substring.ToLower()) || DisplayName.ToLower().Contains(substring.ToLower());
        }

        public bool NameEquivalentTo(string name)
        {
            return Name.ToLower().Equals(name.ToLower()) || DisplayName.ToLower().Equals(name.ToLower());
        }
    }
}