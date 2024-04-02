using System;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.Utils {
    public static class CommonHelper {
        public static bool EqualsCaseInsensitive(string a, string b) {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetItemUniqueKey(Item item) {
            return $"{item.Name}:" + $"{item.ParentSheetIndex}";
        }

        public static IEnumerable<string> GetClassFullNames(Item item) {
            for (Type type = item.GetType(); type != null; type = type.BaseType) {
                yield return type.FullName;
            }
        }
    }
}