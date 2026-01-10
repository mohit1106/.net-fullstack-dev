namespace DigitalWallet.Core
{
    public class WalletData
    {
        // Value Types
        public int UserId;
        public decimal Balance;
        public bool IsActive;

        // Reference Type
        public string UserName;

        // Array to store last transactions
        public decimal[] RecentTransactions;
    }
}
