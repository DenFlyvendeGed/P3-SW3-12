namespace P3_Project.Models
{
    public class ItemPack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ItemModel> Items { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Sizes { get; set; }
        ItemPack()
        {

        }
    }
}
