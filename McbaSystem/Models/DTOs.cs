namespace McbaSystem.Models;

public class DTOs
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public AccountDTO[] Accounts { get; set; }
        public LoginDTO Login { get; set; }
    }

    public class AccountDTO
    {
        public int AccountNumber { get; set; }
        public string AccountType { get; set; }
        public int CustomerID { get; set; }
        public TransactionDTO[] Transactions { get; set; }
    }

    public class TransactionDTO
    {
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public DateTime TransactionTimeUtc { get; set; }
    }

    public class LoginDTO
    {
        public string LoginID { get; set; }
        public string PasswordHash { get; set; }
    }
}