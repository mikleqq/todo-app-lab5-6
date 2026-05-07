using System;

namespace ClothingStore.Core
{
    // Базовый абстрактный класс для любого товара
    public abstract class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }

        public abstract void DisplayInfo();
        
        // Вспомогательный метод для изменения цены (понадобится для Strategy)
        public void SetPrice(double newPrice) => Price = newPrice;
    }

    // Конкретный товар: Футболка
    public class TShirt : Product
    {
        public TShirt(string name, double price) 
        {
            Name = name;
            Price = price;
            Category = "Верхняя одежда";
        }
        public override void DisplayInfo() => 
            Console.WriteLine($"[Товар] {Category}: {Name} | Цена: {Price} руб.");
    }

    // Конкретный товар: Обувь
    public class Shoes : Product
    {
        public Shoes(string name, double price) 
        {
            Name = name;
            Price = price;
            Category = "Обувь";
        }
        public override void DisplayInfo() => 
            Console.WriteLine($"[Товар] {Category}: {Name} | Цена: {Price} руб.");
    }
}