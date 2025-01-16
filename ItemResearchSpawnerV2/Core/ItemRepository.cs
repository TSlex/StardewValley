using System.Diagnostics.CodeAnalysis;
using ItemResearchSpawnerV2.Core.Data;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using StardewValley.GameData.FishPonds;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core {

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

    internal static class ItemRepository {
        /*********
        ** Public methods
        *********/
        /// <summary>Get all spawnable items.</summary>
        /// <param name="onlyType">Only include items for the given <see cref="IItemDataDefinition.Identifier"/>.</param>
        /// <param name="includeVariants">Whether to include flavored variants like "Sunflower Honey".</param>
        [SuppressMessage("ReSharper", "AccessToModifiedClosure", Justification = $"{nameof(ItemRepository.TryCreate)} invokes the lambda immediately.")]
        public static IEnumerable<SpawnableItem> GetAll(string? onlyType = null, bool includeVariants = true) {
            //
            //
            // Be careful about closure variable capture here!
            //
            // SearchableItem stores the Func<Item> to create new instances later. Loop variables passed into the
            // function will be captured, so every func in the loop will use the value from the last iteration. Use the
            // TryCreate(type, id, entity => item) form to avoid the issue, or create a local variable to pass in.
            //
            //

            IEnumerable<SpawnableItem?> GetAllRaw() {
                // get from item data definitions
                foreach (IItemDataDefinition itemType in ItemRegistry.ItemTypes) {

                    if (onlyType != null && itemType.Identifier != onlyType)
                        continue;

                    switch (itemType.Identifier) {
                        // objects
                        case "(O)": {
                            ObjectDataDefinition objectDataDefinition = (ObjectDataDefinition) ItemRegistry.GetTypeDefinition(ItemRegistry.type_object);

                            foreach (string id in itemType.GetAllIds()) {
                                // base item
                                SpawnableItem? result = TryCreate(itemType.Identifier, id, p => ItemRegistry.Create(itemType.Identifier + p.Id));

                                // ring
                                if (result?.Item is Ring)
                                    yield return result;

                                // journal scraps
                                else if (result?.QualifiedItemId == "(O)842") {
                                    foreach (SpawnableItem? journalScrap in GetSecretNotes(itemType, isJournalScrap: true))
                                        yield return journalScrap;
                                }

                                // secret notes
                                else if (result?.QualifiedItemId == "(O)79") {
                                    foreach (SpawnableItem? secretNote in GetSecretNotes(itemType, isJournalScrap: false))
                                        yield return secretNote;
                                }

                                //// object
                                //else {
                                //    yield return result?.QualifiedItemId == "(O)340"
                                //        ? TryCreate(itemType.Identifier, result.Id, _ => objectDataDefinition.CreateFlavoredHoney(null)) // game creates "Wild Honey" when there's no ingredient, instead of the base Honey item
                                //        : result;

                                //    if (includeVariants) {
                                //        foreach (SpawnableItem? variant in GetFlavoredObjectVariants(objectDataDefinition, result?.Item as Object, itemType))
                                //            yield return variant;
                                //    }
                                //}

                                else {
                                    switch (result?.QualifiedItemId) {
                                        // honey should be "Wild Honey" when there's no ingredient, instead of the base Honey item
                                        case "(O)340":
                                            yield return TryCreate(itemType.Identifier, result.Id, _ => objectDataDefinition.CreateFlavoredHoney(null));
                                            break;

                                        // don't return placeholder items
                                        case "(O)DriedFruit":
                                        case "(O)DriedMushrooms":
                                        case "(O)SmokedFish":
                                        case "(O)SpecificBait":
                                            break;

                                        default:
                                            if (result != null)
                                                yield return result;
                                            break;
                                    }

                                    if (includeVariants) {
                                        foreach (SpawnableItem? variant in GetFlavoredObjectVariants(objectDataDefinition, result?.Item as Object, itemType))
                                            yield return variant;
                                    }
                                }
                            }
                        }
                        break;

                        // no special handling needed
                        default:
                            foreach (string id in itemType.GetAllIds())
                                yield return TryCreate(itemType.Identifier, id, p => ItemRegistry.Create(itemType.Identifier + p.Id));
                            break;
                    }
                }

                //// wallpapers
                //if (onlyType is null or "(WP)") {
                //    for (int id = 0; id < 112; id++)
                //        yield return TryCreate("(WP)", id.ToString(), p => new Wallpaper(int.Parse(p.Id)) { Category = Object.furnitureCategory });
                //}

                //// flooring
                //if (onlyType is null or "(FL)") {
                //    for (int id = 0; id < 56; id++)
                //        yield return TryCreate("(FL)", id.ToString(), p => new Wallpaper(int.Parse(p.Id), isFloor: true) { Category = Object.furnitureCategory });
                //}
            }

            return (
                from item in GetAllRaw()
                where item != null
                select item
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the individual secret note or journal scrap items.</summary>
        /// <param name="itemType">The object data definition.</param>
        /// <param name="isJournalScrap">Whether to get journal scraps.</param>
        /// <remarks>Derived from <see cref="GameLocation.tryToCreateUnseenSecretNote"/>.</remarks>
        private static IEnumerable<SpawnableItem?> GetSecretNotes(IItemDataDefinition itemType, bool isJournalScrap) {
            // get base item ID
            string baseId = isJournalScrap ? "842" : "79";

            // get secret note IDs
            var ids =
                TryLoad(() => DataLoader.SecretNotes(Game1.content))
                .Keys
                .Where(isJournalScrap
                    ? id => (id >= GameLocation.JOURNAL_INDEX)
                    : id => (id < GameLocation.JOURNAL_INDEX)
                )
                .Select<int, int>(isJournalScrap
                    ? id => (id - GameLocation.JOURNAL_INDEX)
                    : id => id
                );

            // build items
            foreach (int i in ids) {
                int id = i; // avoid closure capture

                yield return TryCreate(itemType.Identifier, $"{baseId}/{id}", _ => {
                    Item note = ItemRegistry.Create(itemType.Identifier + baseId);
                    note.Name = $"{note.Name} #{id}";
                    return note;
                });
            }
        }

        /// <summary>Get flavored variants of a base item (like Blueberry Wine for Blueberry), if any.</summary>
        /// <param name="objectDataDefinition">The item data definition for object items.</param>
        /// <param name="item">A sample of the base item.</param>
        /// <param name="itemType">The object data definition.</param>
        private static IEnumerable<SpawnableItem?> GetFlavoredObjectVariants(ObjectDataDefinition objectDataDefinition, Object? item, IItemDataDefinition itemType) {
            if (item is null)
                yield break;

            string id = item.ItemId;

            // by category
            switch (item.Category) {

                // fish
                case Object.FishCategory:
                    yield return TryCreate(itemType.Identifier, $"SmokedFish/{id}", _ => objectDataDefinition.CreateFlavoredSmokedFish(item));
                    yield return TryCreate(itemType.Identifier, $"SpecificBait/{id}", _ => objectDataDefinition.CreateFlavoredBait(item));
                    break;

                // fruit products
                case Object.FruitsCategory:
                    yield return TryCreate(itemType.Identifier, $"348/{id}", _ => objectDataDefinition.CreateFlavoredWine(item));
                    yield return TryCreate(itemType.Identifier, $"344/{id}", _ => objectDataDefinition.CreateFlavoredJelly(item));

                    if (item.QualifiedItemId != "(O)398") // raisins are their own item
                        yield return TryCreate(itemType.Identifier, $"398/{id}", _ => objectDataDefinition.CreateFlavoredDriedFruit(item));
                    break;

                // greens
                case Object.GreensCategory:
                    yield return TryCreate(itemType.Identifier, $"342/{id}", _ => objectDataDefinition.CreateFlavoredPickle(item));
                    break;

                // vegetable products
                case Object.VegetableCategory:
                    yield return TryCreate(itemType.Identifier, $"350/{id}", _ => objectDataDefinition.CreateFlavoredJuice(item));
                    yield return TryCreate(itemType.Identifier, $"342/{id}", _ => objectDataDefinition.CreateFlavoredPickle(item));
                    break;

                // flower honey
                case Object.flowersCategory:
                    yield return TryCreate(itemType.Identifier, $"340/{id}", _ => objectDataDefinition.CreateFlavoredHoney(item));
                    break;

                // roe and aged roe (derived from FishPond.GetFishProduce)
                case Object.sellAtFishShopCategory when item.QualifiedItemId == "(O)812": {
                    GetRoeContextTagLookups(out HashSet<string> simpleTags, out List<List<string>> complexTags);

                    foreach (string key in Game1.objectData.Keys) {
                        // get input
                        Object? input = TryCreate(itemType.Identifier, key, p => new Object(p.Id, 1))?.Item as Object;
                        if (input == null)
                            continue;

                        HashSet<string> inputTags = input.GetContextTags();
                        if (!inputTags.Any())
                            continue;

                        // check if roe-producing fish
                        if (!inputTags.Any(tag => simpleTags.Contains(tag)) && !complexTags.Any(set => set.All(tag => input.HasContextTag(tag))))
                            continue;

                        // create roe
                        SpawnableItem? roe = TryCreate(itemType.Identifier, $"812/{input.ItemId}", _ => objectDataDefinition.CreateFlavoredRoe(input));
                        yield return roe;

                        // create aged roe
                        if (roe?.Item is Object roeObj && input.QualifiedItemId != "(O)698") // skip aged sturgeon roe (which is a separate caviar item)
                            yield return TryCreate(itemType.Identifier, $"447/{input.ItemId}", _ => objectDataDefinition.CreateFlavoredAgedRoe(roeObj));
                    }
                }
                break;
            }

            // by context tag
            if (item.HasContextTag("preserves_pickle") && item.Category != Object.VegetableCategory)
                yield return TryCreate(itemType.Identifier, $"342/{id}", _ => objectDataDefinition.CreateFlavoredPickle(item));

            if (item.HasContextTag("edible_mushroom"))
                yield return TryCreate(itemType.Identifier, $"DriedMushrooms/{id}", _ => objectDataDefinition.CreateFlavoredDriedMushroom(item));
        }

        /// <summary>Get optimized lookups to match items which produce roe in a fish pond.</summary>
        /// <param name="simpleTags">A lookup of simple singular tags which match a roe-producing fish.</param>
        /// <param name="complexTags">A list of tag sets which match roe-producing fish.</param>
        private static void GetRoeContextTagLookups(out HashSet<string> simpleTags, out List<List<string>> complexTags) {

            simpleTags = new HashSet<string>();
            complexTags = new List<List<string>>();

            foreach (FishPondData data in TryLoad(() => DataLoader.FishPondData(Game1.content))) {

                if (data.ProducedItems.All(p => p.ItemId is not ("812" or "(O)812")))
                    continue; // doesn't produce roe

                if (data.RequiredTags.Count == 1 && !data.RequiredTags[0].StartsWith("!"))
                    simpleTags.Add(data.RequiredTags[0]);
                else
                    complexTags.Add(data.RequiredTags);
            }
        }

        /// <summary>Try to load a data asset, and return empty data if it's invalid.</summary>
        /// <typeparam name="TAsset">The asset type.</typeparam>
        /// <param name="load">A callback which loads the asset.</param>
        private static TAsset TryLoad<TAsset>(Func<TAsset> load)
            where TAsset : new() {
            try {
                return load();
            }
            catch (ContentLoadException) {
                // generally due to a player incorrectly replacing a data file with an XNB mod
                return new TAsset();
            }
        }

        /// <summary>Create a searchable item if valid.</summary>
        /// <param name="type">The item type.</param>
        /// <param name="key">The locally unique item key.</param>
        /// <param name="createItem">Create an item instance.</param>
        private static SpawnableItem? TryCreate(string type, string key, Func<SpawnableItem, Item> createItem) {
            try {
                SpawnableItem item = new SpawnableItem(type, key, createItem);
                item.Item.getDescription(); // force-load item data, so it crashes here if it's invalid

                if (item.Item.Name is null or "Error Item")
                    return null;

                return item;
            }
            catch {
                return null; // if some item data is invalid, just don't include it
            }
        }
    }
}
