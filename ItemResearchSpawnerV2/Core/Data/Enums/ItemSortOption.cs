using System;

namespace ItemResearchSpawnerV2.Models.Enums
{
    public enum ItemSortOption
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
                _ => throw new NotSupportedException($"Unknown sort option: '{current}'")
            };
        }
        
        public static ItemSortOption GetPrevious(this ItemSortOption current)
        {
            return current switch
            {
                ItemSortOption.Name => ItemSortOption.ID,
                ItemSortOption.Category => ItemSortOption.Name,
                ItemSortOption.ID => ItemSortOption.Category,
                _ => throw new NotSupportedException($"Unknown sort option: '{current}'")
            };
        }
    }
}