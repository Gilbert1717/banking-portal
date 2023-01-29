using Api.Data;

namespace Api.Models.Repositories;

public class CustomerRepository : IDataRepository<Customer, int>
{
    private readonly ApiContext _context;

    public CustomerRepository(ApiContext context) => _context = context;

    public IEnumerable<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }

    public Customer Get(int id)
    {
        return _context.Customers.Find(id);
    }

    public void Update(Customer customer)
    {
        _context.Update(customer);
        _context.SaveChanges();
    }
}