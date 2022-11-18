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
                model.LoadImages();

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
       
        public ActionResult EditPromoCode([FromQuery] int? Id)
        {
            var model = Id != null ? new PromoCode((int)Id, new StorageDB()) : new PromoCode();
            return View(model);
        }

        public ActionResult PromoCode()
        {
			var psudoCodes = new StorageDB().DB.ReadFromTable("PromoCode", new string[] {"Id", "Code", "ExpirationDate"}, (r) => ((int)r[0], (string)r[1], (DateTime)r[2]));
            return View(psudoCodes);
        }

        //public ActionResult Webshop()
        //{
        //    StorageDB db = new StorageDB();
            
            
        //    if(!db.DB.CheckTable("ItemModels"))
        //        setup();

        //    List<ItemModel> itemModels = db.DB.GetAllElements("ItemModels", new ItemModel());

        //    foreach (ItemModel itemModel in itemModels)
        //    {
        //        itemModel.items = db.DB.GetAllElements(itemModel.ItemTable, new Item());

        //    };

        //    ViewBag.itemModels = itemModels;
        //    return View();
        //}

        [HttpPut]
        async public void ItemModelTable(ItemModel test)
        {
            //string jsonData = string.Empty;
            //using (var reader = new StreamReader(Request.Body))
            //{
            //    jsonData = await reader.ReadToEndAsync();
            //}

            //Console.WriteLine(jsonData);
            //JsonSerializer.Deserialize<ItemModel>(jsonData);
            ItemModel itemModel = test;
            if (itemModel.Id == 0)
            {
                itemModel.Create();
            }
            else
            {
                itemModel.Update();
            }

        }

        //public ActionResult deleteModel(int Id)
        //{
        //    ItemModel.Delete(Id);
        //    return RedirectToAction(nameof(Stock));
        //}



        public ActionResult CreateItemModel()
        {
            ItemModel model = new ItemModel();
            return View(model);
        }

        //public ActionResult Delete(string id, Item item)
        //{
        //    StorageDB db = new StorageDB();
        //    string subTable = db.DB.GetField("id", id, "ItemModels", "itemTable");
        //    db.DB.RemoveRow("ItemModels", "Id", id);
        //    db.DB.DeleteTable(subTable);

        //    return RedirectToAction(nameof(Webshop));
        //}

        //public ActionResult DeleteItem(string modelId, string id)
        //{
        //    StorageDB db = new StorageDB();

        //    db.DB.RemoveRow("item" + modelId, "id", id);

        //    return RedirectToAction(nameof(Webshop));
        //}
        #endregion
        public ActionResult ItemShowCase(string id)
        {
            StorageDB db = new StorageDB();
            return View();
        }
    }
}
