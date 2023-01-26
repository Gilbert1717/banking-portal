using McbaSystem.Data;
using McbaSystem.Models;
using McbaSystem.Utilities;

namespace McbaSystem.Services;

public class AccountService
{
    private const decimal AtmWithdraw = -0.05m;
    private const decimal AccountTransfer = -0.1m;

    private readonly McbaContext _context;
    public AccountService(McbaContext context) => _context = context;

    public void HandleTransaction(Transaction transaction, Account account)
    {
        transaction.TransactionTimeUtc = DateTime.UtcNow;
        _context.Add(transaction);
        account.Balance += transaction.Amount;
    }

    public void WithdrawServiceFeeCharge(Account account)
    {
        if (TransactionFeeValidation(account.Transactions))
        {
            Transaction transaction = new Transaction
            {
                TransactionType = TransactionType.ServiceCharge,
                Comment = "Withdrawal fee",
                AccountNumber = account.AccountNumber,
                Amount = AtmWithdraw,
                TransactionTimeUtc = DateTime.UtcNow
            };
            HandleTransaction(transaction, account);
        }
    }

    public void TransferServiceFeeCharge(Account account)
    {
        if (TransactionFeeValidation(account.Transactions))
        {
            Transaction transaction = new Transaction
            {
                TransactionType = TransactionType.ServiceCharge,
                Comment = "Account transfer fee",
                AccountNumber = account.AccountNumber,
                Amount = AccountTransfer,
                TransactionTimeUtc = DateTime.UtcNow
            };
            HandleTransaction(transaction, account);
        }
    }

    private bool TransactionFeeValidation(List<Transaction> transactions)
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

    public void BillPayExecute(BillPay billPay)
    {
        if (billPay.Account.InsufficientAmount(billPay.Amount))
        {
            billPay.ErrorMessage = "Insufficient balance";
            _context.Update(billPay);
        }
        else
        {
            Transaction transaction = new Transaction
            {
                TransactionType = TransactionType.BillPay,
                Comment = $"BillPay to {billPay.Payee.Name}",
                AccountNumber = billPay.Account.AccountNumber,
                Amount = billPay.Amount * -1,
                TransactionTimeUtc = DateTime.UtcNow
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