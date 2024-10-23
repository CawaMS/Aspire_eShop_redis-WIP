using DataEntities;
using Store.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace Store.Services
{
    public class CartServiceCache : ICartService
    {
        //private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redisConnectionMultiplexer;
        private readonly IDatabase _database;

        private const string CartKeyPrefix = "Cart_&_";
        private const string CartItemListFieldKey = "CartItemsList";
        private const string CartIdFieldKey = "CartId";
        private const string UserIdFieldKey = "UserId";


        public CartServiceCache(IConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisConnectionMultiplexer = redisConnectionMultiplexer;
            _database = _redisConnectionMultiplexer.GetDatabase();
        }

        public async Task AddItem(string userName, int itemId, decimal price, int quantity = 1)
        {            
            var CartKey = GetCartKey(userName);
            var CartHash = await _database.HashGetAllAsync(CartKey);

            if (!(CartHash.Count() > 0))
            {
                List<CartItem> cartItemList = new List<CartItem>();
                CartItem cartItemToAdd = new CartItem(itemId, quantity, price);
                cartItemList.Add(cartItemToAdd);
                await _database.HashSetAsync(CartKey, 
                    new HashEntry[] 
                    { new HashEntry(CartItemListFieldKey, ConvertData<CartItem>.ObjectListToByteArray(cartItemList).Result),
                      new HashEntry(CartIdFieldKey, CartKey),
                      new HashEntry(UserIdFieldKey, userName)
                    });
            }
            else
            {
                byte[]? CartItemListByteArray = CartHash.Where(hashEntry => hashEntry.Name == CartItemListFieldKey).FirstOrDefault().Value;
                List<CartItem> CartItemList = await ConvertData<CartItem>.ByteArrayToObjectList(CartItemListByteArray).ToListAsync();
                CartItem? cartItem = CartItemList.Where(item => item.ItemId == itemId).FirstOrDefault();
                if (cartItem != null)
                {
                    CartItem newCartItem = new CartItem(itemId, cartItem.Quantity+1, price);
                    CartItemList.Remove(cartItem);
                    CartItemList.Add(newCartItem);
                }
                else
                {
                    CartItem newCartItem = new CartItem(itemId, 1, price);

                    CartItemList.Add(newCartItem);
                }

                byte[] CartItemListToUpdateByteArray = await ConvertData<CartItem>.ObjectListToByteArray(CartItemList);
                await _database.HashSetAsync(CartKey, CartItemListFieldKey, CartItemListToUpdateByteArray);
            }
        }

        public async Task DeleteCart(string username)
        {

            if (username == null)
            {
                return;
            }

            await _database.KeyDeleteAsync(GetCartKey(username));
        }

        public async Task<Cart?> GetCart(string username)
        {

            throw new NotImplementedException();


        }

        public async Task SetQuantities(string userName, int productId, int quantity)
        {
            var CartKey = GetCartKey(userName);
            var CartHash = await _database.HashGetAllAsync(CartKey);
            List<CartItem> CartItemList = await ConvertData<CartItem>.ByteArrayToObjectList(CartHash.Where(hashEntry => hashEntry.Name == CartItemListFieldKey).FirstOrDefault().Value).ToListAsync();
            CartItem? cartItem = CartItemList.Where(item => item.ItemId == productId).FirstOrDefault();
            CartItem newCartItem = new CartItem(productId, quantity, cartItem.UnitPrice);
            CartItemList.Remove(cartItem);
            CartItemList.Add(newCartItem);
            byte[] CartItemListToUpdateByteArray = await ConvertData<CartItem>.ObjectListToByteArray(CartItemList);
            await _database.HashSetAsync(CartKey, CartItemListFieldKey, CartItemListToUpdateByteArray);
        }

        public async Task TransferCart(string anonymousName, string userName)
        {
            var oldCartKey = GetCartKey(anonymousName);
            var newCartKey = GetCartKey(userName);

            var CartHash = await _database.HashGetAllAsync(oldCartKey);

            if (!(CartHash.Count() > 0))
            {
                return;
            }
            else
            {
                byte[]? CartItemListByteArray = CartHash.Where(hashEntry => hashEntry.Name == CartItemListFieldKey).FirstOrDefault().Value;
                await _database.HashSetAsync(newCartKey, new HashEntry[] 
                        { new HashEntry(CartItemListFieldKey, CartItemListByteArray),
                          new HashEntry(CartIdFieldKey, newCartKey),
                          new HashEntry(UserIdFieldKey, userName)
                        });
            }

            await _database.KeyDeleteAsync(oldCartKey);
        }

        public async IAsyncEnumerable<CartItem>? GetCartItems(string userName)
        {

            if (userName == null)
            {
                yield break;
            }

            var CartKey = GetCartKey(userName);
            byte[]? cartItemslist = await _database.HashGetAsync(CartKey, CartItemListFieldKey);

            if (cartItemslist.IsNullOrEmpty())
            {
                yield break;
            }
            else
            {
                await foreach (CartItem _cartItem in ConvertData<CartItem>.ByteArrayToObjectList(cartItemslist))
                {
                    yield return _cartItem;
                }
            }


        }

        private int generateCartId()
        {
            var rand = new Random();
            int cartId = rand.Next();
            return cartId;
        }

        private string GetCartKey(string userName)
        {
            return CartKeyPrefix + userName;
        }

    }
}
