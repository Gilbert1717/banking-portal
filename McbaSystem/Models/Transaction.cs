using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaSystem.Models;

public enum TransactionType
{
    Deposit = 'D',
    Withdraw = 'W',
    Transfer = 'T',
    ServiceCharge = 'S',
    BillPay = 'B'
}

public class Transaction
{
    public int TransactionID { get; set; }

    [Required, Column(TypeName = "char")]
    [Display(Name = "Type")]
    public TransactionType TransactionType { get; set; }

    [ForeignKey("Account")]
    [Display(Name = "Source Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    [Display(Name = "Dest Account")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot exceed 30 characters")]
    public string Comment { get; set; }

    [Display(Name = "Time")]
    public DateTime TransactionTimeUtc { get; set; }
}