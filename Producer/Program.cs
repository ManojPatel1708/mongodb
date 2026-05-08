using Confluent.Kafka;
using System.Text.Json;

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092"
};

using var producer = new ProducerBuilder<string, string>(config).Build();

Console.WriteLine("🚀 Sending order events...");

var random = new Random();

while (true)
{
    var order = new Order
    {
        OrderId = "ORD-" + random.Next(100, 999),
        Customer = "Customer-" + random.Next(1, 10),
        Amount = random.Next(500, 15000),
        Status = "CREATED",
        CreatedAt = DateTime.UtcNow,
        Items = new List<OrderItem>
        {
            new OrderItem { Name = "Item-A", Price = 500 },
            new OrderItem { Name = "Item-B", Price = 1000 }
        }
    };

    var json = JsonSerializer.Serialize(order);

    await producer.ProduceAsync(
        "orders",
        new Message<string, string>
        {
            Key = order.OrderId,
            Value = json
        });

    Console.WriteLine($"✅ Sent: {json}");

    await Task.Delay(2000);
}