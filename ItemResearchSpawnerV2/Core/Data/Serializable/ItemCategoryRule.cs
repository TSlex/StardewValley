using ItemResearchSpawnerV2.Core.Utils;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.Data.Serializable {
    internal record ItemCategoryRule {
        public ISet<string> Class { get; }
        public ISet<string> ObjType { get; }
        public ISet<int> ObjCategory { get; }
        public ISet<string> ItemId { get; }
        public ISet<string> UniqueKey { get; set; }

        public ItemCategoryRule(HashSet<string> @class, HashSet<string> objType, HashSet<int> objCategory, HashSet<string> itemId, HashSet<string> uniqueKey) {
            IEnumerable<string> empty = Enumerable.Empty<string>();

            Class = new HashSet<string>(@class ?? empty, StringComparer.OrdinalIgnoreCase);
            ObjType = new HashSet<string>(objType ?? empty, StringComparer.OrdinalIgnoreCase);
            ObjCategory = new HashSet<int>(objCategory ?? Enumerable.Empty<int>());
            ItemId = new HashSet<string>(itemId ?? empty, StringComparer.OrdinalIgnoreCase);
            UniqueKey = new HashSet<string>(uniqueKey ?? empty, StringComparer.OrdinalIgnoreCase);
        }

        public bool IsMatch(SpawnableItem entry) {
            var item = entry.Item;
            var key = CommonHelper.GetItemUniqueKey(item);

            if (UniqueKey.Any() && UniqueKey.Where(ukey => key.Contains(ukey)).Any()) {
                return true;
            }
            if (Class.Any() && CommonHelper.GetClassFullNames(item).Any(className => Class.Contains(className))) {
                return true;
            }
            if (ObjCategory.Any() && ObjCategory.Contains(item.Category)) {
                return true;
            }
            if (ObjType.Any() && item is SObject obj && ObjType.Contains(obj.Type)) {
                return true;
            }
            if (ItemId.Any() && ItemId.Contains($"{entry.Type}:{item.ParentSheetIndex}")) {
                return true;
            }

            return false;
        }
    }
}