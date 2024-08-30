using DevStore.ShoppingCart.API.Model;
using DevStore.WebAPI.Core.User;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DevStore.ShoppingCart.API.Services.gRPC
{
    [Authorize]
    public class ShoppingCartGrpcService(
        ILogger<ShoppingCartGrpcService> logger,
        IAspNetUser user,
        Data.ShoppingCartContext context) : ShoppingCartOrders.ShoppingCartOrdersBase
    {
        public override async Task<CustomerShoppingCartClientResponse> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
        {
            logger.LogInformation("Call GetCart");

            var shoppingCart = await GetShoppingCartClient() ?? new CustomerShoppingCart();

            return MapShoppingCartClientToProtoResponse(shoppingCart);
        }

        private async Task<CustomerShoppingCart> GetShoppingCartClient()
        {
            return await context.CustomerShoppingCart
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == user.GetUserId());
        }

        private static CustomerShoppingCartClientResponse MapShoppingCartClientToProtoResponse(CustomerShoppingCart shoppingCart)
        {
            var shoppingCartResponse = new CustomerShoppingCartClientResponse
            {
                Id = shoppingCart.Id.ToString(),
                Customerid = shoppingCart.CustomerId.ToString(),
                Total = (double)shoppingCart.Total,
                Discount = (double)shoppingCart.Discount,
                Hasvoucher = shoppingCart.HasVoucher,
            };

            if (shoppingCart.Voucher != null)
            {
                shoppingCartResponse.Voucher = new VoucherResponse
                {
                    Code = shoppingCart.Voucher.Code,
                    Percentage = (double?)shoppingCart.Voucher.Percentage ?? 0,
                    Discount = (double?)shoppingCart.Voucher.Discount ?? 0,
                    Discounttype = (int)shoppingCart.Voucher.DiscountType
                };
            }

            foreach (var item in shoppingCart.Items)
            {
                shoppingCartResponse.Items.Add(new ShoppingCartItemResponse
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Image = item.Image,
                    Productid = item.ProductId.ToString(),
                    Quantity = item.Quantity,
                    Price = (double)item.Price
                });
            }

            return shoppingCartResponse;
        }
    }
}