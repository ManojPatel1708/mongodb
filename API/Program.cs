using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var client = new MongoClient("mongodb://localhost:27017");
var db = client.GetDatabase("shop");
var orders = db.GetCollection<Order>("orders");

app.MapGet("/analytics/revenue", async () =>
{
    var result = await orders.Aggregate()
        .Match(o => o.Status == "PAID")
        .Group(o => 1, g => new
        {
            TotalRevenue = g.Sum(x => x.Amount)
        })
        .FirstOrDefaultAsync();

    return result;
});

app.MapGet("/analytics/orders-per-customer", async () =>
{
    var result = await orders.Aggregate()
        .Group(o => o.Customer, g => new
        {
            Customer = g.Key,
            Count = g.Count()
        })
        .ToListAsync();

    return result;
});

app.MapGet("/analytics/top-customers", async () =>
{
    var result = await orders.Aggregate()
        .Match(o => o.Status == "PAID")
        .Group(o => o.Customer, g => new
        {
            Customer = g.Key,
            Total = g.Sum(x => x.Amount)
        })
        .SortByDescending(x => x.Total)
        .Limit(5)
        .ToListAsync();

    return result;
});


app.MapGet("/orders", async () =>
{
    return await orders.Find(_ => true).ToListAsync();
});

app.MapGet("/orders/{id}", async (string id) =>
{


    return await orders.Find(o => o.OrderId == id).FirstOrDefaultAsync();
});

app.MapGet("/orders/paid", async () =>
{
    return await orders.Find(o => o.Status == "PAID").ToListAsync();
});

//GET /orders/search?customer=Manoj&status=PAID&page=1&pageSize=5
app.MapGet("/orders/search", async (
    string? customer,
    string? status,
    int page = 1,
    int pageSize = 10) =>
{
    var filterBuilder = Builders<Order>.Filter;
    var filter = filterBuilder.Empty;

    if (!string.IsNullOrEmpty(customer))
        filter &= filterBuilder.Eq(o => o.Customer, customer);

    if (!string.IsNullOrEmpty(status))
        filter &= filterBuilder.Eq(o => o.Status, status);

    var totalCount = await orders.CountDocumentsAsync(filter);

    var data = await orders.Find(filter)
        .Skip((page - 1) * pageSize)
        .Limit(pageSize)
        .ToListAsync();

    return Results.Ok(new
    {
        page,
        pageSize,
        totalCount,
        data
    });
});

app.Run();