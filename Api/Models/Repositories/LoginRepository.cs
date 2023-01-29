using Api.Data;

namespace Api.Models.Repositories;

public class LoginRepository : IDataRepository<Login, int>
{
    private readonly ApiContext _context;

    public LoginRepository(ApiContext context) => _context = context;

    public IEnumerable<Login> GetAll()
    {
        return _context.Logins.ToList();
    }

    public Login Get(int id)
    {
        return _context.Logins.Find(id);
    }

    public void Update(Login login)
    {
        _context.Update(login);
        _context.SaveChanges();
    }
}