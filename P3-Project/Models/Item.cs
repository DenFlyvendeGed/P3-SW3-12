using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

using P3_Project.Models.DB;

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
        public int Reserved { get; set; }
        public int Sold { get; set; }

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


		public void NotifyStockAlarm(int modelId){
			var item = db.DB.GetRow("ItemModels", new ItemModel(), modelId.ToString());
			if(item.StockAlarm >= Stock - Reserved){
				new Mail.MailClient()
					.ToList(new Mail.MailList(db))
					.Subject("Lavt Lagerbeholdningen")
					.SetHtml(true)
					.Body($@"
<h1> Lavt lager indhold på gendstand {item.ModelName} i størrelse {Size} og farve {Color} </h1>
<p> Der er blevet placeret en ordre der når den hentes vil sende Lagerbeholdningen af {item.ModelName} under den ønskede værdi </p>
					")
					.SendMail();
			}
		}
		

		void ChangeField(int initial, int amount, string field){
            string table = "Item" + ModelId;
            if (db.DB.CheckRow(table, "Id", Id.ToString()))
            {
                if(initial + amount < 0)
                {
                    throw new Exception("Cant reduce stock below 0");
                }
                db.DB.UpdateField(table, "Id", Id.ToString(), field, (initial + amount).ToString());
            }
            else
            {
                throw new Exception("Item Dosent exist");
            }
			
		}


        public void ChangeStock(int amount)
        {
			ChangeField(Stock, amount, "Stock");
        }

		public void ChangeReservated(int amount) {
			ChangeField(Reserved, amount, "Reserved");
		}

		public void ChangeSold(int amount) {
			ChangeField(Sold, amount, "Sold");
		}

        public void Create()
        {
            string table = "Item" + ModelId;
            db.DB.AddRowToTable(table, this);

        }

        public void Delete()
        {
            db.DB.RemoveRow("item" + ModelId, "Id", Id.ToString());
        }

    }
}
