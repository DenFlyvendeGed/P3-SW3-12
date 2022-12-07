using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using P3_Project.Models;
using P3_Project.Models.DB;


namespace P3_Project.Utilities
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            StorageDB db = new();
            
            if(!db.DB.CheckTable("Users"))
            {
                db.DB.CreateTable("Users", new User());
                db.DB.AddRowToTable("Users", new User()
                {
                    UserName = "Admin",
                    UserPassword = "admin123"
                });
            }
            if (filterContext.HttpContext.Session.GetString("UserName") == null && filterContext.HttpContext.Request.Cookies["UserName"] != null)
            {
                if(db.DB.CheckRow("Users", "UserName", filterContext.HttpContext.Request.Cookies["UserName"]))
                    filterContext.HttpContext.Session.SetString("UserName", filterContext.HttpContext.Request.Cookies["UserName"]);
            }

            if (filterContext.HttpContext.Session.GetString("UserName") == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                                { "Controller", "Home" },
                                { "Action", "Login" }
                            });
            }
            

        }
    }
}
