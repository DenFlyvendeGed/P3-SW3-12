using Microsoft.AspNetCore.Mvc;
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
        }

        public IActionResult Index()
        {

            StorageDB db = new StorageDB();
            if (!db.DB.CheckTable("ItemModels"))
                setup();

            string userName = Request.Cookies["UserName"];
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
            return View();
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
