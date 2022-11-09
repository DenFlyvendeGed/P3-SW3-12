using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System.ComponentModel;

namespace P3_Project.Models
{
 
    public class ItemModel
    {

        public int Id { get; set; }

        public string ModelName { get; set; }
        public string ItemTable { get; set; }
        public string Description { get; set; }

        public int ModelPrice { get; set; }
        public int StockAlarm { get; set; }
        //public string? Colors { get; set; }
        //public string? Sizes { get; set; }


        public List<Item>? items { get; set; }


        StorageDB db = new StorageDB();
        public ItemModel()
        {
            Id = 0;
            ModelName = "";
            ModelPrice = 0;
            StockAlarm = 0;
            Description = "";
            ItemTable = "";


        }

        //Adds the ItemModel instance to the list of Itemmodel's int the SQL table
        public void Create()
        {
            
            if(db.CheckRow("ItemModels", "ModelName", ModelName)){
                throw new Exception("Item already exist");
            }
            db.AddRowToTable("ItemModels", this);
            Id = int.Parse(db.GetField("modelName", this.ModelName, "ItemModels", "Id"));
            ItemTable = "Item" + Id;
            db.UpdateField("ItemModels", "Id", Id.ToString(), "ItemTable", ItemTable);
            CreateItemTable();
            if(items != null)
            {
                foreach (Item item in items)
                {
                    item.ModelId = Id.ToString();
                    AddItem(item);
                }
            }
           
        }

        //Skal fikses, der er placeholders i
        public List<(string, string)> GetUniqueColor()
        {
            var list = new List<(string, string)>();
            //Sql til at hente data her...
            list.Add(("Rød", "#ff0000"));
            list.Add(("Blå", "#0008ff"));
            list.Add(("Sort", "000000"));
            return list;
        }

        //Skal fikses, der er placeholders i
        public List<string> GetUniqueSize()
        {
            var list = new List<string>();
            //Sql til at hente data her...

            list.Add("S");
            list.Add("M");
            list.Add("L");
            return list;
        }

        //Skal fikses, der er placeholders i
        public Dictionary<string, int> GetAllItemsOfSize(string size)
        {
            
            var dict = new Dictionary<string, int>();

            dict.Add("Rød", 1);
            dict.Add("Blå", 2);
            dict.Add("Sort", 3);

            return dict;
        }



        //Create SQL table corresponding to the items in the ItemModel that is being created
        public void CreateItemTable()
        {


            db.CreateTable(ItemTable, new Item());
        }



        public void AddItem(Item item)
        {

            db.AddRowToTable(ItemTable, item);
        }
        
        public void Delete()
        {
            db.DeleteTable(ItemTable);
            db.RemoveRow("ItemModels", "Id", Id.ToString());
        }
    }
}
