using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using NuGet.ContentModel;
using P3_Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;
using System.Dynamic;
using System.Text.Json;


namespace P3_Project.Controllers
{
    public class Admin : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Storage()
        {
            StorageDB DB = new StorageDB();

            DB.getFieldType("Test", "Id");

            DB.GetField("Id", "1", "Test", "Id");
            return View();
        }

        public ActionResult Stock()
        {
            return View();
        }

        public ActionResult AddItemModel()
        {
            /*
            string jsonData = "{\"ModelName\":\"Bukser\",\"ModelPrice\":1000,\"items\":[{\"Color\":\"Rød\",\"ColorWheel\":\"#ff0000\",\"Size\":\"S\",\"Stock\":1},{\"Color\":\"Blå\",\"ColorWheel\":\"#0008ff\",\"Size\":\"S\",\"Stock\":2},{\"Color\":\"Sort\",\"ColorWheel\":\"000000\",\"Size\":\"S\",\"Stock\":3},{\"Color\":\"Rød\",\"ColorWheel\":\"#ff0000\",\"Size\":\"M\",\"Stock\":4},{\"Color\":\"Blå\",\"ColorWheel\":\"#0008ff\",\"Size\":\"M\",\"Stock\":5},{\"Color\":\"Sort\",\"ColorWheel\":\"000000\",\"Size\":\"M\",\"Stock\":6},{\"Color\":\"Rød\",\"ColorWheel\":\"#ff0000\",\"Size\":\"L\",\"Stock\":7},{\"Color\":\"Blå\",\"ColorWheel\":\"#0008ff\",\"Size\":\"L\",\"Stock\":8},{\"Color\":\"Sort\",\"ColorWheel\":\"000000\",\"Size\":\"L\",\"Stock\":9}],\"StockAlarm\":111}";

            ItemModel deptObj = JsonSerializer.Deserialize<ItemModel>(jsonData);
            ViewBag.model = deptObj;
            */
            
            return View();
        }

        public ActionResult EditPackModel()
        {
            return View();
        }

        public ActionResult PromoCode()
        {
            return View();
        }

        public ActionResult Webshop()
        {
            StorageDB db = new StorageDB();
            
            if(!db.CheckTable("ItemModels"))
                setup();

            List<ItemModel> itemModels = db.getAllElements("itemModels", new ItemModel());

            foreach (ItemModel itemModel in itemModels)
            {
                itemModel.items = db.getAllElements(itemModel.ItemTable, new Item());

            };

            ViewBag.itemModels = itemModels;
            return View();
        }

        [HttpPut]
        async public void ItemModelTable(ItemModel test)
        {
            string jsonData = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                jsonData = await reader.ReadToEndAsync();
            }

            Console.WriteLine(jsonData);

            ItemModel itemModel = JsonSerializer.Deserialize<ItemModel>(jsonData);
            itemModel.Create();
        }


        private void setup()
        {
            StorageDB db = new StorageDB();
            db.CreateTable("ItemModels", new ItemModel());
        }


        public ActionResult CreateItemModel()
        {
            ItemModel model = new ItemModel();
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateItemModel( string modelName, string description, ItemModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            StorageDB db = new StorageDB();
            
            if(model.Id == 0)
            {
                model.Create();
            }
            else
            {
                throw new NotImplementedException();
            }

            return Redirect("Webshop");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncreaseStock(string modelId, string id, Item item)
        {

            if (!ModelState.IsValid)
                return BadRequest("Form has to be filled out");
            StorageDB db = new StorageDB();
            try
            {
                item.ChangeStock(1);    
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            
            return Redirect("Webshop");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DecreaseStock(string modelId, string id, Item item)
        {

            if (!ModelState.IsValid)
                return BadRequest("Form has to be filled out");
            StorageDB db = new StorageDB();
            try
            {

                item.ChangeStock(-1);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Redirect("Webshop");
        }

        [HttpPost]
        public void CreateItem(Item item) 
        {
            if (!ModelState.IsValid )
                BadRequest("Form has to be filled out");

            item.Create();

            Redirect("Webshop");
        }

        public ActionResult Delete(string id, Item item)
        {
            StorageDB db = new StorageDB();
            string subTable = db.GetField("id", id, "ItemModels", "itemTable");
            db.RemoveRow("ItemModels", "id", id);
            db.DeleteTable(subTable);
            
            return RedirectToAction(nameof(Webshop));
        }

        public ActionResult DeleteItem(string modelId, string id)
        {
            StorageDB db = new StorageDB();

            db.RemoveRow("item" + modelId, "id", id);

            return RedirectToAction(nameof(Webshop));
        }
        public ActionResult ItemShowCase(string id)
        {
            StorageDB db = new StorageDB();
            return View();
        }
       
    }
}
