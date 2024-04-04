using System;

namespace ItemResearchSpawnerV2.Models.Enums
{
    internal enum ItemSortOption
    {
        NameUp,
        NameDown,
        CategoryUp,
        CategoryDown,
        IDUp,
        IDDown,
        PriceUp,
        PriceDown
    }

    internal static class ItemSortOptionExtensions {
        //public static ItemSortOption GetNext(this ItemSortOption current) {
        //    return current switch {
        //        ItemSortOption.NameUp => ItemSortOption.Category,
        //        ItemSortOption.Category => ItemSortOption.ID,
        //        ItemSortOption.ID => ItemSortOption.NameUp,
        //        _ => throw new NotSupportedException($"Unknown sort option: '{current}'")
        //    };
        //}

        //public static ItemSortOption GetPrevious(this ItemSortOption current) {
        //    return current switch {
        //        ItemSortOption.NameUp => ItemSortOption.ID,
        //        ItemSortOption.Category => ItemSortOption.NameUp,
        //        ItemSortOption.ID => ItemSortOption.Category,
        //        _ => throw new NotSupportedException($"Unknown sort option: '{current}'")
        //    };
        //}

        public static string GetString(this ItemSortOption current) {
            return current switch {
                ItemSortOption.NameUp => I18n.Sort_ByNameAsc(),
                ItemSortOption.NameDown => I18n.Sort_ByNameDesc(),
                ItemSortOption.CategoryUp => I18n.Sort_ByCategoryAsc(),
                ItemSortOption.CategoryDown => I18n.Sort_ByCategoryDesc(),
                ItemSortOption.IDUp => I18n.Sort_ByIdAsc(),
                ItemSortOption.IDDown => I18n.Sort_ByIdDesc(),
                ItemSortOption.PriceUp => I18n.Sort_ByPriceAsc(),
                ItemSortOption.PriceDown => I18n.Sort_ByPriceDesc(),
                _ => "???"
            };
        }
    }
}