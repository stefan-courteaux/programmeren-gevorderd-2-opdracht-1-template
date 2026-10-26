namespace WebShoppie.Storage.DataModel;

public class OrderDataModel
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalPrice { get; set; }
}