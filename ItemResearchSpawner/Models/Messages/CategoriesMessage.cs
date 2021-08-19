using System.Collections.Generic;

namespace ItemResearchSpawner.Models.Messages
{
    public class CategoriesMessage
    {
        public string PlayerID { get; set; }
        public List<ModDataCategory> Categories { get; set; }
    }
}