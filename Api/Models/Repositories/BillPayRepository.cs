using Api.Data;

namespace Api.Models.Repositories;

public class BillPayRepository : IDataRepository<BillPay, int>
{
    private readonly ApiContext _context;

    public BillPayRepository(ApiContext context) => _context = context;

    public IEnumerable<BillPay> GetAll()
    {
        return _context.BillPays.ToList();
    }

    public BillPay Get(int id)
    {
        return _context.BillPays.Find(id);
    }

    public void Update(BillPay billPay)
    {
        _context.Update(billPay);
        _context.SaveChanges();
    }
}