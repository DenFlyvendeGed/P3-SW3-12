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



        public ActionResult Webshop()
        {

            

            StorageDB db = new StorageDB();
            
            
            if(!db.CheckTable("itemModels"))
                setup();

            List<ItemModel> itemModels = db.getAllElementsTest("itemModels", new ItemModel());

            foreach (ItemModel itemModel in itemModels)
            {
                itemModel.items = db.getAllElementsTest(itemModel.itemTable, new Item());

            };


            //List<IDictionary<string, object>> itemModels2 = db.getAllElements("itemModels", new ItemModel());
            //int i = 0;
            //foreach (Dictionary<string, object> itemModel in itemModels2)
            //{
            //    List<IDictionary<string, object>> items = db.getAllElements((string)itemModel["itemTable"], new Item());
            //    itemModels2[i].Add("items", items);
            //    _ = items;
            //    i++;
            //};


            //foreach(object obj in itemModels){
            //    ItemModel item = (ItemModel)obj;
            //    items.Add(db.getAllElements(item.itemTable , new Item()));
            //}


            ViewBag.itemModels = itemModels;
            return View();
        }


        private void setup()
        {
            StorageDB db = new StorageDB();


            List<string> param = new List<string>();
            param.Add("id nchar(10)");
            param.Add("modelName nchar(30)");
            param.Add("itemTable nchar(10)");
            param.Add("description nchar(30)");
            param.Add("colors nchar(10)");
            param.Add("sizes nchar(10)");

            db.CreateTable("itemModels", param);


        }


        public ActionResult CreateItemModel()
        {

            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateItemModel( string modelName, string id, string description)
        {

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            StorageDB db = new StorageDB();

            if (db.CheckRow("ItemModels", "id", id.ToString()))
            {
                throw new Exception("ItemModel already exist");
            }
            else
            {
                if(!db.CheckTable(id.ToString()))
                { 
                    List<string> columns = new List<string>();
                    columns.Add("id int IDENTITY(1,1)");
                    columns.Add("modelId char(10)");
                    columns.Add("modelName char(10)");
                    columns.Add("color char(10)");
                    columns.Add("size char(10)");
                    columns.Add("stock char(10)");
                    columns.Add("reserved char(10)");
                    columns.Add("sold char(10)");

                    db.CreateTable("item" + id.ToString(), columns);

                    List<(string, string)> modelColumns = new List<(string, string)>();
                    modelColumns.Add(("id", id.ToString()));
                    modelColumns.Add(("modelName", modelName.ToString()));
                    modelColumns.Add(("description", description.ToString()));
                    db.AddRowToTable("ItemModels", modelColumns);
                }
                else
                {
                    throw new Exception("Item table already exist");
                }
            }


            return Redirect("Webshop");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncreaseStock(string modelId, string id)
        {

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            StorageDB db = new StorageDB();

            if (db.CheckRow("item" + modelId, "id", id))
            {
                db.increaseItemStock("item" + modelId, id);
            }
            else
            {
                throw new Exception("Item Dosent exist");
            }
            
            return Redirect("Webshop");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DecreaseStock(string modelId, string id)
        {

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            StorageDB db = new StorageDB();

            if (db.CheckRow("item" + modelId, "id", id))
            {
                db.DecreaseItemStock("item" + modelId, id);
            }
            else
            {
                throw new Exception("Item Dosent exist");
            }

            return Redirect("Webshop");
        }

        //hej

        [HttpPost]
        public void CreateItem()
        {
            Item item = new Item();
            item.color = Request.Headers["color"];
            item.size = Request.Headers["size"];
            item.modelId = Request.Headers["id"];



            StorageDB db = new StorageDB();

            
            db.AddRowToTableTest("item" + item.modelId, item);

        }

        public ActionResult Delete(string id)
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
