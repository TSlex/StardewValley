namespace TimeSkipper.Core.Data.Enums {
    internal enum SleepSchedule {
        calendar_mode,
        rainy_mode,
        sunny_mode,
        lucky_mode,
        unlucky_mode,
        event_mode,
        farm_event_mode,
        building_completed_mode
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
                SleepSchedule.farm_event_mode => I18n.Schedule_FarmEventMode(),
                SleepSchedule.building_completed_mode => I18n.Schedule_BuildingCompletedMode(),
                _ => "???"
            };
        }

        public static string GetSkippingNote(this SleepSchedule current) {
            return current switch {
                SleepSchedule.calendar_mode => "",
                SleepSchedule.rainy_mode => I18n.Info_SkipRainyMode(),
                SleepSchedule.sunny_mode => I18n.Info_SkipSunnyMode(),
                SleepSchedule.lucky_mode => I18n.Info_SkipLuckyMode(),
                SleepSchedule.unlucky_mode => I18n.Info_SkipUnluckyMode(),
                SleepSchedule.event_mode => I18n.Info_SkipEventMode(),
                SleepSchedule.farm_event_mode => I18n.Info_SkipFarmEventMode(),
                SleepSchedule.building_completed_mode => I18n.Info_SkipBuildingCompletedMode(),
                _ => "???"
            };
        }
    }
}
