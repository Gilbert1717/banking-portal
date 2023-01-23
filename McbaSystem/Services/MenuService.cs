using McbaSystem.Data;
using McbaSystem.Models;

// namespace s3665887_a1.Services;
//
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
    
     
   
    
    
    
    

    // public decimal? WithdrawAmountValidation(string amount, Account account)
    // {
    //     if (decimal.TryParse(amount, out decimal validAmount)
    //         && decimal.Round(validAmount, 2) == validAmount
    //         && validAmount > 0)
    //     {
    //         if ((account.AccountType == AccountType.Saving && account.Balance - validAmount < 0) ||
    //             (account.AccountType == AccountType.Checking && account.Balance - validAmount < 300))
    //         {
    //             PrintWarning("\nInsufficient balance\nDirect to main menu\n");
    //             return 0;
    //         }
    //
    //         return validAmount;
    //     }
    //
    //     Console.WriteLine("Invalid Input");
    //     return null;
    // }
    //
    // public void WithdrawMoney(string comment, decimal amount, Account account)
    // {
    //     HandleTransaction(CreateTransaction(TransactionType.W, comment, amount * -1, account), account);
    //     if (TransactionFeeValidation(_transactionRepository.GetById(account.AccountNumber)))
    //     {
    //         HandleTransaction(
    //             CreateTransaction(TransactionType.S, "Atm Withdraw charge", atmWithdraw, account),
    //             account
    //         );
    //     }
    //
    //     Console.WriteLine($"Successfully withdraw {amount:C} from your account");
    //     Console.WriteLine(new string('-', 80));
    //     Console.WriteLine();
    // }


    


    // public void TransferMoney(string comment, decimal amount, Account account, Account destinationAccount)
    // {
    //     HandleTransaction(
    //         CreateTransaction(TransactionType.T, comment, amount * -1, account, destinationAccount),
    //         account
    //     );
    //     if (TransactionFeeValidation(_transactionRepository.GetById(account.AccountNumber)))
    //     {
    //         HandleTransaction(
    //             CreateTransaction(TransactionType.S, "Transfer charge", accountTransfer, account),
    //             account
    //         );
    //     }
    //
    //     HandleTransaction(
    //         CreateTransaction(TransactionType.T, comment, amount, destinationAccount),
    //         destinationAccount
    //     );
    //     Console.WriteLine($"Successfully transfer {amount:C} to account {destinationAccount.AccountNumber}");
    //     Console.WriteLine(new string('-', 80));
    //     Console.WriteLine();
    // }
    //
    // private void SaveTransaction(Transaction transaction)
    // {
    //     _transactionRepository.Save(transaction);
    // }
    //
    //
    // private void UpdateAccount(Account account)
    // {
    //     _accountRepository.Update(account);
    // }

    // private Transaction CreateTransaction(TransactionType transactionType, string comment, decimal amount,
    //     Account account, Account destinationAccount = null)
    // {
    //     return new Transaction
    //     {
    //         TransactionType = transactionType,
    //         AccountNumber = account.AccountNumber,
    //         Comment = comment,
    //         Amount = amount,
    //         DestinationAccountNumber = destinationAccount?.AccountNumber,
    //         TransactionTimeUtc = DateTime.Now.ToUniversalTime()
    //     };
    // }
    //
    // private bool TransactionFeeValidation(List<Transaction> transactions)
    // {
    //     int count = 0;
    //     foreach (var transaction in transactions)
    //     {
    //         if (transaction.TransactionType == TransactionType.W
    //             || (transaction.TransactionType == TransactionType.T
    //                 && transaction.DestinationAccountNumber != null))
    //         {
    //             count++;
    //             if (count > 2)
    //                 return true;
    //         }
    //     }
    //
    //     return false;
    // }
    
     
     
//
//     public void PrintWarning(string message)
//     {
//         Console.Clear();
//         Console.ForegroundColor = ConsoleColor.Red;
//         Console.WriteLine($"\n{message}\n");
//         Console.ResetColor();
//     }
}