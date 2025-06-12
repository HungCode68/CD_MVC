using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NailMVC.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Nếu chưa đăng nhập thì chuyển hướng về Login
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại." });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
