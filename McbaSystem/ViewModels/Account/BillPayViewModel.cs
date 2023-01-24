using McbaSystem.Models;

namespace McbaSystem.ViewModels.Account;

public class BillPayViewModel
{
    public BillPay BillPay { get; set; }
    public List<Payee> Payees { get; set; }
}