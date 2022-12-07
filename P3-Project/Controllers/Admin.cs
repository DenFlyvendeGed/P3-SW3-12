using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using NuGet.ContentModel;
using P3_Project.Models;
using P3_Project.Models.DB;
using P3_Project.Models.Orders;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;
using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using System.Web.Http;

using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using P3_Project.Utilities;

namespace P3_Project.Controllers
{
    [Authentication]
    public class Admin : Controller
    {
        // GET: Admin 
        public ActionResult Index()
        {

            return RedirectToAction("Stock", "Admin");
        }


        //Create starting database
        private void setup()
        {
            StorageDB db = new StorageDB();

            db.DB.CreateTable("ItemModels", new ItemModel());
            db.DB.CreateTable("Tags", new Tag());
            db.DB.CreateTable("FakturaSettings", new Faktura());

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


        public ActionResult PackViewModel()
        {
            
            return View(PsudoPackModel.GetAllFromDatabase(new StorageDB()));
        }

        public ActionResult CreatePackModel([FromQuery] int? PackID)
        {
            var db = new StorageDB();
            List<(int, string)> Items;
            try
            {
                Items = db.DB.ReadFromTable("ItemModels", new string[] { "Id", "ModelName" }, (r) => ((int)r[0], (string)r[1]));
            }
            catch
            {
                Items = new List<(int, string)> { };
            }
            var packmodel = PackID != null ? new PackModel((int)PackID, new StorageDB()) : new PackModel();
            List<(int, string, string)> Items2 = new();
            Items.ForEach(item =>
            {
                Items2.Add((item.Item1, item.Item2, ImageModel.GetFirstImg(item.Item1).FilePath));
            });
            if(packmodel.PackID != null) { 
                packmodel.LoadTags();
                packmodel.LoadImages();
            }
            var model = (packmodel, Items2);
            return View(model);
        }

        #endregion

        #region PromoCode
       
        public ActionResult EditPromoCode([FromQuery] int? Id)
        {
            var model = Id != null ? new PromoCode((int)Id, new StorageDB()) : new PromoCode();

            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();

            List<ItemModel> models = db.DB.GetAllElements("ItemModels", new ItemModel());
            List<(int, string)> packs = db.DB.ReadFromTable("PackModel",new string[] {"Id","Name"}, r => ((int)r[0],(string)r[1]));

            ViewBag.model = models;


            return View((model, models, packs));
        }

        public ActionResult PromoCode()
        {

			List<(int, string, DateTime, int, PromoCodeDiscountType, PromoCodeItemType)> psudoCodes;
			try {
				psudoCodes = new StorageDB().DB.ReadFromTable("PromoCode", new string[] {"Id", "Code", "ExpirationDate", "Value", "DiscountType", "ItemType" }, (r) => ((int)r[0], (string)r[1], (DateTime)r[2], (int)r[3], (PromoCodeDiscountType)(short)r[4], (PromoCodeItemType)(short)r[5]));
			} catch {
				psudoCodes = new();
			}
            return View(psudoCodes);
        }
        
		public ActionResult Settings()
        {


            return View();
        }

        #endregion

        #region ConfirmSale

        public ActionResult ConfirmSale([FromQuery]int id)
        {
            Order order = P3_Project.Models.Orders.Globals.OrderDB.Fetch(id);
			Console.WriteLine(order.Email);
			Console.WriteLine("Count: " + order.ShopUnits.Count());
			Console.WriteLine("Price: " + order.Price);
            return View(order);
        }

        #endregion

        #region Settings

        public ActionResult FakturaSettings()
        {
            StorageDB db = new StorageDB();

            List<Faktura> models = db.DB.GetAllElements("FakturaSettings", new Faktura());

            ViewBag.model = models;

            return View();
        }
        #endregion

    }
}
