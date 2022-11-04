using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System.ComponentModel;

using P3_Project.Models.DB;

namespace P3_Project.Models
{
 
    public class ItemModel
    {

        public int Id { get; set; }
        public string ModelName { get; set; }
        public string? ItemTable { get; set; }
        public string Description { get; set; }
        public string? Colors { get; set; }
        public string? Sizes { get; set; }

        public List<Item>? items;

        StorageDB db = new StorageDB();
        public ItemModel()
        {
            Id = 0;
            ModelName = "";
            
            Description = "";
            
        }

        //Adds the ItemModel instance to the list of Itemmodel's int the SQL table
        public void Create()
        {
            
            if(db.DB.CheckRow("ItemModels", "ModelName", ModelName)){
                throw new Exception("Item already exist");
            }
            db.DB.AddRowToTable("ItemModels", this);
            Id = int.Parse(db.DB.GetField("modelName", this.ModelName, "ItemModels", "Id"));
            ItemTable = "Item" + Id;
            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "ItemTable", ItemTable);
        }

        //Create SQL table corresponding to the items in the ItemModel that is being created
        public void CreateItemTable()
        {


            db.DB.CreateTable(ItemTable, new Item());
        }



        public void AddItem(Item item)
        {

            db.DB.AddRowToTable(ItemTable, item);
        }
        
        public void Delete()
        {
            db.DB.DeleteTable(ItemTable);
            db.DB.RemoveRow("ItemModels", "Id", Id.ToString());
        }
    }
}
