using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace P3_Project.Models
{
 
    public class ItemModel
    {

        public int Id;
        public string ModelName { get; set; }
        //public string ItemTable { get; set; }
        public string Description { get; set; }
        public string? Colors { get; set; }
        public string? Sizes { get; set; }

        public List<Item>? items;

        StorageDB db = new StorageDB();
        public ItemModel()
        {

            ModelName = "";
            //ItemTable = "";
            Description = "";
            
        }

        //Adds the ItemModel instance to the list of Itemmodel's int the SQL table
        public void Create()
        {
            

            db.AddRowToTable("ItemModels", this);


        }

        //Create SQL table corresponding to the items in the ItemModel that is being created
        public void CreateItemTable()
        {
            //List<string> columns = new List<string>();
            //columns.Add("id int IDENTITY(1,1)");
            //columns.Add("modelId char(10)");
            //columns.Add("modelName char(10)");
            //columns.Add("color char(10)");
            //columns.Add("size char(10)");
            //columns.Add("stock char(10)");
            //columns.Add("reserved char(10)");
            //columns.Add("sold char(10)");

            //db.CreateTable("item" + Id.ToString(), columns);

            db.CreateTable(ModelName, new Item());
        }



        public void AddItem(Item item)
        {

            db.AddRowToTable(ModelName, item);
        }
        
    }
}
