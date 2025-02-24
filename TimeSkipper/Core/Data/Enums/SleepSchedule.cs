namespace TimeSkipper.Core.Data.Enums {
    internal enum SleepSchedule {
        calendar_mode,
        rainy_mode,
        sunny_mode,
        lucky_mode,
        unlucky_mode,
        event_mode
    }

    internal static class SleepScheduleExtensions {
        public static string GetString(this SleepSchedule current) {
            return current switch {
                SleepSchedule.calendar_mode => I18n.Schedule_CalendarMode(),
                SleepSchedule.rainy_mode => I18n.Schedule_RainyMode(),
                SleepSchedule.sunny_mode => I18n.Schedule_SunnyMode(),
                SleepSchedule.lucky_mode => I18n.Schedule_LuckyMode(),
                SleepSchedule.unlucky_mode => I18n.Schedule_UnluckyMode(),
                SleepSchedule.event_mode => I18n.Schedule_EventMode(),
                _ => "???"
            };
        }
    }
}
