using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using WebShoppie.Storage.DataModel;

namespace WebShoppie.Storage.Products;

public class DapperProductRepository(IOptions<DapperRepositoryOptions> options) : IProductRepository
{
    private readonly string _connectionString = options.Value.ConnectionString;

    public ProductDataModel Create(ProductDataModel product)
    {
        const string sql =
            """
            INSERT INTO storefront.products(title, description, price, stockcount)
            VALUES (@title, @description, @price, @stockcount)
            RETURNING ProductId
            """;

        using var connection = new NpgsqlConnection(_connectionString);
        var id = connection.ExecuteScalar<int>(sql, product);

        return GetById(id) ?? throw new InvalidOperationException(
            $"Product {product.Title} could not be created");
    }

    public ProductDataModel? GetById(int id)
    {
        const string sql = "SELECT * FROM storefront.products WHERE ProductId = @id";

        using var connection = new NpgsqlConnection(_connectionString);
        return connection.QuerySingleOrDefault<ProductDataModel>(sql, new { id });
    }

    public IEnumerable<ProductDataModel> GetAll()
    {
        const string sql = "SELECT * FROM storefront.products";

        using var connection = new NpgsqlConnection(_connectionString);
        return connection.Query<ProductDataModel>(sql);
    }

    public bool Delete(int productId)
    {
        const string sql = "DELETE FROM storefront.products WHERE ProductId = @id";

        using var connection = new NpgsqlConnection(_connectionString);
        return connection.Execute(sql, new { id = productId }) == 1;
    }

    public ProductDataModel Update(ProductDataModel product)
    {
        const string sql =
            """
            UPDATE storefront.products
            SET title = @title,
                description = @description,
                price = @price,
                stockcount = @stockcount
            WHERE ProductId = @productId
            """;

        using var connection = new NpgsqlConnection(_connectionString);
        var affectedRows = connection.Execute(sql, product);

        return affectedRows == 1 
            ? GetById(product.ProductId)! 
            : throw new InvalidOperationException($"Product {product.ProductId} could not be updated");
    }
}