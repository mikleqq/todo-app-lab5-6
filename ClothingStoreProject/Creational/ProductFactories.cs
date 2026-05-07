using System;
using System.Collections.Generic;
using ClothingStore.Core;

namespace ClothingStore.Creational
{
    // 1. SINGLETON: Корзина (должна быть только одна на всё приложение)
    public class Cart
    {
        private static Cart _instance;
        public List<Product> Items { get; private set; } = new List<Product>();

        private Cart() { } // Приватный конструктор

        public static Cart GetInstance()
        {
            if (_instance == null) _instance = new Cart();
            return _instance;
        }

        public void AddToCart(Product product)
        {
            Items.Add(product);
            Console.WriteLine($"-> {product.Name} добавлено в корзину.");
        }
    }

    // 2. FACTORY METHOD: Фабрика для создания товаров
    public abstract class Creator
    {
        public abstract Product FactoryMethod(string name, double price);
    }

    public class TShirtCreator : Creator
    {
        public override Product FactoryMethod(string name, double price) => new TShirt(name, price);
    }

    public class ShoesCreator : Creator
    {
        public override Product FactoryMethod(string name, double price) => new Shoes(name, price);
    }

    // 3. PROTOTYPE: Клонирование товара (чтобы не создавать заново)
    public class ProductCloner
    {
        public static Product Clone(Product p)
        {
            // Упрощенная реализация клонирования для примера
            if (p is TShirt) return new TShirt(p.Name + " (Копия)", p.Price);
            if (p is Shoes) return new Shoes(p.Name + " (Копия)", p.Price);
            return null;
        }
    }
}