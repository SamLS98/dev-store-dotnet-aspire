using DevStore.Core.Communication;
using Microsoft.AspNetCore.Mvc;

namespace DevStore.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponseHasErrors(ResponseResult response)
        {
            if (response != null && response.Errors.Messages.Count != 0)
            {
                foreach (var message in response.Errors.Messages)
                {
                    ModelState.AddModelError(string.Empty, message);
                }

                return true;
            }

            return false;
        }

        protected void AddValidationError(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }

        protected bool IsValid()
        {
            return ModelState.ErrorCount == 0;
        }
    }
}