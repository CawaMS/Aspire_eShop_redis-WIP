using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities;

[Serializable]
public class CartItem
{
    public int Id { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int ItemId { get; set; }
    public string? CartId { get; set; }

    public CartItem(int itemId, int quantity, decimal unitPrice)
    {
        ItemId = itemId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}
