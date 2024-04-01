namespace ItemResearchSpawnerV2.Core.Data.Enums {
    internal enum ProgressionDisplayMode {
        ResearchedOnly,
        ResearchStarted,
        Combined
    }

    internal static class ProgressionDisplayModeExtensions {
        public static ProgressionDisplayMode GetNext(this ProgressionDisplayMode current) {
            return current switch {
                ProgressionDisplayMode.ResearchedOnly => ProgressionDisplayMode.ResearchStarted,
                ProgressionDisplayMode.ResearchStarted => ProgressionDisplayMode.Combined,
                ProgressionDisplayMode.Combined => ProgressionDisplayMode.ResearchedOnly,
                _ => throw new NotSupportedException($"Unknown progression display mode: '{current}'")
            };
        }

        public static ProgressionDisplayMode GetPrevious(this ProgressionDisplayMode current) {
            return current switch {
                ProgressionDisplayMode.ResearchedOnly => ProgressionDisplayMode.Combined,
                ProgressionDisplayMode.ResearchStarted => ProgressionDisplayMode.ResearchedOnly,
                ProgressionDisplayMode.Combined => ProgressionDisplayMode.ResearchStarted,
                _ => throw new NotSupportedException($"Unknown progression display mode: '{current}'")
            };
        }
    }
}
