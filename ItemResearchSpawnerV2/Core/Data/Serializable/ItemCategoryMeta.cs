namespace ItemResearchSpawnerV2.Core.Data.Serializable
{
    internal record ItemCategoryMeta(
            string Label,
            int BaseCost,
            int ResearchCount,
            ItemCategoryRule When,
            ItemCategoryRule Except
        ) {

        public bool IsMatch(SpawnableItem item) {
            return When != null && When.IsMatch(item) && Except?.IsMatch(item) != true;
        }
    }
}
