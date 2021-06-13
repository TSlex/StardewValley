using System;
using StardewValley;

namespace ItemResearchSpawner.Utils
{
    public static class Helpers
    {
        public static bool EqualsCaseInsensitive(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetItemUniqueKey(Item item)
        {
            // var category = GetItemCategory(item);

            return $"{item.Name}:" + $"{item.ParentSheetIndex}";
            
            // return $"{category}:" + $"{item.Name}:" + $"{item.ParentSheetIndex}";
        }

        private static int GetItemCategory(Item item)
        {
            if (item.Name.Equals("Wallpaper", StringComparison.InvariantCultureIgnoreCase))
            {
                return -24;
            }
            if (item.Name.Equals("Flooring", StringComparison.InvariantCultureIgnoreCase))
            {
                return -24;
            }
            if (item.Name.Equals("Wedding Ring", StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }

            return item.category;
        }
    }
}