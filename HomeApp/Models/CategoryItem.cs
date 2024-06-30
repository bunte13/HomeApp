namespace HomeApp.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int CategoryId { get; set; }
        public Item? Item { get; set; }
        public Category? Category { get; set; }
    }
}
