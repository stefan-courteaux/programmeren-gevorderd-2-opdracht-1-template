using WebShoppie.Api.Contracts.Orders;
using WebShoppie.Domain.Services.Customers;
using WebShoppie.Domain.Services.Products;
using WebShoppie.Storage.DataModel;
using WebShoppie.Storage.Orders;

namespace WebShoppie.Domain.Services.Orders;

public class OrderService(IOrderRepository orderRepository, IProductService productService, ICustomerService customerService) : IOrderService
{
    public OrderResponseContract CreateOrder(OrderRequestContract orderToCreate)
    {
        var orderProducts = orderToCreate.OrderProducts.Select(orderProduct =>
        {
            var product = productService.GetById(orderProduct.ProductId)
                ?? throw new InvalidOperationException($"Product {orderProduct.ProductId} was not found");

            return new OrderProductDataModel
            {
                ProductId = orderProduct.ProductId,
                Quantity = orderProduct.Quantity,
                Price = product.Price
            };
        }).ToList();

        var order = new OrderDataModel
        {
            OrderDate = DateTime.Now,
            CustomerId = orderToCreate.CustomerId,
            TotalPrice = orderProducts.Sum(orderProduct => orderProduct.Price * orderProduct.Quantity)
        };

        // Maak order aan via repo dat bij deze service "hoort"
        var createdOrder = orderRepository.Create(order, orderProducts);

        // Return response contract
        return GetOrderResponseContract(createdOrder);
    }

    public OrderResponseContract? GetOrder(int id)
    {
        // Lees order uit repo
        var order = orderRepository.Get(id);
        
        // Return response contract
        return GetOrderResponseContract(order);
    }

    // Response contract van Order bevat ook velden die _niet_ in Order tabel/repo zitten: customer email en product title.
    // Mag een service gebruik maken van andere repo's, of hanteren we meer encapsulatie en halen we die info dus uit de relevante service?
    // Als ik kies voor verdere encapsulatie, heb ik dus de Product- en CustomerService nodig.
    // > Wat denk je zelf? Fan of niet? Voordelen & nadelen? <
    private OrderResponseContract GetOrderResponseContract(OrderDataModel order)
    {
        // Deze service calls zullen impliciet ook database calls doen.
        // Het is af te raden dat te verbergen in mapping extension methods.
        // Je wil niet dat andere devs onbedoeld veel meer db calls doen door ze te verstoppen.
        // Een dev zal niet verwachten dat een mapping extension service/db calls doet. 
        // We doen het dus zichtbaar in de service.
        var orderProductContracts = orderRepository.GetOrderProducts(order.OrderId)
            .Select(opDataModel => opDataModel.AsContract(productService.GetById(opDataModel.ProductId)!.Title));
        var orderCustomerContract = customerService.GetById(order.CustomerId)!;
        
        // Dit is dan wel zuiver object/prop mapping.
        return order.AsContract(orderProductContracts, orderCustomerContract);
        // Schrijf je dan toch liever alle mapping code hier? Da's ok.
    }
}