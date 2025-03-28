namespace EShop.Domain.DomainModels
{
    public class ProductInOrder : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product SelectedProduct { get; set; }
        public Guid OrderId { get; set; }
        public Order UserOrder { get; set; }
        public int Quantity { get; set; }
        //public virtual ICollection<ProductInOrder> Products { get;set; }

    }
}
