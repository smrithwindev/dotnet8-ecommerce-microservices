namespace ProductApi.Application.DTOs
{
    public class UpdateProductDto
    {
         public string Name { get; set; }
         public Decimal Price { get; set; }
         public int Quantity { get; set; }
    }
}
