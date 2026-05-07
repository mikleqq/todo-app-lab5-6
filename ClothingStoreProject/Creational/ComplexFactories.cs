using System;
using System.Collections.Generic;
using ClothingStore.Core;

namespace ClothingStore.Creational
{
    // --- ПАТТЕРН BUILDER ---
    // Позволяет собрать "Костюм" по частям
    public class Outfit
    {
        public List<string> Parts = new List<string>();
        public void Show() => Console.WriteLine("Комплект собран: " + string.Join(", ", Parts));
    }

    public class OutfitBuilder
    {
        private Outfit _outfit = new Outfit();

        public OutfitBuilder AddShirt() { _outfit.Parts.Add("Рубашка"); return this; }
        public OutfitBuilder AddPants() { _outfit.Parts.Add("Брюки"); return this; }
        public OutfitBuilder AddJackets() { _outfit.Parts.Add("Пиджак"); return this; }

        public Outfit Build() => _outfit;
    }

    // --- ПАТТЕРН ABSTRACT FACTORY ---
    // Создает целые наборы вещей одного стиля
    public interface IStyleFactory
    {
        Product CreateTop();
        Product CreateBottom();
    }

    public class SportStyleFactory : IStyleFactory
    {
        public Product CreateTop() => new TShirt("Спортивная майка", 2000);
        public Product CreateBottom() => new Shoes("Беговые кроссовки", 7000);
    }
}