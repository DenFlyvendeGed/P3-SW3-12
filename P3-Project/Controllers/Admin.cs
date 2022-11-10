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
            //DB.CreateItemTable("Test");
            //DB.CreateItemTable("Item");
            //DB.CreateItemTable("Slet");

            //DB.CheckTable("Ingenting");
            //DB.CheckTable("Test");

            //DB.DeleteTable("Slet");

            //DB.CreatePromoCode();

            //DB.AddItem("Item", "1");
            //DB.AddItem("Item", "2");

            //DB.RemoveItem("Item", "2");

            //DB.CheckRow("Test", "id", "1");
            //DB.CheckRow("Test", "id", "5");
            return View();
        }

        public ActionResult Stock()
        {
            return View();
        }

        public ActionResult AddItemModel()
        {
            return View();
        }

        public ActionResult PackViewModel()
        {
            var x = 5;
            PackModel test = new PackModel(1);
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
            var model = new Models.PromoCode("It's a promo code");
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


        private void setup()
        {
            StorageDB db = new StorageDB();


            //List<string> param = new List<string>();
            //param.Add("Id int");

            //param.Add("ModelName varchar(30)");
            //param.Add("ItemTable varchar(10)");
            //param.Add("Description varchar(30)");
            //param.Add("Colors varchar(10)");
            //param.Add("Sizes varchar(10)");

            //db.CreateTable("ItemModels", param);

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

            
                
            model.Create();
            model.CreateItemTable();

                    
                //List<(string, string)> modelColumns = new List<(string, string)>();
                //modelColumns.Add(("id", id.ToString()));
                //modelColumns.Add(("modelName", modelName.ToString()));
                //modelColumns.Add(("description", description.ToString()));


                //db.AddRowToTable("ItemModels", modelColumns);
            



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

        //hej

        [HttpPost]
        public void CreateItem(Item item) // updateres til at sende item med.
        {
            if (!ModelState.IsValid )
                BadRequest("Form has to be filled out");

            //Item item2 = new Item();
            //item2.Color = Request.Headers["color"];
            //item2.Size = Request.Headers["size"];
            //item2.ModelId = Request.Headers["id"];

            item.Create();

            //StorageDB db = new StorageDB();

            //string table = db.GetField("Id", item.ModelId, "ItemModels", "ItemTable");
            //db.AddRowToTable(table, item);

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


        //// GET: Admin/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Admin/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Admin/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Admin/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Admin/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        ////// GET: Admin/Delete/5
        ////public ActionResult Delete(int id)
        ////{
        ////    return View();
        ////}

        //// POST: Admin/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
