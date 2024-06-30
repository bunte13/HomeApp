namespace HomeApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public int SupplierId { get; set; }
        public string ? PictureJPG { get; set; }
        public string? Description { get; set; }
        public string? Dimensions { get; set; }
        public Supplier? Supplier { get; set; }
        public int Price { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<CategoryItem>? CategoryItems { get; set; }
        public ICollection<UserItem>? userItems { get; set; }
    }
}
