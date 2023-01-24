using McbaSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace McbaSystem.Data;

public class McbaContext : DbContext
{
    public McbaContext(DbContextOptions<McbaContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Payee> Payees { get; set; }

    // Fluent-API.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Set check constraints (cannot be expressed with data annotations).
        builder.Entity<Login>().ToTable(b =>
        {
            b.HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8");
            b.HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 94");
        });

        builder.Entity<Account>().ToTable(b => b.HasCheckConstraint("CH_Account_Balance", "Balance >= 0"));

        builder.Entity<Transaction>().ToTable(b => b.HasCheckConstraint("CH_Transaction_Amount", "Amount != 0"));
        builder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.TransactionType)
                .HasConversion(type => (char)type, type => (TransactionType)type);
        });
        // Configure ambiguous Account.Transactions navigation property relationship.
        builder.Entity<Transaction>().HasOne(transaction => transaction.Account)
            .WithMany(account => account.Transactions)
            .HasForeignKey(transaction => transaction.AccountNumber);

        builder.Entity<BillPay>().ToTable(b => b.HasCheckConstraint("CH_BillPay_Amount", "Amount > 0"));
        builder.Entity<BillPay>(entity =>
        {
            entity.Property(e => e.Period)
                .HasConversion(period => (char)period, period => (BillPayPeriod)period);
            entity.Property(e => e.Status)
                .HasConversion(status => (char)status, status => (BillPayStatus)status);
        });
        // Configure ambiguous Account.BillPays navigation property relationship.
        builder.Entity<BillPay>().HasOne(billPay => billPay.Account)
            .WithMany(account => account.BillPays)
            .HasForeignKey(billPay => billPay.AccountNumber);
    }
}