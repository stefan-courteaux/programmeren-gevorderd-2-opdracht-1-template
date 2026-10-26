using WebShoppie.Domain.Services.Customers;
using WebShoppie.Domain.Services.Orders;
using WebShoppie.Domain.Services.Products;
using WebShoppie.Storage;
using WebShoppie.Storage.Customers;
using WebShoppie.Storage.Orders;
using WebShoppie.Storage.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DapperRepositoryOptions>(
    builder.Configuration.GetSection(nameof(DapperRepositoryOptions)));

builder.Services.AddControllers();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerRepository, DapperCustomerRepository>();
builder.Services.AddScoped<IOrderRepository, DapperOrderRepository>();
builder.Services.AddScoped<IProductRepository, DapperProductRepository>();
// AddSingleton - 1 object instantie die hergebruikt wordt zolang
// server runt voor elke vermelding interface in constructor
// AddScoped() - 1 instantie die hergebruikt wordt voor elke vermelding
// gedurende dit http request
// AddTransient() - elke keer interface vermeld wordt in constructor
// een nieuwe instantie


var app = builder.Build();
app.MapControllers();

app.Run();

