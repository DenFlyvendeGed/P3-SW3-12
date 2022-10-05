using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using NuGet.ContentModel;
using P3_Project.Models;

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
            DB.CreateItemTable("Test");
            DB.CreateItemTable("Item");
            DB.CreateItemTable("Slet");

            DB.CheckTable("Ingenting");
            DB.CheckTable("Test");

            DB.DeleteTable("Slet");

            DB.AddItem("Item", 1);
            DB.AddItem("Item", 2);

            DB.RemoveItem("Item", 2);

            DB.CheckRow("Test", "id", "1");
            DB.CheckRow("Test", "id", "5");
            return View();
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

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

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
