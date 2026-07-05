using eVote360Pro.Core.Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eVote360Pro.Filters
{
    public class RequireVoterAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isVoterAuthenticated = context.HttpContext.Session.Get<bool>("IsVoterAuthenticated");

            if (!isVoterAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new { area = "", controller = "VoterIdentity", action = "Index" });
            }

            base.OnActionExecuting(context);
        }
    }
}
