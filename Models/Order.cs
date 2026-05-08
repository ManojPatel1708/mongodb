using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("orderId")]
    public string OrderId { get; set; } = "";

    [BsonElement("customer")]
    public string Customer { get; set; } = "";

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("paidAt")]
    public DateTime? PaidAt { get; set; }

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();
}
