using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSkipper {

    internal class ModConfig {

        public string ShowMenuButton = "Add";

        // ===============================================================================

        public KeybindList GetShowMenuButton() {
            return KeybindList.Parse(ShowMenuButton);
        }

        // ===============================================================================

        public void SetShowMenuButton(KeybindList value) {
            ShowMenuButton = value.ToString();
        }

    }
}
