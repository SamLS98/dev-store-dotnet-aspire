using DevStore.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DevStore.WebApp.MVC.Controllers
{
    public class CatalogController(ICatalogService catalogService) : MainController
    {
        [HttpGet]
        [Route("")]
        [Route("showcase")]
        public async Task<IActionResult> Index([FromQuery] int ps = 8, [FromQuery] int page = 1, [FromQuery] string q = null)
        {
            var products = await catalogService.ListAll(ps, page, q);
            ViewBag.Search = q;
            products.ReferenceAction = "Index";

            return View(products);
        }

        [HttpGet]
        [Route("products/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            var product = await catalogService.GetById(id);

            return View(product);
        }
    }
}