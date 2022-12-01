using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace P3_Project.Utilities
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            
            if(filterContext.HttpContext.Request.Cookies["UserName"] != null) 
            {
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
