using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eVote360Pro.Filters
{
    public class SessionAuthorize : ActionFilterAttribute
    {
        private readonly UserRole[] _roles;
        public SessionAuthorize(params UserRole[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Session.Get<UserViewModel>("User");

            if(user is null || _roles.Any() && !_roles.Contains(user.Role))
            {
                context.Result = new RedirectToRouteResult(new { area = "", controller = "Auth", action = "AccessDenied" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
