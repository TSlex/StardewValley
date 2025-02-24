namespace TimeSkipper.Core.Data.Enums {
    internal enum GameSeason {
        spring,
        summer,
        fall,
        winter
    }

    internal static class GameSeasonsExtensions {
        public static string GetString(this GameSeason current) {
            return current switch {
                GameSeason.spring => I18n.Calendaar_Spring(),
                GameSeason.summer => I18n.Calendaar_Summer(),
                GameSeason.fall => I18n.Calendaar_Fall(),
                GameSeason.winter => I18n.Calendaar_Winter(),
                _ => "???"
            };
        }

        public static GameSeason GetNext(this GameSeason current) {
            return current switch {
                GameSeason.spring => GameSeason.summer,
                GameSeason.summer => GameSeason.fall,
                GameSeason.fall => GameSeason.winter,
                GameSeason.winter => GameSeason.spring,
                _ => throw new NotSupportedException($"Unknown game season: '{current}'")
            };
        }

        public static GameSeason GetPrevious(this GameSeason current) {
            return current switch {
                GameSeason.spring => GameSeason.winter,
                GameSeason.summer => GameSeason.spring,
                GameSeason.fall => GameSeason.summer,
                GameSeason.winter => GameSeason.fall,
                _ => throw new NotSupportedException($"Unknown game season: '{current}'")
            };
        }
    }
}
