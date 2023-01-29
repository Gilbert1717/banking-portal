using AdminPortal.Models;

namespace AdminPortal.ViewModels.Home;

public class IndexViewModel
{
    public List<Customer> Customers { get; set; }

    public List<Login> Logins { get; set; }
}