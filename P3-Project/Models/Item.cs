namespace P3_Project.Models
{
    public class Item
    {
        public string id;
        public string modelId;
        public string modelName;
        public string color;
        public string size;
        public string stock;
        private string reserved;
        private string sold;

        public Item()
        {
            
        }

        public Item(string id, string modelId, string description, string color, string size)
        {

        }

    }
}
