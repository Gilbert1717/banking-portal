using McbaSystem.Models;

namespace McbaSystem.Utilities;

public static class MiscellaneousExtensionUtilities
{
    public static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
    public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);
    public static bool InsufficientAmount(this Account account , decimal value)
    {
        if (account.AccountType == AccountType.Saving)
            return account.Balance - value <= 0;
        if (account.AccountType == AccountType.Checking)
            return account.Balance - value <= 300;
        throw new InvalidDataException("Invalid account");
    }
}
