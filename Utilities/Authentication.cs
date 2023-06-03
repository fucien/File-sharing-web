using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace web_ver_2.Utilities
{
	public class Authentication : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{


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
