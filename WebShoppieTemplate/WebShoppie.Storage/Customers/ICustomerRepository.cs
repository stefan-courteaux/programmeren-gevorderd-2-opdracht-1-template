using WebShoppie.Storage.DataModel;

namespace WebShoppie.Storage.Customers;

public interface ICustomerRepository
{
    CustomerDataModel Create(CustomerDataModel customer); 
    CustomerDataModel? GetById(int id);
    IEnumerable<CustomerDataModel> GetAll();
    bool Delete(int customerId);
    CustomerDataModel Update(CustomerDataModel customer);
}