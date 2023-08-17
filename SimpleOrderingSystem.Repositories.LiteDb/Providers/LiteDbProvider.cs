using LiteDB;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

namespace SimpleOrderingSystem.Repositories.LiteDB.Providers;

internal class LiteDbProvider: ILiteDbProvider
{
    internal static readonly string CustomersCollectionName = "customers";
    internal static readonly string OrdersCollectionName = "orders";
    private readonly string _connectionString;
    private readonly Lazy<ILiteDatabase> _database;

    public LiteDbProvider(string connectionString)
    {   
        _connectionString = connectionString;
        _database = new Lazy<ILiteDatabase>(() => new LiteDatabase(_connectionString), true);
    }

    public Task<CustomerDataModel> GetCustomerAsync(Guid id)
    {
        var customers = GetCustomersCollection();

        var result = customers
            .Query()
            .Where(a=>a.Id == id)
            .SingleOrDefault();

        return Task.FromResult(result);
    }

    public Task<List<CustomerDataModel>> SearchCustomersAsync(string name)
    {
        var customers = GetCustomersCollection();

        return Task.FromResult(customers
            .Query()
            .Where(a=>a.Name.Contains(name))
            .OrderBy(a=>a.Name)
            .ToList());//force materialization of query
    }

    public Task CreateOrderAsync(OrderDataModel order)
    {
        var orders = GetOrdersCollection();

        orders.Insert(order);

        return Task.CompletedTask;
    }

    public Task<bool> UpdateOrderAsync(OrderDataModel order)
    {
        var orders = GetOrdersCollection();

        return Task.FromResult(orders.Update(order));
    }

    public Task<OrderDataModel?> GetOrderAsync(Guid id)
    {
        var orders = GetOrdersCollection();

        var order = orders
            .Query()
            .Where(a=>a.Id == id)
            .SingleOrDefault();

        return Task.FromResult<OrderDataModel?>(order);
    }

    public Task<List<OrderDataModel>> SearchOrdersAsync(DateTime noOlderThanUtc, Guid? customerId)
    {
        var orders = GetOrdersCollection();

        var query = orders
            .Query()
            .Where(a=> a.CreatedDateTimeUtc >= noOlderThanUtc);
        
        if(customerId.HasValue)
        {
            query = query.Where(a=> a.Customer.Id == customerId);
        }

        return Task.FromResult(query.OrderByDescending(a=>a.CreatedDateTimeUtc).ToList());
    }

    public void Initialize()
    {
        if(!Database.CollectionExists(OrdersCollectionName))  
        {
            var ordersCollection = Database.GetCollection<OrderDataModel>(OrdersCollectionName);
            ordersCollection.EnsureIndex(a=>a.Id);
        }

        if(!Database.CollectionExists(CustomersCollectionName))
        {
            // Get a collection (or create, if doesn't exist)
            var customersCollection = Database.GetCollection<CustomerDataModel>(CustomersCollectionName);
            customersCollection.EnsureIndex(a=>a.Id);
            customersCollection.EnsureIndex(a=>a.Name);


            var random = new Random();
            var firstNames = new string[] { "Rick", "Homer", "Peter" , "Lois" ,"Stan" , "Francine" , "Stebe", "Morty", "Lisa" , "Bart" };
            var middleNames = new string[] { "Alissa", "Dawn", "Marie" , "Ayden" ,"Mary" , "Elmer" , "James", "Joseph", "Phillip" , "Jay" };
            var lastNames = new string[] { "Sanchez", "Simpson", "Griffin" , "Bullocks" ,"Smith" , "Jimenez" , "Hornsby", "King", "Ellis" , "Jones" };
            List<CustomerDataModel> seedCustomers = new List<CustomerDataModel>();
            for(int i = 0; i < 300; i++)
            {
                var name = $"{Random(random, firstNames)} {Random(random, middleNames)} {Random(random, lastNames)}";
                var birthYear = DateTime.Now.Year - 80 + random.Next(50);
                seedCustomers.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Name = $"{Random(random, firstNames)} {Random(random, middleNames)} {Random(random, lastNames)}",
                    AnnualSalary = Convert.ToDecimal(random.Next(12000, 450000)),
                    DateOfBirth = DateOnly.Parse($"{random.Next(1,13)}/{random.Next(1, 29)}/{birthYear}"),
                    DateHired = DateOnly.Parse($"{random.Next(1,13)}/{random.Next(1, 29)}/{birthYear + 18 + random.Next(10)}")
                });
            }

            customersCollection.InsertBulk(seedCustomers);
        }  
    }

    #region Helpers

    private T Random<T>(Random random, IEnumerable<T> set)
    {
        var items = set.ToArray();
        var index = random.Next(0, items.Length);
        return items[index];
    }

    private ILiteDatabase Database => _database.Value;

    private ILiteCollection<CustomerDataModel> GetCustomersCollection()
    {
        return Database.GetCollection<CustomerDataModel>(CustomersCollectionName);
    }

    private ILiteCollection<OrderDataModel> GetOrdersCollection()
    {
        return Database.GetCollection<OrderDataModel>(OrdersCollectionName);
    }
    
    #endregion
}