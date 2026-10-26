using WebShoppie.Storage.DataModel;

namespace WebShoppie.Storage.Orders;

public interface IOrderRepository
{
    OrderDataModel Create(OrderDataModel order, IEnumerable<OrderProductDataModel> orderProducts);
    OrderDataModel? Get(int id);
    IEnumerable<OrderProductDataModel> GetOrderProducts(int orderId);
    IEnumerable<OrderDataModel> GetAll();
}