namespace WebShoppie.Storage.DataModel;

public class OrderProductDataModel
{
    public int OrderProductId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}