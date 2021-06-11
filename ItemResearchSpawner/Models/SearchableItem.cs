using System;
using StardewValley;
using Object = StardewValley.Object;

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

        public bool EqualsToSItem(Item item)
        {
            if (!Item.Category.Equals(item.category))
            {
                return false;
            }

            if (!Item.Name.Equals(item.Name))
            {
                return false;
            }

            if (Item.ParentSheetIndex.Equals(item.ParentSheetIndex))
            {
                if (Item is Object item1 && Item is Object item2)
                {
                    return item1.quality.Equals(item2.quality);
                }

                return true;
            }

            return false;
        }
    }
}