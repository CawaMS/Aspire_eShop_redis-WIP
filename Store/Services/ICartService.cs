using DataEntities;

namespace Store.Services;

public interface ICartService
{
    Task TransferCart(string anonymousId, string userName);
    Task AddItem(string userName, int productId, decimal price, int quantity = 1);
    Task SetQuantities(string userName, int productId, int quantity);
    Task DeleteCart(string cartId);
    Task<Cart?> GetCart(string cartId);
    IAsyncEnumerable<CartItem> GetCartItems(string username);

}
