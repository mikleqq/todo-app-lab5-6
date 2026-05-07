using System;
using Spectre.Console;
using ClothingStore.Core;
using ClothingStore.Creational;
using ClothingStore.Structural;
using ClothingStore.Behavioral;

class Program
{
    static void Main(string[] args)
    {
        // Инициализация (Singleton & Observer)
        Cart cart = Cart.GetInstance();
        var notifications = new StoreNotificationSystem();
        notifications.Subscribe(new Customer { Name = "Покупатель" });

        while (true)
        {
            Console.Clear();
            // Красивый заголовок
            var figlet = new FigletText("CLOTHING STORE").Color(Color.Blue);
            AnsiConsole.Write(figlet);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Выберите действие из меню:[/]")
                    .AddChoices(new[] {
                        "Добавить товар (Factory Method)",
                        "Спортивная коллекция (Abstract Factory)",
                        "Собрать костюм (Builder)",
                        "Посмотреть корзину (Singleton)",
                        "Оформить заказ (Strategy + Adapter)",
                        "Выход"
                    }));

            if (choice == "Выход") break;

            switch (choice)
            {
                case "Добавить товар (Factory Method)":
                    var itemType = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Что создаем?")
                            .AddChoices(new[] { "Футболка", "Обувь" }));

                    // Используем классы из ProductFactories.cs
                    Creator creator = itemType == "Футболка" ? new TShirtCreator() : new ShoesCreator();
                    Product item = creator.FactoryMethod($"{itemType} Basic", 2000);

                    if (AnsiConsole.Confirm("Добавить подарочную упаковку?"))
                        item = new GiftWrapDecorator(item); // Decorator

                    cart.AddToCart(item);
                    AnsiConsole.MarkupLine("[green]Товар успешно добавлен![/]");
                    break;

                case "Спортивная коллекция (Abstract Factory)":
                    IStyleFactory sport = new SportStyleFactory();
                    cart.AddToCart(sport.CreateTop());
                    cart.AddToCart(sport.CreateBottom());
                    AnsiConsole.MarkupLine("[blue]Спортивный набор добавлен в корзину![/]");
                    break;

                case "Собрать костюм (Builder)":
                    var suit = new OutfitBuilder().AddShirt().AddPants().AddJackets().Build();
                    suit.Show();
                    break;

                case "Посмотреть корзину (Singleton)":
                    var table = new Table();
                    table.AddColumn("Наименование");
                    table.AddColumn(new TableColumn("Цена").Centered());

                    foreach (var p in cart.Items)
                    {
                        table.AddRow(p.Name, $"[green]{p.Price}[/] руб.");
                    }
                    AnsiConsole.Write(table);
                    break;

                case "Оформить заказ (Strategy + Adapter)":
                    if (cart.Items.Count == 0) 
                    {
                        AnsiConsole.MarkupLine("[red]Корзина пуста![/]");
                        break;
                    }

                    double total = 0;
                    cart.Items.ForEach(i => total += i.Price);

                    var isVip = AnsiConsole.Confirm("Вы VIP-клиент (скидка 20%)?");
                    IDiscountStrategy strategy = isVip ? new VipDiscount() : new NoDiscount();
                    
                    double final = strategy.ApplyDiscount(total);
                    
                    AnsiConsole.MarkupLine($"[bold yellow]Итог к оплате: {final} руб.[/]");
                    
                    // Имитация оплаты через Adapter
                    new BankAdapter().Pay(final);
                    
                    notifications.NotifyAll("Ваш заказ успешно оплачен!");
                    cart.Items.Clear(); // Очистка после оплаты
                    break;
            }

            AnsiConsole.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}