using System;
using System.Collections.Generic;

namespace ClothingStore.Behavioral
{
    // --- ПАТТЕРН STRATEGY (Стратегия скидок) ---
    public interface IDiscountStrategy
    {
        double ApplyDiscount(double price);
    }

    public class NoDiscount : IDiscountStrategy 
    {
        public double ApplyDiscount(double price) => price;
    }

    public class VipDiscount : IDiscountStrategy 
    {
        public double ApplyDiscount(double price) => price * 0.8; // Скидка 20%
    }

    // --- ПАТТЕРН OBSERVER (Система уведомлений) ---
    public class Customer
    {
        public string Name { get; set; }
        public void Update(string message) => 
            Console.WriteLine($"[SMS для {Name}]: {message}");
    }

    public class StoreNotificationSystem
    {
        private List<Customer> _customers = new List<Customer>();

        public void Subscribe(Customer customer) => _customers.Add(customer);
        
        public void NotifyAll(string message)
        {
            foreach (var c in _customers) c.Update(message);
        }
    }
}