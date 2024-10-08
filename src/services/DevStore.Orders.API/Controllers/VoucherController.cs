using DevStore.Orders.API.Application.DTO;
using DevStore.Orders.API.Application.Queries;
using DevStore.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace DevStore.Orders.API.Controllers
{
    [Authorize]
    public class VoucherController(IVoucherQueries voucherQueries) : MainController
    {
        [HttpGet("voucher/{code}")]
        [ProducesResponseType(typeof(VoucherDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> ObterPorCodigo(string code)
        {
            if (string.IsNullOrEmpty(code)) return NotFound();

            var voucher = await voucherQueries.GetVoucher(code);

            return voucher == null ? NoContent() : CustomResponse(voucher);
        }
    }
}