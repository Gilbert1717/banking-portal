using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaSystem.Models;

public enum BillPayPeriod
{
    OneOff = 'O',
    Monthly = 'M'
}

public class BillPay
{
    public int BillPayID { get; set; }

    [ForeignKey("Account")] 
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("Payee")] 
    public int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Column(TypeName = "money")] 
    public decimal Amount { get; set; }

    public DateTime ScheduleTimeUtc { get; set; }
    
    [Required, Column(TypeName = "char")]
    public BillPayPeriod Period { get; set; }
}