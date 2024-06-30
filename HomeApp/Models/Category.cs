namespace HomeApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? CatgoryName { get; set; }
        public ICollection<CategoryItem>? CategoryItems { get; set;}
    }
}
