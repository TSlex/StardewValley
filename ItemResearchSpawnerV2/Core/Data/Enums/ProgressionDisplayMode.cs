namespace ItemResearchSpawnerV2.Core.Data.Enums {
    internal enum ProgressionDisplayMode {
        ResearchedOnly,
        ResearchStarted,
        Combined,
        NotResearched
    }

    internal static class ProgressionDisplayModeExtensions {
        public static ProgressionDisplayMode GetNext(this ProgressionDisplayMode current) {
            return current switch {
                ProgressionDisplayMode.ResearchedOnly => ProgressionDisplayMode.ResearchStarted,
                ProgressionDisplayMode.ResearchStarted => ProgressionDisplayMode.Combined,
                ProgressionDisplayMode.Combined => ProgressionDisplayMode.NotResearched,
                ProgressionDisplayMode.NotResearched => ProgressionDisplayMode.ResearchedOnly,
                _ => throw new NotSupportedException($"Unknown progression display mode: '{current}'")
            };
        }

        public static ProgressionDisplayMode GetPrevious(this ProgressionDisplayMode current) {
            return current switch {
                ProgressionDisplayMode.ResearchedOnly => ProgressionDisplayMode.NotResearched,
                ProgressionDisplayMode.ResearchStarted => ProgressionDisplayMode.ResearchedOnly,
                ProgressionDisplayMode.Combined => ProgressionDisplayMode.ResearchStarted,
                ProgressionDisplayMode.NotResearched => ProgressionDisplayMode.Combined,
                _ => throw new NotSupportedException($"Unknown progression display mode: '{current}'")
            };
        }
    }
}
