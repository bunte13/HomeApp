using HomeApp.Models;

namespace HomeApp.ViewModel
{
    public class ItemReview
    {
        public string name { get; set; }
        public int Id { get; set; }
        public Review? Review { get; set; } = new Review();
        
    }
}
