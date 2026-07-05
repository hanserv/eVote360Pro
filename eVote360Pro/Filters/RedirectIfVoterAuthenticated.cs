using eVote360Pro.Core.Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eVote360Pro.Filters
{
    public class RedirectIfVoterAuthenticated : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (actionName == "Cancel")
            {
                base.OnActionExecuting(context);
                return;
            }

            var isVoterAuthenticated = context.HttpContext.Session.Get<bool>("IsVoterAuthenticated");

            if (isVoterAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new { area = "", controller = "Voting", action = "Index" });
            }

            base.OnActionExecuting(context);
        }
    }
}
