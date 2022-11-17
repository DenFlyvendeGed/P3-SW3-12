using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using P3_Project.Models.DB;

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


        static StorageDB db = new StorageDB();
        public ItemModel()
        {
            Id = 0;
            ModelName = "";
            ModelPrice = 0;
            StockAlarm = 0;
            Description = "";
            ItemTable = "";


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

        //Get unique colors from given item model
        public List<(string, string)> GetUniqueColor()
        {
            var list = new List<(string, string)>();

            List<Item> uniqueColors = items.GroupBy(i => i.Color).Select(grp => grp.First()).ToList();

            foreach (Item item in uniqueColors)
            {
                list.Add((item.Color, item.ColorWheel));
            }
           
            return list;
        }

        //Get unique sizes from given item model
        public List<string> GetUniqueSize()
        {
            List<string> uniqueSizes = items.GroupBy(i => i.Size).Select(grp => grp.First()).Select(str => str.Size).ToList();
            
            return uniqueSizes;
        }

        //Get all items of a given size on an item model
        public Dictionary<string, int> GetAllItemsOfSize(string size)
        {

            var dict = new Dictionary<string, int>();

            List<string> columns = new List<string>() { "Color", "Stock" };
            List<List<string>> items = db.DB.GetSortedList(ItemTable, columns, "Size", size);

            items.ForEach(item => dict.Add(item[0], int.Parse(item[1])));


            return dict;
        }

        //Create SQL table corresponding to the items in the ItemModel that is being created
        public void CreateItemTable()
        {


            db.DB.CreateTable(ItemTable, new Item());
        }

        public static ItemModel LoadModel(string id)
        {
            return db.DB.GetRow("ItemModels", new ItemModel(), id);
        }

        public void LoadItems()
        {
            
            items = db.DB.GetAllElements(ItemTable, new Item());
        }
       
        public void AddItem(Item item)
        {

            db.DB.AddRowToTable(ItemTable, item);
        }
        
        //Update exsisting item model values
        public void Update()
        {
            ItemTable = "Item" + Id;

            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "ModelPrice", ModelPrice.ToString());
            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "ModelName", ModelName);
            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "StockAlarm", StockAlarm.ToString());
            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "Description", Description);

            db.DB.DeleteTable(ItemTable);
            CreateItemTable();
            if (items != null)
            {
                foreach (Item item in items)
                {
                    item.ModelId = Id.ToString();
                    AddItem(item);
                }
            }

        }

        //Delete exsisting table
        public void Delete()
        {
            db.DB.DeleteTable(ItemTable);
            db.DB.RemoveRow("ItemModels", "Id", Id.ToString());


        }

        public static void Delete(int Id)
        {
            string ItemTable = db.DB.GetField("Id", Id.ToString(), "ItemModels", "ItemTable");
            db.DB.DeleteTable(ItemTable);
            db.DB.RemoveRow("ItemModels", "Id", Id.ToString());
        }
    }
}
