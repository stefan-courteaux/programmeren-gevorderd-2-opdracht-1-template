using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using WebShoppie.Storage.DataModel;

namespace WebShoppie.Storage.Customers;

public class DapperCustomerRepository(IOptions<DapperRepositoryOptions> options) : ICustomerRepository
{
    private readonly string _connectionstring = options.Value.ConnectionString;
    
    public CustomerDataModel Create(CustomerDataModel customer)
    {
        const string sql =
            """
            INSERT INTO storefront.customers(
                        firstname, lastname, dateofbirth, email,
                        addressline1, addressline2, addressline3, country)
            VALUES (@firstname, @lastname, @dateofbirth, @email,
                    @addressline1, @addressline2, @addressline3, @country)
            RETURNING CustomerId
            """;

        using var connection = new NpgsqlConnection(_connectionstring);

        var id = connection.ExecuteScalar<int>(sql, customer);

        return GetById(id) ?? throw new Exception( 
            $"Cust {customer.FirstName} could not be created"
        );    }

    public CustomerDataModel? GetById(int id)
    {
        const string sql = "SELECT * FROM storefront.customers WHERE CustomerId = @id";
        
        using var connection = new NpgsqlConnection(_connectionstring);
        
        var dbCustomer =  connection.QuerySingleOrDefault<CustomerDataModel>(
            sql, new { id });
        
        return dbCustomer;
    }

    public IEnumerable<CustomerDataModel> GetAll()
    {
        const string sql = "SELECT * FROM storefront.customers";
        
        using var connection = new NpgsqlConnection(_connectionstring);
        
        var dbCustomers =  connection.Query<CustomerDataModel>( sql);
        
        return dbCustomers;    
    }

    public bool Delete(int customerId)
    {
        const string sql = "DELETE FROM storefront.customers WHERE CustomerId = @id";
        
        using var connection = new NpgsqlConnection(_connectionstring);
        
        var dbResult =  connection.Execute( sql, new { id = customerId });
        
        return dbResult == 1; 
    }

    public CustomerDataModel Update(CustomerDataModel customer)
    {
        const string sql = 
            """
            UPDATE storefront.customers
            SET  firstname = @firstname,
                 lastname = @lastname,
                 dateofbirth = @dateofbirth,
                 email = @email,
                 addressline1 = @addressline1,
                 addressline2 = @addressline2,
                 addressline3 = @addressline3,
                 country = @country
            WHERE customerId = @customerId
            """;            
        
        using var connection = new NpgsqlConnection(_connectionstring);
        
        // Dit was een mooie bug. In een happy path executie zouden we altijd klant 1 terug gekregen hebben.
        // (Gespot door Kurt Van Hul)
        /*
        var dbCustomer =  connection.Execute(sql, customer);        
        return GetById(dbCustomer) ?? throw new Exception();
        */
        
        var rowsAffected =  connection.Execute(sql, customer);

        return rowsAffected == 1
            ? GetById(customer.CustomerId)!
            : throw new InvalidOperationException($"Customer {customer.CustomerId} could not be updated");
    }
}
