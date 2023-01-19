using McbaSystem.Models;
using Newtonsoft.Json;

namespace McbaSystem.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();
        
         if (context.Customers.Any())
         {
             return;
         }
         
        List<DTOs.CustomerDTO> customers = LoadDataFromServer();


        //nested for loop to convert DTO to Business Obj and load them to database.
        foreach (var customer in customers)
        {
            context.Add(CustomerConvert(customer));
            context.Add(LoginConvert(customer));
            foreach (var account in customer.Accounts)
            {
                context.Add(AccountConvert(account, AccountTypeSwitch(account.AccountType)));
                foreach (var transaction in account.Transactions)
                {
                    context.Add(TransactionConvert(account, transaction));
                }
            }
        }

        context.SaveChanges();
    }
    
    

    private static List<DTOs.CustomerDTO> LoadDataFromServer()
    {
        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";
        using var client = new HttpClient();
        var json = client.GetStringAsync(Url).Result;

        return JsonConvert.DeserializeObject<List<DTOs.CustomerDTO>>(json, new JsonSerializerSettings
        {
            DateFormatString = "DD/MM/YYYY hh:mm:ss tt"
        });
    }

    private static Customer CustomerConvert(DTOs.CustomerDTO customer)
    {
        return new Customer()
        {
            CustomerID = customer.CustomerID,
            Name = customer.Name,
            Address = customer.Address,
            City = customer.City,
            PostCode = customer.PostCode
        };
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

    private static AccountType AccountTypeSwitch(string accountType)
    {
        switch (accountType)
        {
            case "C":
                return AccountType.Checking;
            case "S":
                return AccountType.Saving;
            default:
                throw new InvalidOperationException($"Invalid AccountType: {accountType}");
        }
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
