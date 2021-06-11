using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using StardewValley;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Models
{
    internal class ModDataCategoryRule
    {
        public ISet<string> Class { get; set; }
        public ISet<string> ObjType { get; set; }
        public ISet<int> ObjCategory { get; set; }
        public ISet<string> ItemId { get; set; }
        public ISet<string> UniqueKey { get; set; }

        public bool IsMatch(SearchableItem entry)
        {
            var item = entry.Item;
            var obj = item as Object;
            var key = Utils.Helpers.GetItemUniqueKey(item);

            if (UniqueKey.Any() && UniqueKey.Contains(key))
            {
                return true;
            }

            if (Class.Any() && GetClassFullNames(item).Any(className => Class.Contains(className)))
            {
                return true;
            }

            if (ObjCategory.Any() && ObjCategory.Contains(item.Category))
            {
                return true;
            }

            if (ObjType.Any() && obj != null && ObjType.Contains(obj.Type))
            {
                return true;
            }

            if (ItemId.Any() && ItemId.Contains($"{entry.Type}:{item.ParentSheetIndex}"))
            {
                return true;
            }

            return false;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Class = new HashSet<string>(Class ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            ObjType = new HashSet<string>(ObjType ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            ItemId = new HashSet<string>(ItemId ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            UniqueKey = new HashSet<string>(UniqueKey ?? (IEnumerable<string>) new string[0],
                StringComparer.OrdinalIgnoreCase);
            ObjCategory ??= new HashSet<int>();
        }

        private IEnumerable<string> GetClassFullNames(Item item)
        {
            for (var type = item.GetType(); type != null; type = type.BaseType)
            {
                yield return type.FullName;
            }
        }
    }
}