using Microsoft.AspNetCore.Mvc.Filters;
using NailMVC.Services;
using System.Security.Claims;
using System.Threading.Tasks;
namespace NailMVC.Filters
{
    public class ActionLoggingFilter : IAsyncActionFilter
    {
        private readonly ActivityLogger _logger;

        public ActionLoggingFilter(ActivityLogger logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Tiếp tục thực thi action
            var executedContext = await next();

            if (executedContext.Exception == null)
            {
                var httpContext = context.HttpContext;
                var userIdStr = httpContext.Session.GetString("TaiKhoanId");

                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int taiKhoanId))
                {
                    var actionName = context.ActionDescriptor.RouteValues["action"];
                    var controllerName = context.ActionDescriptor.RouteValues["controller"];
                    var method = httpContext.Request.Method;

                    string hoatDong = $"{method} - {controllerName}/{actionName}";
                    await _logger.LogAsync(taiKhoanId, $"Truy cập {controllerName}/{actionName}");
                }
            }

        }
    }
}
