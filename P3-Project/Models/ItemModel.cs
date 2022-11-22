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
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using Image = System.Drawing.Image;
using System.Text.RegularExpressions;

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
        public List<Item>? items { get; set; }

        public string Type { get; set; }

        public List<Image>? Pictures { get; set; }

        public List<Tag>? Tags { get; set; }
       

        static StorageDB db = new StorageDB();
        

        public ItemModel()
        {
            Id = 0;
            ModelName = "";
            ModelPrice = 0;
            StockAlarm = 0;
            Description = "";
            ItemTable = "";

            Pictures = new();
            Description = "";
        }

        

        //Adds the ItemModel instance to the list of Itemmodel's int the SQL table
        public void Create()
        {

            if (db.DB.CheckRow("ItemModels", "ModelName", ModelName))
            {
                throw new Exception("Item already exist");
            }
            db.DB.AddRowToTable("ItemModels", this);
            Id = int.Parse(db.DB.GetField("modelName", this.ModelName, "ItemModels", "Id"));
            ItemTable = "Item" + Id;
            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "ItemTable", ItemTable);
            CreateItemTable();
            CreateTagTable();
            if (items != null)
            {
                foreach (Item item in items)
                {
                    item.ModelId = Id.ToString();
                    AddItem(item);
                }
            }
            if (Pictures != null)
            {
                foreach (Image item in Pictures)
                {
                    if (item.Data != null)
                        item.Save(Id);
                }
            }
            if (Tags != null)
            {
                foreach(Tag tag in Tags)
                {
                    if (db.DB.CheckRow("Tags", "Id", tag.Id.ToString()))
                    {
                        Tag dbTag = db.DB.GetRow("Tags", new Tag(), tag.Id.ToString());
                        db.DB.AddRowToTable($"ItemModel_{Id}_Tags", dbTag);
                    }
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

        private void CreateTagTable()
        {

            db.DB.CreateTable($"ItemModel_{Id}_Tags", new Tag());
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
        public void LoadImages()
        
        {
            DirectoryInfo dir = Image.GetDir(Id);

            foreach(FileInfo file in dir.GetFiles())
            {
                Image img = new();
                img.Name = file.Name;
                img.Type = file.Extension;
                //img.Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(file.DirectoryName, file.Name)));
                //img.Data = $"data:image/{img.Type};base64,{img.Data}";
                img.FilePath = img.GetFilePath(Id);
                Pictures.Add(img);
            }
        }
        public Image GetFirstImg()
        {
            DirectoryInfo dir = Image.GetDir(Id);

            foreach (FileInfo file in dir.GetFiles())
            {
                Image img = new();
                img.Name = file.Name;
                img.Type = file.Extension.Replace(".","");
                //img.Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(file.DirectoryName, file.Name)));
                //img.Data = $"data:image/{img.Type};base64,{img.Data}";
                img.FilePath = img.GetFilePath(Id);
                return img;
            }
            return null;
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
            db.DB.UpdateField("ItemModels", "Id", Id.ToString(), "Type", Type);



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
            if(Pictures != null)
            {
                foreach (Image item in Pictures)
                {
                    if(item.Data != null)
                        item.Save(Id);
                }
            }

        }

        //Delete exsisting table
        public void Delete()
        {
            db.DB.DeleteTable(ItemTable);
            db.DB.DeleteTable($"ItemModels_{Id}_Tags");
            db.DB.RemoveRow("ItemModels", "Id", Id.ToString());

            DirectoryInfo dir = Image.GetDir(Id);
            dir.Delete(true);
        }

        public static void Delete(int Id)
        {
            string ItemTable = db.DB.GetField("Id", Id.ToString(), "ItemModels", "ItemTable");
            db.DB.DeleteTable(ItemTable);
            db.DB.DeleteTable($"ItemModels_{Id}_Tags");
            db.DB.RemoveRow("ItemModels", "Id", Id.ToString());

            DirectoryInfo dir = Image.GetDir(Id);
            dir.Delete(true);
        }

        //Get 
        public int GetStockTotal()
        {
            int result = db.DB.GetStockAmount(ItemTable);


            return result;
        }
        

    }

    public class Image
    {
        public string? Name { get; set; }
        public int? Size { get; set; }
        public string? Type { get; set; }
        public string? Data { get; set; }
        public string? FilePath { get; set; }

        //Save Image under 'wwwroot/Pictures/{Id}/{Filename}'
        public void Save(int id)
        {
            string projectPath = Directory.GetCurrentDirectory();
            string folderName = Path.Combine(projectPath, "wwwroot" ,"Pictures\\" + id);
            DirectoryInfo dir = Directory.CreateDirectory(folderName);
            string fileName = Path.Combine(dir.FullName, Name);
            
            string base64Data = Regex.Replace(Data, "^data:image\\/(png|jpeg);base64,", "");
            //Image img = LoadBase64(base64Data);
            File.WriteAllBytes(fileName, Convert.FromBase64String(base64Data));
        }

        ///<summary>
        /// Acces Directory of a ItemModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Directory of ItemModel picture folder</returns>
        public static DirectoryInfo GetDir(int id)
        {
            string projectPath = Directory.GetCurrentDirectory();
            string folderName = Path.Combine(projectPath, "wwwroot" , "Pictures\\" + id);
            return Directory.CreateDirectory(folderName);
        }

        /// <summary>
        /// Only works if 'Image.Name' has been set.
        /// Get filepath for frontend reading. wwwroot is leftout given that it shouldnt be included in the frontend.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Filepath for browser accesing of Image</returns>
        public string GetFilePath(int id)
        {
            string projectPath = Directory.GetCurrentDirectory();
            string fileName = Path.Combine( "/Pictures", id.ToString() , Name);
            fileName = Regex.Replace(fileName, "\\\\", "/");
            //fileName = fileName.Replace(@"\\", "/");
            this.FilePath = fileName;
            return fileName;
        }
    }
}
