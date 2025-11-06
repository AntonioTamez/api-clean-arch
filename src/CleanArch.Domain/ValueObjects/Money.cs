using CleanArch.Domain.Common;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.ValueObjects;

/// <summary>
/// Value Object que representa dinero
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    // Constructor privado para EF Core
    private Money()
    {
        Currency = string.Empty;
    }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Crea una instancia de Money
    /// </summary>
    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        if (currency.Length != 3)
            throw new DomainException("Currency must be a 3-letter ISO code");

        return new Money(amount, currency.ToUpperInvariant());
    }

    /// <summary>
    /// Crea Money con valor cero
    /// </summary>
    public static Money Zero(string currency) => Create(0, currency);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    // Operadores
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot add money with different currencies");

        return Create(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot subtract money with different currencies");

        return Create(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return Create(money.Amount * multiplier, money.Currency);
    }
}
