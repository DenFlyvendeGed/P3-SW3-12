using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Utilities;
using P3_Project.Models;
using P3_Project.Models.DB;
using System.Diagnostics;

namespace P3_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
			
        }

        private void setup()
        {
            StorageDB db = new StorageDB();

            db.DB.CreateTable("ItemModels", new ItemModel());
            db.DB.CreateTable("Tags", new Tag());
        }

        public IActionResult Index()
        {

            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();
            

            List<ItemModel> models = db.DB.GetAllElements("ItemModels", new ItemModel(), "Type", "Tøj");

            ViewBag.model = models;

            return View();
        }

        public IActionResult ShowItemModel(string id)
        {
            ItemModel models = ItemModel.LoadModel(id);
            models.LoadItems();
            models.LoadImages();
            ViewBag.item = models;
            return View();
        }



        public IActionResult PackModels()
        {
            var db = new StorageDB();
            List<(int, string, int)> Packs;
            try
            {
                Packs = db.DB.ReadFromTable("PackModel", new string[] { "Id", "Name", "Price" }, (r) => ((int)r[0], (string)r[1], (int)r[2]));
            }
            catch {
                Packs = new List<(int, string, int)> { };
            }

            return View(Packs);
        }
        public IActionResult PackPicker([FromQuery] int? PackID)
        {
            var packmodel = PackID != null ? new PackModel((int)PackID, new StorageDB()) : new PackModel();
            return View(packmodel);
        }

        public ActionResult Accessoires()
        {
            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();


            List<ItemModel> models = db.DB.GetAllElements("ItemModels", new ItemModel(), "Type", "Tilbehør");

            ViewBag.model = models;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
