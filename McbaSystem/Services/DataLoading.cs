using McbaSystem.Data;
using McbaSystem.Models;
using Newtonsoft.Json;

namespace McbaSystem.Services;

public class DataLoading
{
    private readonly McbaContext _context; 
   

    public async Task Preloading()
    {
        if (_context.Customers.Any())
        {
            return;
        }

        //Create instances which will be used for loading data
        List<DTOs.CustomerDTO> customers = LoadDataFromServer();


        //nested for loop to convert DTO to Business Obj and load them to database.
        foreach (var customer in customers)
        {
            _context.Add(customer);
            Login login = LoginConvert(customer);
            _context.Add(login);
            foreach (var account in customer.Accounts)
            {
                if (Enum.TryParse(account.AccountType, out AccountType result))
                {
                    AccountConvert(account, result);
                    _context.Add(account);
                }
                foreach (var transaction in account.Transactions)
                {
                    Transaction transactionB = TransactionConvert(account, transaction);
                    _context.Add(transactionB);
                }
            }
        }

        _context.SaveChanges();
    }

    private List<DTOs.CustomerDTO> LoadDataFromServer()
    {
        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";
        using var client = new HttpClient();
        var json = client.GetStringAsync(Url).Result;

        return JsonConvert.DeserializeObject<List<DTOs.CustomerDTO>>(json, new JsonSerializerSettings
        {
            DateFormatString = "DD/MM/YYYY hh:mm:ss tt"
        });
    }

    private static Account AccountConvert(DTOs.AccountDTO account, AccountType accountType)
    {
        return new Account
        {
            CustomerID = account.CustomerID,
            AccountNumber = account.AccountNumber,
            AccountType = accountType,
            Balance = CalculateBalance(account.Transactions)
        };
    }

    private static Login LoginConvert(DTOs.CustomerDTO customer)
    {
        return new Login
        {
            LoginID = customer.Login.LoginID,
            PasswordHash = customer.Login.PasswordHash,
            CustomerID = customer.CustomerID
        };
    }

    private static Transaction TransactionConvert(DTOs.AccountDTO account,
        DTOs.TransactionDTO transaction)
    {
        return new Transaction()
        {
            TransactionType = TransactionType.Deposit,
            AccountNumber = account.AccountNumber, 
            Amount = transaction.Amount,
            Comment = transaction.Comment, 
            TransactionTimeUtc = transaction.TransactionTimeUtc
        };
    }
    
    private static decimal CalculateBalance(DTOs.TransactionDTO[] transactions)
    {
        return transactions.Sum(transaction => transaction.Amount);
    }
    
    
}