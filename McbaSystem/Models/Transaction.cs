using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaSystem.Models;

public enum TransactionType
{
    Deposit = 'D',
    Withdraw = 'W',
    Transfer = 'T',
    ServiceCharge = 'S'
}

public class Transaction
{
    public int TransactionID { get; set; }

    public TransactionType TransactionType { get; set; }

    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }
    
    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot exceed 30 characters")]
    public string Comment { get; set; }

    public DateTime TransactionTimeUtc { get; set; }
}
