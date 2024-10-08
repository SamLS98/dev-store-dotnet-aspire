using DevStore.WebApp.MVC.Models;
using DevStore.WebApp.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.WebApp.MVC.Controllers
{
    [Authorize]
    public class CustomerController(ICustomerService customerService) : MainController
    {
        [HttpPost]
        public async Task<IActionResult> NewAddress(AddressViewModel address)
        {
            var response = await customerService.AddAddress(address);

            if (ResponseHasErrors(response)) TempData["Errors"] =
                ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

            return RedirectToAction("DeliveryAddress", "Order");
        }
    }
}