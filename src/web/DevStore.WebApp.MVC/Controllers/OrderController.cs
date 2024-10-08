using DevStore.WebApp.MVC.Models;
using DevStore.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevStore.WebApp.MVC.Controllers
{
    public class OrderController(ICustomerService customerService,
        ICheckoutBffService checkoutBffService) : MainController
    {
        [HttpGet]
        [Route("delivery-address")]
        public async Task<IActionResult> DeliveryAddress()
        {
            var shoppingCart = await checkoutBffService.GetShoppingCart();
            if (shoppingCart.Items.Count == 0) return RedirectToAction("Index", "ShoppingCart");

            var endereco = await customerService.GetAddress();
            var order = checkoutBffService.MapToOrder(shoppingCart, endereco);

            return View(order);
        }

        [HttpGet]
        [Route("payment")]
        public async Task<IActionResult> Payment()
        {
            var shoppingCart = await checkoutBffService.GetShoppingCart();
            if (shoppingCart.Items.Count == 0) return RedirectToAction("Index", "ShoppingCart");

            var order = checkoutBffService.MapToOrder(shoppingCart, null);

            return View(order);
        }

        [HttpPost]
        [Route("finish-order")]
        public async Task<IActionResult> FinishOrder(TransactionViewModel transaction)
        {
            if (!ModelState.IsValid) return View("Payment", checkoutBffService.MapToOrder(await checkoutBffService.GetShoppingCart(), null));

            var retorno = await checkoutBffService.FinishOrder(transaction);

            if (ResponseHasErrors(retorno))
            {
                var shoppingCart = await checkoutBffService.GetShoppingCart();
                if (shoppingCart.Items.Count == 0) return RedirectToAction("Index", "ShoppingCart");

                var orderMap = checkoutBffService.MapToOrder(shoppingCart, null);
                return View("Payment", orderMap);
            }

            return RedirectToAction("OrderDone");
        }

        [HttpGet]
        [Route("order-done")]
        public async Task<IActionResult> OrderDone()
        {
            return View("OrderDone", await checkoutBffService.GetLastOrder());
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var model = await checkoutBffService.GetCustomersById();
            return View(model);
        }
    }
}