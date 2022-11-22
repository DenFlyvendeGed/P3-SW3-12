using P3_Project.Models.DB;

namespace P3_Project.Models
{
    public class Tag
    {

        public int Id { get; set; }
        public string Name { get; set; }

        static StorageDB db = new StorageDB();

        public static List<Tag> GetAllTagsOfItemModel(string itemModelId)
        {
            var list = new List<Tag>();
            list = db.DB.GetAllElements($"ItemModel_{itemModelId}_Tags", new Tag());

            
            return list;
        }

        public static List<Tag> GetAllTags()
        {
            var list = new List<Tag>();
            list = db.DB.GetAllElements($"Tags", new Tag());


            return list;
        }
    }
    
}
