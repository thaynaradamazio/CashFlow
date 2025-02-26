using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Extensions
{
    public static class PaymentTypeExtensions
    {
        public static string PaymentTypeToString(this PaymentType paymentType)
        {
            return paymentType switch
            {
                Domain.Enums.PaymentType.Cash => "Dinheiro",
                Domain.Enums.PaymentType.CreditCard => "Cartão de crédito",
                Domain.Enums.PaymentType.DebitCard => "Cartão de débito",
                Domain.Enums.PaymentType.EletronicTransfer => "Transferência bancária",
                _ => string.Empty
            };
        }
    }
}
