using System;

namespace ItemResearchSpawnerV2.Core.Data.Enums {
    internal enum ItemSortOption {
        NameASC,
        NameDESC,
        CategoryASC,
        CategoryDESC,
        IDASC,
        IDDESC,
        PriceASC,
        PriceDESC
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
                ItemSortOption.NameASC => I18n.Sort_ByNameAsc(),
                ItemSortOption.NameDESC => I18n.Sort_ByNameDesc(),
                ItemSortOption.CategoryASC => I18n.Sort_ByCategoryAsc(),
                ItemSortOption.CategoryDESC => I18n.Sort_ByCategoryDesc(),
                ItemSortOption.IDASC => I18n.Sort_ByIdAsc(),
                ItemSortOption.IDDESC => I18n.Sort_ByIdDesc(),
                ItemSortOption.PriceASC => I18n.Sort_ByPriceAsc(),
                ItemSortOption.PriceDESC => I18n.Sort_ByPriceDesc(),
                _ => "???"
            };
        }
    }
}