namespace HomeApp.Models
{
    public class UserItem
    {
        public int Id { get; set; }
        public string? AppUser { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
        public int Quantity { get; set; }
    }
}
