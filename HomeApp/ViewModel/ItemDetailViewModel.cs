using HomeApp.Models;

namespace HomeApp.ViewModel
{
    public class ItemDetailViewModel
    {
        public Item Item { get; set; }
        public int Quantity { get; set; } // Add Quantity property
        public bool IsOwnedByCurrentUser { get; set; }
    }
}
