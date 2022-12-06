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

        public IActionResult Login()
        {


            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //Post Action
        [HttpPost]
        public ActionResult Login(User u)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {

                if (ModelState.IsValid)
                {
                    StorageDB db = new StorageDB();
                    List<bool> exist = db.DB.ReadFromTable("Users", "UserName='" + u.UserName + "' AND UserPassword='" + u.UserPassword +"'", arr => true);
                    //var obj = db.Users.Where(a => a.UserName.Equals(u.UserName) && a.UserPassword.Equals(u.UserPassword)).FirstOrDefault();
                    //if (obj != null)
                    if(exist.Count() > 0)
                    {
                        HttpContext.Session.SetString("UserName", u.UserName.ToString());

                        //set the key value in Cookie              
                        CookieOptions option = new CookieOptions();
                        option.Expires = DateTime.Now.AddMinutes(10);
                        Response.Cookies.Append("UserName", u.UserName.ToString(), option);
                        return RedirectToAction("Index");
                    }
                    
                }
            }
            else
            {



                return RedirectToAction("Login");
            }
            return View();


        }


        private void setup()
        {
            StorageDB db = new StorageDB();

            db.DB.CreateTable("ItemModels", new ItemModel());
            db.DB.CreateTable("Tags", new Tag());
            db.DB.CreateTable("FakturaSettings", new Faktura());
        }

        //Main webshop page - Clothes
        public IActionResult Index()
        {

            string type = "Tøj";

            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();

            string userName = Request.Cookies["UserName"];
            List<ItemModel> models = db.DB.GetAllElements("ItemModels", new ItemModel(), "Type", "Tøj");
            
            ViewBag.model = models;

            return View("Index",type);
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
            List<(int, string, int, string, string)> Packs2 = new();
            Packs.ForEach(item =>
            {
                List<Tag> tags = Tag.GetAllTagsOfPackModel(item.Item1.ToString());
                string nameString = "";
                tags.ForEach(tag =>
                {
                    nameString += $" {tag.Name}";
                });
                Packs2.Add(
                    (item.Item1,
                    item.Item2,
                    item.Item3,
                    ImageModel.GetFirstImg(item.Item1, "PackModel").FilePath,
                    nameString
                    ));
            });
            return View(Packs2);
        }

        public IActionResult PackPicker([FromQuery] int? PackID)
        {
            var packmodel = PackID != null ? new PackModel((int)PackID, new StorageDB()) : new PackModel();
            return View(packmodel);
        }

        
        //Webshop page - Accessoires
        public ActionResult Accessoires()
        {
            string type = "Tilbehør";

            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();


            List<ItemModel> models = db.DB.GetAllElements("ItemModels", new ItemModel(), "Type", "Tilbehør");

            ViewBag.model = models;

            
            return View("Index", type);
        }

        public ActionResult Cart()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
