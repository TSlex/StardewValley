using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemResearchSpawnerV2.Core.Data.Serializable {
    internal record ToolEnchantment(
            string Name,
            int Level
        ) {
    }
}
