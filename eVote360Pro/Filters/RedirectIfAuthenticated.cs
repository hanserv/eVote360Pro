using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eVote360Pro.Filters
{
    public class RedirectIfAuthenticated : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Session.Get<UserViewModel>("User");

            if(user is not null)
            {
                context.Result = user.Role switch
                {
                    UserRole.Admin => new RedirectToRouteResult(new { area = "Admin", controller = "Home", action = "Index" }),
                    UserRole.PoliticalLeader => context.Result = new RedirectToRouteResult(new { area = "PoliticalLeader", controller = "Home", action = "Index" }),
                    _ => default
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
