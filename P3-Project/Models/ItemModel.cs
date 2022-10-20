using Microsoft.Build.Framework;

namespace P3_Project.Models
{
    public class ItemModel
    {

        public string id;
        public string modelName;
        public string itemTable;
        public string description;
        public string? colors;
        public string? sizes;

        public List<Item> items;

        public ItemModel()
        {
            
        }



    }
}
