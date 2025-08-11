using System;

using System.Collections.Generic;

public record Transaction (int id, DateTime Date, decimal amount, string category);

public interface ITransactionProcessor
{
    void ProcessTransaction (Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }

    
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Sent {transaction.Amount:C} for {transaction.Category}");
    }
}


public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Transferred {transaction.Amount:C} for {transaction.Category}");
    }
}


public class Account
{
    public string AccountNumber { get; }

    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;

        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)

    {
        Balance -= transaction.Amount;

        Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
    }
}



public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {

            Console.WriteLine("Insufficient funds");

        }

        else
        {

            Balance -= transaction.Amount;

            Console.WriteLine($"Transaction applied to savings. Updated balance: {Balance:C}");
        }
    }
}



public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()

    {
        // Step i: Create savings account
        SavingsAccount savingsAccount = new("SA001", 1000m);

        // Step ii: Create transactions
        var t1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 150m, "Utilities");

        var t3 = new Transaction(3, DateTime.Now, 300m, "Entertainment");

        // Step iii: Process each transaction
        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();

        ITransactionProcessor p3 = new CryptoWalletProcessor();

        p1.Process(t1);
        savingsAccount.ApplyTransaction(t1);
        _transactions.Add(t1);

        p2.Process(t2);
        savingsAccount.ApplyTransaction(t2);
        _transactions.Add(t2);

        p3.Process(t3);
        savingsAccount.ApplyTransaction(t3);
        _transactions.Add(t3);
    }

    public static void Main(string[] args)

    {
        FinanceApp app = new FinanceApp();

        app.Run();
    }
}