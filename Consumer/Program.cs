using Confluent.Kafka;
using MongoDB.Driver;
using System.Text.Json;

var kafkaConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "order-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var mongoClient = new MongoClient("mongodb://localhost:27017");
var database = mongoClient.GetDatabase("shop");
var collection = database.GetCollection<Order>("orders");

using var consumer = new ConsumerBuilder<string, string>(kafkaConfig).Build();
consumer.Subscribe("orders");

Console.WriteLine("🚀 Listening for order events...");

while (true)
{
    try
    {
        var cr = consumer.Consume();

        var order = JsonSerializer.Deserialize<Order>(cr.Message.Value);

        // Add processing timestamp
        order.CreatedAt = DateTime.UtcNow;

        // UPSERT (VERY IMPORTANT)
        var filter = Builders<Order>.Filter.Eq(o => o.OrderId, order.OrderId);

        await collection.ReplaceOneAsync(
            filter,
            order,
            new ReplaceOptions { IsUpsert = true }
        );

        Console.WriteLine($"✅ Stored OrderId={order.OrderId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}