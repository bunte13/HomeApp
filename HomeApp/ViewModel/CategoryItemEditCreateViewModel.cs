using HomeApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Reflection.Metadata.BlobBuilder;

namespace HomeApp.ViewModel
{
    public class CategoryItemEditCreateViewModel
    {
        public Item? Item { get; set; }
        public IEnumerable<int>? SelectedCategories { get; set; }
        public IEnumerable<SelectListItem>? CategoriesList { get; set; }
        public SelectList? SuppliersList { get; set; }
        public int SelectedSupplierId { get; set; }
        public IFormFile? FrontPageFile { get; set; }
        public int Quantity { get; set; }
        
    }
}
