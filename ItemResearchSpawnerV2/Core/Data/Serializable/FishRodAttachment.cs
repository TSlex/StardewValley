namespace ItemResearchSpawnerV2.Core.Data.Serializable {
    internal record FishRodAttachment(
            string UniqueKey,
            int Uses,
            int Index,
            int Stack,
            bool IsEmpty
        ) {
    }
}
