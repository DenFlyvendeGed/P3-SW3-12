using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using NuGet.ContentModel;
using P3_Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


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
            List<string> itemModels = db.getAllElements("itemModels", "modelName");
            ViewBag.itemModels = itemModels;
            return View();
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
                    columns.Add("id char(10)");
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


        public ActionResult Delete(string id)
        {

            StorageDB db = new StorageDB();
            string subTable = db.GetField("modelName", id, "ItemModels", "itemTable");
            db.RemoveRow("ItemModels", "modelName", id);
            db.DeleteTable(subTable);
            
            return RedirectToAction(nameof(Webshop));
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //// GET: Admin/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
