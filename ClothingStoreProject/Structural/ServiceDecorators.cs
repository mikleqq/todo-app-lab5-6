using ClothingStore.Core;

namespace ClothingStore.Structural
{
    // Базовый декоратор
    public abstract class ProductDecorator : Product
    {
        protected Product _product;
        public ProductDecorator(Product product) => _product = product;
    }

    // Конкретный декоратор: Подарочная упаковка
    public class GiftWrapDecorator : ProductDecorator
    {
        public GiftWrapDecorator(Product product) : base(product)
        {
            this.Name = product.Name + " (+ Подарочная упаковка)";
            this.Price = product.Price + 500; // Добавляем 500 руб за упаковку
        }

        public override void DisplayInfo() => _product.DisplayInfo();
    }
}