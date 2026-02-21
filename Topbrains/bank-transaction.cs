using System;

public class Solution
{
    public int FinalBalance(int initialBalance, int[] transactions)
    {
        int balance = initialBalance;

        if (transactions == null)
            return balance;

        foreach (int t in transactions)
        {
            if (t >= 0)
            {
                balance += t;
            }
            else
            {
                if (balance + t >= 0)
                {
                    balance += t;
                }
            }
        }

        return balance;
    }
}
