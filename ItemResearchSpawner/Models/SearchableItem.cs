using System;
using StardewValley;

namespace ItemResearchSpawner.Models
{
    internal class SearchableItem
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
    }
}