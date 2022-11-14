﻿using Microsoft.AspNetCore.Http;
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
using System.Web.Http;

using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace P3_Project.Controllers
{
    public class Admin : Controller
    {
        // GET: Admin 
        public ActionResult Index()
        {
            return View("Stock");
        }

        //Create starting database
        private void setup()
        {
            StorageDB db = new StorageDB();

            db.DB.CreateTable("ItemModels", new ItemModel());
        }

        #region ItemModel
        //Load stock page
        public ActionResult Stock()
        {
            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();
            
            List<ItemModel> models = db.DB.GetAllElements("ItemModels", new ItemModel());

            ViewBag.model = models;
            return View();
        }

        //Add or edit item model
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
        #endregion

        #region PackModel
        //
        public ActionResult PackViewModel()
        {
            var x = 5;
            PackModel test = new PackModel();
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

        #endregion

        #region PromoCode
        public ActionResult EditPromoCode()
        {
            var model = new Models.PromoCode();
            return View(model);
        }

        public ActionResult PromoCode()
        {
            return View();
        }

        #endregion

       
    }
}
