using MongoDB.Bson.Serialization.Attributes;

public class OrderItem
{
    [BsonElement("name")]
    public string Name { get; set; } = "";

    [BsonElement("price")]
    public decimal Price { get; set; }
}