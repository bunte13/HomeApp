namespace HomeApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string? AppUser { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public Item? item { get; set; }
    }
}
