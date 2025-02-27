using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSkipper {

    internal class ModConfig {

        public string ShowMenuButton = "Z";

        public string SkipOneDayButton = "Add";

        public int SleepSheduleMaxDays = 28;

        // danger, but skipping gets faster

        public bool DisableSavingWhileSkipping = false;
        public bool DisableFading = false;

        // ===============================================================================

        public KeybindList GetShowMenuButton() {
            return KeybindList.Parse(ShowMenuButton);
        }

        public KeybindList GetSkipOneDayButton() {
            return KeybindList.Parse(SkipOneDayButton);
        }

        public int GetSleepSheduleMaxDays() {
            return SleepSheduleMaxDays;
        }

        public bool GetDisableSavingWhileSkipping() {
            return DisableSavingWhileSkipping;
        }

        public bool GetDisableFading() {
            return DisableFading;
        }

        // ===============================================================================

        public void SetShowMenuButton(KeybindList value) {
            ShowMenuButton = value.ToString();
        }

        public void SetSkipOneDayButton(KeybindList value) {
            SkipOneDayButton = value.ToString();
        }

        public void SetSleepSheduleMaxDays(int value) {
            SleepSheduleMaxDays = value;
        }

        public void SetDisableSavingWhileSkipping(bool value) {
            DisableSavingWhileSkipping = value;
        }

        public void SetDisableFading(bool value) {
            DisableFading = value;
        }

    }
}
