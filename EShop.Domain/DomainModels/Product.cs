using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.DomainModels
{
    public class Product : BaseEntity
    {
      
        [Required]
        public String ProductName { get; set; }
        [Required]
        public String ProductDescription { get; set; }
        [Required]
        public String ProductImage { get; set; }
        [Required]
        public int ProductPrice { get; set; }
        public int Rating { get; set; }
        public virtual ICollection<ProductInShoppingCart> ProductinShoppingCart { get; set; }
        public virtual ICollection<ProductInOrder> Orders { get; set; } = new List<ProductInOrder>(); // Initialize as empty list



        public Product()
        {
            ProductinShoppingCart = new List<ProductInShoppingCart>();
        }
    }
}
