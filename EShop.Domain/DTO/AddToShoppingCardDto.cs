
using EShop.Domain.DomainModels;

namespace EShop.Domain.DTO
{
    public class AddToShoppingCardDto
    {
        public Product SelectedProduct { get; set; }
        public Guid ProductId { get; set; }
        public  int Quantity { get; set; }
    }
}
