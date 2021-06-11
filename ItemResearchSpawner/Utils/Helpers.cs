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
            return $"{item.category}:" + $"{item.Name}:" + $"{item.ParentSheetIndex}";
        }
    }
}