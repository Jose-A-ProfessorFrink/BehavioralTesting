using System.Globalization;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using SimpleOrderingSystem.DataModels;

namespace SimpleOrderingSystem.Providers;

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

    public ILiteCollection<CustomerDataModel> GetCustomersCollection()
    {
        return Database.GetCollection<CustomerDataModel>(CustomersCollectionName);
    }

    public ILiteCollection<OrderDataModel> GetOrdersCollection()
    {
        return Database.GetCollection<OrderDataModel>(OrdersCollectionName);
    }

    public void Initialize()
    {
        if(!Database.CollectionExists(OrdersCollectionName))  
        {
            var customersCollection = Database.GetCollection<OrderDataModel>(OrdersCollectionName);
            customersCollection.EnsureIndex(a=>a.Id);
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

    private void SeedData()
    {
        
    }
    private T Random<T>(Random random, IEnumerable<T> set)
    {
        var items = set.ToArray();
        var index = random.Next(0, items.Length);
        return items[index];
    }

    private ILiteDatabase Database => _database.Value;
    
    #endregion
}