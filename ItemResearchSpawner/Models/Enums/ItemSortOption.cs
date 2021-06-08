using System;

namespace ItemResearchSpawner.Models
{
    internal enum ItemSortOption
    {
        Name,
        Category,
        ID
    }

    internal static class ItemSortOptionExtensions
    {
        public static ItemSortOption GetNext(this ItemSortOption current)
        {
            return current switch
            {
                ItemSortOption.Name => ItemSortOption.Category,
                ItemSortOption.Category => ItemSortOption.ID,
                ItemSortOption.ID => ItemSortOption.Name,
                _ => throw new NotSupportedException($"Unknown sort '{current}'.")
            };
        }
    }
}