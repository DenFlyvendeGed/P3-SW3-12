namespace P3_Project.Models
{
    public class Item
    {
        public int Id;
        public string ModelId { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        private int Reserved { get; set; }
        private int Sold { get; set; }

        public Item()
        {
            ModelId = "";
            Color = "";
            Size = "";
            Stock = 0;
            Reserved = 0;
            Sold = 0;
        }

        public Item(string id, string modelId, string description, string color, string size)
        {

        }

    }
}
