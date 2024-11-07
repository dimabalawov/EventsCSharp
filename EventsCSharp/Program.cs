using System;


CreditCard card = new CreditCard("123456789", "Иван Иванов", "1234", DateTime.Now.AddYears(3), 5000);

card.BalanceReplenished += (sender, e) => Console.WriteLine($"Счёт пополнен на {e.Amount} гривен.");

card.FundsSpent += (sender, e) => Console.WriteLine($"Потрачено {e.Amount} гривен.");

card.CreditLimitUsed += (sender, e) => Console.WriteLine("Началось использование кредитных средств!");

card.TargetBalanceReached += (sender, e) => Console.WriteLine($"Достигнута целевая сумма: {e.Amount} гривен.");

card.PinChanged += (sender, e) => Console.WriteLine("PIN изменён.");

card.ReplenishBalance(2000);
card.SpendFunds(2500);
card.ChangePIN("5678");
card.CheckTargetBalance(2000);

public class CreditCardEventArgs : EventArgs
{
    public decimal Amount { get; }
    public CreditCardEventArgs(decimal amount) => Amount = amount;
}

public class CreditCard
{
    public string CardNumber { get; }
    public string OwnerName { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string PIN { get; private set; }
    public decimal CreditLimit { get; set; }
    public decimal Balance { get; private set; }

    public event EventHandler<CreditCardEventArgs> BalanceReplenished;
    public event EventHandler<CreditCardEventArgs> FundsSpent;
    public event EventHandler<CreditCardEventArgs> CreditLimitUsed;
    public event EventHandler<CreditCardEventArgs> TargetBalanceReached;
    public event EventHandler PinChanged;

    public CreditCard(string cardNumber, string ownerName, string pin, DateTime expiryDate, decimal creditLimit)
    {
        CardNumber = cardNumber;
        OwnerName = ownerName;
        PIN = pin;
        ExpiryDate = expiryDate;
        CreditLimit = creditLimit;
        Balance = 0;
    }

    public void ReplenishBalance(decimal amount)
    {
        Balance += amount;
        OnBalanceReplenished(new CreditCardEventArgs(amount));
    }

    public void SpendFunds(decimal amount)
    {
        if (Balance + CreditLimit >= amount)
        {
            Balance -= amount;
            OnFundsSpent(new CreditCardEventArgs(amount));

            if (Balance < 0)
            {
                OnCreditLimitUsed(new CreditCardEventArgs(amount));
            }
        }
        else
        {
            Console.WriteLine("Недостаточно средств для совершения операции.");
        }
    }

    public void ChangePIN(string newPin)
    {
        PIN = newPin;
        OnPinChanged();
    }

    protected virtual void OnBalanceReplenished(CreditCardEventArgs e) => BalanceReplenished?.Invoke(this, e);
    protected virtual void OnFundsSpent(CreditCardEventArgs e) => FundsSpent?.Invoke(this, e);
    protected virtual void OnCreditLimitUsed(CreditCardEventArgs e) => CreditLimitUsed?.Invoke(this, e);
    protected virtual void OnTargetBalanceReached(CreditCardEventArgs e) => TargetBalanceReached?.Invoke(this, e);
    protected virtual void OnPinChanged() => PinChanged?.Invoke(this, EventArgs.Empty);

    public void CheckTargetBalance(decimal targetBalance)
    {
        if (Balance >= targetBalance)
            OnTargetBalanceReached(new CreditCardEventArgs(Balance));
    }
}
