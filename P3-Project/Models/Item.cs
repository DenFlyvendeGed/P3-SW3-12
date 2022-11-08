using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace P3_Project.Models
{
    public class Item
    {
        
        public int Id { get; set; }
        private string _modelId { get; set; }
        public string ModelId
        {
            get
            {
                return _modelId;
            }
            set
            {
                _modelId = value.Trim();
            }
        }
        public string Color { get; set; }
        public string ColorWheel { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        private int Reserved { get; set; }
        private int Sold { get; set; }

        private StorageDB db = new StorageDB();
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

        public void ChangeStock(int amount)
        {
            string table = "Item" + ModelId;
            if (db.CheckRow(table, "Id", Id.ToString()))
            {
                if(Stock + amount < 0)
                {
                    throw new Exception("Cant reduce stock below 0");
                }
                db.UpdateField(table, "Id", Id.ToString(), "Stock", (Stock + amount).ToString());
            }
            else
            {
                throw new Exception("Item Dosent exist");
            }
        }

        public void Create()
        {
            string table = "item" + ModelId;
            db.AddRowToTable(table, this);

        }

        public void Delete()
        {
            db.RemoveRow("item" + ModelId, "Id", Id.ToString());
        }

    }
}
