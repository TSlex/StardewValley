using System.Collections.Generic;

namespace ItemResearchSpawner.Models.Messages
{
    public class PricelistMessage
    {
        public string PlayerID { get; set; }
        public Dictionary<string, int> Pricelist { get; set; }
    }
}