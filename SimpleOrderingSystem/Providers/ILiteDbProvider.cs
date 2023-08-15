using SimpleOrderingSystem.DataModels;
using LiteDB;

namespace SimpleOrderingSystem.Providers;

internal interface ILiteDbProvider
{
    ILiteCollection<CustomerDataModel> GetCustomersCollection();

    ILiteCollection<OrderDataModel> GetOrdersCollection();
}