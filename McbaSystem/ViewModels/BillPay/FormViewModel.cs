using McbaSystem.Models;

namespace McbaSystem.ViewModels.BillPay;

public class FormViewModel
{
    public Models.BillPay BillPay { get; set; }
    public List<Payee> Payees { get; set; }
    public string Action { get; set; }
}