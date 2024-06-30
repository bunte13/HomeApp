namespace HomeApp.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public int Established { get; set; }
        public string? LogoUrl { get; set; }  // Add this property
        public ICollection<Item>? items { get; set; }

    }
}
