using WebShoppie.Api.Contracts.Customers;
using WebShoppie.Api.Contracts.Orders;
using WebShoppie.Api.Contracts.Products;
using WebShoppie.Storage.DataModel;

namespace WebShoppie.Domain.Services.Orders;

public static class MappingExtensions
{
    public static OrderResponseContract AsContract(
        this OrderDataModel order,
        IEnumerable<OrderProductResponseContract> orderProducts,
        CustomerResponseContract customer)
    {
        return new OrderResponseContract
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            TotalPrice = order.TotalPrice,
            CustomerEmail = customer.Email,
            OrderProducts = orderProducts
        };
    }

    public static OrderProductResponseContract AsContract(
        this OrderProductDataModel orderProduct,
        string productName)
    {
        return new OrderProductResponseContract
        {
            OrderProductId = orderProduct.OrderProductId,
            OrderId = orderProduct.OrderId,
            ProductId = orderProduct.ProductId,
            ProductName = productName,
            Quantity = orderProduct.Quantity,
            Price = orderProduct.Price
        };
    }
}