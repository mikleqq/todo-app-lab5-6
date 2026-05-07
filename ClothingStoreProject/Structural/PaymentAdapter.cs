using System;

namespace ClothingStore.Structural
{
    // Внешняя сложная система банка (которую мы не можем менять)
    public class ExternalBankAPI
    {
        public void CommitTransaction(double amount) => 
            Console.WriteLine($"Банк подтвердил перевод на сумму {amount} руб.");
    }

    // Адаптер, который делает API банка удобным для нашего магазина
    public class BankAdapter
    {
        private ExternalBankAPI _bankAPI = new ExternalBankAPI();

        public void Pay(double amount)
        {
            Console.WriteLine("Процессинг оплаты через Адаптер...");
            _bankAPI.CommitTransaction(amount);
        }
    }
}