using McbaSystem.Models;
using Newtonsoft.Json;

namespace McbaSystem.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();

        if (!context.Customers.Any())
        {
            List<DTOs.CustomerDTO> customers = LoadDataFromServer();
            SaveServerDataToDb(customers, context);
        }

        if (!context.Payees.Any())
        {
            SeedPayees(context);
        }
    }

    private static void SeedPayees(McbaContext context)
    {
        context.Payees.AddRange(
            new Payee
            {
                Name = "ATO",
                Address = "1 Gov Rd",
                City = "Canberra",
                State = "ACT",
                PostCode = "2000",
                Phone = "(02) 2222 3333"
            },
            new Payee
            {
                Name = "Origin Energy",
                Address = "55 Water Pl",
                City = "Cairns",
                State = "QLD",
                PostCode = "4870",
                Phone = "(07) 9876 5432"
            }
        );

        context.SaveChanges();
    }

    private static void SaveServerDataToDb(List<DTOs.CustomerDTO> customers, McbaContext context)
    {
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
        const string url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";
        using var client = new HttpClient();
        var json = client.GetStringAsync(url).Result;

        return JsonConvert.DeserializeObject<List<DTOs.CustomerDTO>>(json, new JsonSerializerSettings
        {
            DateFormatString = "DD/MM/YYYY hh:mm:ss tt"
        });
    }

    private static Customer CustomerConvert(DTOs.CustomerDTO customer)
    {
        return new Customer
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
        return new Transaction
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