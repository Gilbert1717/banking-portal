using McbaSystem.Data;
using McbaSystem.Models;
using McbaSystem.Utilities;


public class MenuService
{
    private readonly decimal atmWithdraw = -0.05m;
    private readonly decimal accountTransfer = -0.1m;
    private readonly McbaContext _context;
    public MenuService(McbaContext context) => _context = context;
    
    
    
    public void HandleTransaction(Transaction transaction, Account account)
    {
        transaction.TransactionTimeUtc = DateTime.Now.ToUniversalTime();
        _context.Add(transaction);
        account.Balance += transaction.Amount;
    }

    public void WithdrawServiceFeeCharge(Account account)
    {
        if (TransactionFeeValidation(account.Transactions))
        {
            Transaction transaction = new Transaction()
            {
                TransactionType = TransactionType.ServiceCharge,
                Comment = "Withdrawal fee",
                AccountNumber = account.AccountNumber,
                Amount = atmWithdraw,
                TransactionTimeUtc = DateTime.Now.ToUniversalTime()
            };
            HandleTransaction(transaction,account);
        }
    }

    
    
    public void TransferServiceFeeCharge(Account account)
    {
        if (TransactionFeeValidation(account.Transactions))
        {
            Transaction transaction = new Transaction()
            {
                TransactionType = TransactionType.ServiceCharge,
                Comment = "Account transfer fee",
                AccountNumber = account.AccountNumber,
                Amount = accountTransfer,
                TransactionTimeUtc = DateTime.Now.ToUniversalTime()
            };
            HandleTransaction(transaction,account);
        }
    }
    
    public bool TransactionFeeValidation(List<Transaction> transactions)
    {
        int count = 0;
        foreach (var transaction in transactions)
        {
            if (transaction.TransactionType == TransactionType.Withdraw
                || (transaction.TransactionType == TransactionType.Transfer
                    && transaction.DestinationAccountNumber != null))
            {
                count++;
                if (count > 2)
                    return true;
            }
        }
    
        return false;
    }
    
    public async Task BillPayExecute(BillPay billPay)
    {

        if (billPay.Account.InsufficientAmount(billPay.Amount))
        {
            billPay.ErrorMessage = "Insufficient balance";
            _context.Update(billPay);
        }
            
        else
        {
            Transaction transaction = new Transaction()
            {
                TransactionType = TransactionType.BillPay,
                Comment = $"BillPay to {billPay.Payee.Name}",
                AccountNumber = billPay.Account.AccountNumber,
                Amount = billPay.Amount * -1,
                TransactionTimeUtc = DateTime.Now.ToUniversalTime()
            };
            HandleTransaction(transaction, billPay.Account);
            if (billPay.Period == BillPayPeriod.Monthly)
            {
                billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
                _context.Update(billPay);
            }
            else
            {
                _context.Remove(billPay);
            }
        }
    }
}