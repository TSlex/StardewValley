using StardewValley;

namespace ItemResearchSpawnerV2.Core.Data {

    /**
        MIT License

        Copyright (c) 2018 Pathoschild, CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
    **/

    internal class SpawnableItem {

        public bool Forbidden = false;

        /*********
        ** Accessors
        *********/
        /// <summary>The <see cref="IItemDataDefinition.Identifier"/> value for the item type.</summary>
        public string Type {
            get;
        }

        /// <summary>A sample item instance.</summary>
        public Item Item {
            get; set;
        }

        /// <summary>Create an item instance.</summary>
        public Func<Item> CreateItem {
            get;
        }

        /// <summary>The unqualified item ID.</summary>
        public string Id {
            get;
        }

        /// <summary>The qualified item ID.</summary>
        public string QualifiedItemId {
            get;
        }

        /// <summary>The item's default name.</summary>
        public string Name => Item.Name;

        /// <summary>The item's display name for the current language.</summary>
        public string DisplayName => Item.DisplayName;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="type">The item type.</param>
        /// <param name="id">The unqualified item ID.</param>
        /// <param name="createItem">Create an item instance.</param>
        public SpawnableItem(string type, string id, Func<SpawnableItem, Item> createItem) {
            Type = type;
            Id = id;
            QualifiedItemId = Type + Id;
            CreateItem = () => createItem(this);
            Item = createItem(this);
            Forbidden = false;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="item">The item metadata to copy.</param>
        public SpawnableItem(SpawnableItem item) {
            Type = item.Type;
            Id = item.Id;
            QualifiedItemId = item.QualifiedItemId;
            CreateItem = item.CreateItem;
            Item = item.Item;
            Forbidden = item.Forbidden;
        }

        /// <summary>Get whether the item name contains a case-insensitive substring.</summary>
        /// <param name="substring">The substring to find.</param>
        public bool NameContains(string substring) {
            return
                Name.Contains(substring, StringComparison.OrdinalIgnoreCase)
                || DisplayName.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Get whether the item name is exactly equal to a case-insensitive string.</summary>
        /// <param name="name">The substring to find.</param>
        public bool NameEquivalentTo(string name) {
            return
                Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                || DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
