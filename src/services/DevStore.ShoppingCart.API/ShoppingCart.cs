using DevStore.ShoppingCart.API.Data;
using DevStore.ShoppingCart.API.Model;
using DevStore.WebAPI.Core.User;
using Microsoft.EntityFrameworkCore;

namespace DevStore.ShoppingCart.API
{
    public class ShoppingCart(ShoppingCartContext context, IAspNetUser user)
    {
        private readonly ICollection<string> _errors = [];

        public async Task<CustomerShoppingCart> GetShoppingCart()
        {
            return await GetShoppingCartClient() ?? new CustomerShoppingCart();
        }

        public async Task<IResult> AddItem(CartItem item)
        {
            var shoppingCart = await GetShoppingCartClient();

            if (shoppingCart == null)
                ManageNewCart(item);
            else
                ManageCart(shoppingCart, item);

            if (_errors.Count != 0) return CustomResponse();

            await Persist();
            return CustomResponse();
        }

        public async Task<IResult> UpdateItem(Guid productId, CartItem item)
        {
            var shoppingCart = await GetShoppingCartClient();
            var shoppingCartItem = await GetValidItem(productId, shoppingCart, item);
            if (shoppingCartItem == null) return CustomResponse();

            shoppingCart.UpdateUnit(shoppingCartItem, item.Quantity);

            ValidateShoppingCart(shoppingCart);
            if (_errors.Count != 0) return CustomResponse();

            context.CartItems.Update(shoppingCartItem);
            context.CustomerShoppingCart.Update(shoppingCart);

            await Persist();
            return CustomResponse();
        }

        public async Task<IResult> RemoveItem(Guid productId)
        {
            var cart = await GetShoppingCartClient();

            var item = await GetValidItem(productId, cart);
            if (item == null) return CustomResponse();

            ValidateShoppingCart(cart);
            if (_errors.Count != 0) return CustomResponse();

            cart.RemoveItem(item);

            context.CartItems.Remove(item);
            context.CustomerShoppingCart.Update(cart);

            await Persist();
            return CustomResponse();
        }

        public async Task<IResult> ApplyVoucher(Voucher voucher)
        {
            var cart = await GetShoppingCartClient();

            cart.ApplyVoucher(voucher);

            context.CustomerShoppingCart.Update(cart);

            await Persist();
            return CustomResponse();
        }


        async Task<CustomerShoppingCart> GetShoppingCartClient()
        {
            return await context.CustomerShoppingCart
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == user.GetUserId());
        }

        void ManageNewCart(CartItem item)
        {
            var cart = new CustomerShoppingCart(user.GetUserId());
            cart.AddItem(item);

            ValidateShoppingCart(cart);
            context.CustomerShoppingCart.Add(cart);
        }

        void ManageCart(CustomerShoppingCart cart, CartItem item)
        {
            var savedItem = cart.HasItem(item);

            cart.AddItem(item);
            ValidateShoppingCart(cart);

            if (savedItem)
            {
                context.CartItems.Update(cart.GetProductById(item.ProductId));
            }
            else
            {
                context.CartItems.Add(item);
            }

            context.CustomerShoppingCart.Update(cart);
        }

        async Task<CartItem> GetValidItem(Guid productId, CustomerShoppingCart cart, CartItem item = null)
        {
            if (item != null && productId != item.ProductId)
            {
                AddErrorToStack("Current item is not the same sent item");
                return null;
            }

            if (cart == null)
            {
                AddErrorToStack("Shopping cart not found");
                return null;
            }

            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(i => i.ShoppingCartId == cart.Id && i.ProductId == productId);

            if (cartItem == null || !cart.HasItem(cartItem))
            {
                AddErrorToStack("The item is not in cart");
                return null;
            }

            return cartItem;
        }

        async Task Persist()
        {
            var result = await context.SaveChangesAsync();
            if (result <= 0) AddErrorToStack("Error saving data");
        }

        bool ValidateShoppingCart(CustomerShoppingCart shoppingCart)
        {
            if (shoppingCart.IsValid()) return true;

            shoppingCart.ValidationResult.Errors.ToList().ForEach(e => AddErrorToStack(e.ErrorMessage));
            return false;
        }

        void AddErrorToStack(string error)
        {
            _errors.Add(error);
        }

        IResult CustomResponse(object result = null)
        {
            if (_errors.Count == 0)
            {
                return Results.Ok(result);
            }

            return Results.BadRequest(Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    { "Messages", _errors.ToArray() }
                }));
        }
    }
}
