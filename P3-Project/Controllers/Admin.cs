using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using NuGet.ContentModel;
using P3_Project.Models;
using P3_Project.Models.DB;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;
using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            StorageDB DB = new();

            //DB.GetFieldType("Test", "Id");

            DB.DB.GetField("Id", "1", "Test", "Id");
            return View();
        }

        public void uploadItemModel()
        {
            Response.StatusCode = 200;
            
        }
        public ActionResult Stock()
        {
            StorageDB db = new StorageDB();
            if (!db.CheckTable("ItemModels"))
                setup();
            
            List<ItemModel> models = db.getAllElements("ItemModels", new ItemModel());

            ViewBag.model = models;
            return View();
        }

        public ActionResult AddItemModel(string id)
        {
            Console.WriteLine(id);
            if (!string.IsNullOrEmpty(id)) { 
                ItemModel model = ItemModel.LoadModel(id);
                model.LoadItems();

                ViewBag.model = model;
            }

            return View();
        }

        public ActionResult PackViewModel()
        {
            var x = 5;
            PackModel test = new PackModel(1, new StorageDB());
            test.Name = "Test";
            List<PackModel> Packs = new List<PackModel>();
            for (int i = 0; i<x; i++) {
                Packs.Add(test);
            }
            return View(test);
        }

        public ActionResult CreatePackModel()
        {
            return View();
        }

        public ActionResult EditPackModel()
        {
            return View();
        }
        
        public ActionResult EditPromoCode()
        {
            var model = new Models.PromoCode();
            return View(model);
        }

        public ActionResult PromoCode()
        {
            return View();
        }

        public ActionResult Webshop()
        {
            StorageDB db = new StorageDB();
            
            
            if(!db.DB.CheckTable("ItemModels"))
                setup();

            List<ItemModel> itemModels = db.DB.GetAllElements("ItemModels", new ItemModel());

            foreach (ItemModel itemModel in itemModels)
            {
                itemModel.items = db.DB.GetAllElements(itemModel.ItemTable, new Item());

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
            if (itemModel.Id == 0)
            {
                itemModel.Create();
            }
            else
            {
                itemModel.Update();
            }

        }

        public ActionResult deleteModel(int Id)
        {
            ItemModel.Delete(Id);
            return RedirectToAction(nameof(Stock));
        }

        private void setup()
        {
            StorageDB db = new StorageDB();

            db.DB.CreateTable("ItemModels", new ItemModel());
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
                model.Update();
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
            string subTable = db.DB.GetField("id", id, "ItemModels", "itemTable");
            db.DB.RemoveRow("ItemModels", "Id", id);
            db.DB.DeleteTable(subTable);
            
            return RedirectToAction(nameof(Webshop));
        }

        public ActionResult DeleteItem(string modelId, string id)
        {
            StorageDB db = new StorageDB();
           
            db.DB.RemoveRow("item" + modelId, "id", id);

            return RedirectToAction(nameof(Webshop));
        }

        public ActionResult ItemShowCase(string id)
        {
            StorageDB db = new StorageDB();
            return View();
        }
       
    }
}
