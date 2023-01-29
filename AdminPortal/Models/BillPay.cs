using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortal.Models;

public enum BillPayPeriod
{
    OneOff = 'O',
    Monthly = 'M'
}

public class BillPay
{
    public int BillPayID { get; set; }

    public int AccountNumber { get; set; }

    public int PayeeID { get; set; }

    [Column(TypeName = "money")] 
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Display(Name = "When")]
    public DateTime ScheduleTimeUtc { get; set; }
    
    [Required, Column(TypeName = "char")]
    public BillPayPeriod Period { get; set; }

    [Display(Name = "Error")]
	public string ErrorMessage { get; set; }
    
    [Required] 
    public bool isBlocked { get; set; } = false;
}