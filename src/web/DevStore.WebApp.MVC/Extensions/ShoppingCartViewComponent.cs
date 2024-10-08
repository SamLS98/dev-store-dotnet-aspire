using DevStore.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevStore.WebApp.MVC.Extensions
{
    public class ShoppingCartViewComponent(ICheckoutBffService checkoutBffService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await checkoutBffService.GetShoppingCartItemsQuantity());
        }
    }
}