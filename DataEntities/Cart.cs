namespace DataEntities;

[Serializable]
public class Cart
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public IAsyncEnumerable<CartItem> _items { get; set; }
    public int TotalItems => _items.SumAsync(i => i.Quantity).Result;

    public Cart(string buyerId)
    {
        BuyerId = buyerId;
    }

    public async Task AddItemAsync(int itemId, decimal unitPrice, int quantity = 1)
    {
        var existingItem = await _items.FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (existingItem == null)
        {
            var newItem = new CartItem(itemId, quantity, unitPrice);
            _items = _items.Append(newItem);
        }
        else
        {
            existingItem.AddQuantity(quantity);
        }
    }

    public async Task CopyItemAsync(CartItem item)
    {
        _items = _items.Append(item);
    }

    public async Task RemoveEmptyItemsAsync()
    {
        var nonEmptyItems = await _items.Where(i => i.Quantity > 0).ToListAsync();
        _items = nonEmptyItems.ToAsyncEnumerable();
    }

}
