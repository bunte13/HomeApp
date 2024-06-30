using HomeApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Reflection.Metadata.BlobBuilder;

namespace HomeApp.ViewModel
{
    public class ItemCategoryViewModel
    {
        public IList<Item>? items { get; set; }
        public SelectList? Categories { get; set; }
        public string? CategoryItem { get; set; } // spored koja kategorija
        public string? SearchString { get; set; } //spored imeto na produktot
        public IList<Supplier>? Suppliers { get; set; }
        public string? SupplierSearchString { get; set; } //spored snabduvach 
        public List<int>? UserItems { get; set; } // za da ne mozhe da se kupi ako vekje e kupeno
    }
}
