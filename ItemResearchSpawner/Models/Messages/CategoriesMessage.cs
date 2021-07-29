namespace ItemResearchSpawner.Models.Messages
{
    public class CategoriesMessage
    {
        public string PlayerID { get; set; }
        public ModDataCategory[] Categories { get; set; }
    }
}