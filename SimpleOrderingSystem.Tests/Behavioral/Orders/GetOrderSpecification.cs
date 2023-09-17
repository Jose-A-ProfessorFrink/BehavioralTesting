using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

namespace SimpleOrderingSystem.Tests.Behavioral.Orders;

public class GetOrderSpecification : IDisposable
{
    // sut
    private readonly WebApplicationFactory _webApplicationFactory;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    
    // default return objects bound to mocks
    private OrderDataModel? _orderDataModel = new()
    {
            Id = Defaults.OrderId,
            Status = Domain.Models.OrderStatus.New,
            Type = Domain.Models.OrderType.Shipped,
            CreatedDateTimeUtc = Defaults.DateCreated,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = Defaults.DateCompleted,
            Shipping = 7.25M,
            LineItemTotal = 105M,
            DiscountTotal = 5M,
            Customer = new()
            {
                Id = Defaults.CustomerId,
                Name = Defaults.CustomerName,
                DateOfBirth = Defaults.CustomerDateOfBirth,
                DateHired = Defaults.CustomerDateHired,
                AnnualSalary = Defaults.CustomerAnnualSalary
            },
            ShippingAddress = new()
            {
                Line1  = "1121 Ash Lane",
                Line2 = "Southwest",
                City  = "Beverly Hills",
                State  = "CA",
                ZipCode  = "90210"
            },
            Discounts = new()
            {
                new() {Type = Domain.Models.DiscountType.LargeOrder, PercentDiscount = .10M },
                new()  {Type = Domain.Models.DiscountType.SeniorCitizen, PercentDiscount = .15M}
            },
            Items = new()
            {
                new()
                {
                    MovieId = "tt0083658",
                    MovieYear = "1984",
                    MovieMetascore = "81",
                    Quantity = 3,
                    Price = 10.25M
                },
                new()
                {
                    MovieId = "tt0089999",
                    MovieYear = "2002",
                    MovieMetascore = "54",
                    Quantity = 1,
                    Price = 17.54M
                },
            }
    };

    public GetOrderSpecification()
    {
        // given I have a web application factory
        _webApplicationFactory = WebApplicationFactory.Create();

        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = _webApplicationFactory.Mock<ILiteDbProvider>();
        _liteDbProviderMock
            .Setup(a=>a.GetOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => _orderDataModel);
    }

    [Fact(DisplayName = "Get order should return not found when order id is invalid")]
    public async Task Test1()
    {
        // when I get an order
        var response = await GetOrderAsync("not a guid");

        // the I expect the response to be not found
        response.ShouldBeNotFound();
    }

    
    [Fact(DisplayName = "Get order should return not found when order is not found")]
    public async Task Test2()
    {
        // given there is no order
        _orderDataModel = default;

        // when I get an order
        var response = await GetOrderAsync(Defaults.OrderId.ToString());

        // the I expect the response to be not found
        response.ShouldBeNotFound();
    }

    [Fact(DisplayName = "Get order should return correct order data from database and invoke database with correct parameters")]
    public async Task Test3()
    {
        // when I get an order
        var response = await GetOrderAsync(Defaults.OrderId.ToString());

        // then I expect the following 
        await response.ShouldBeOkWithResponseAsync(new TestOrderViewModel
        {
            Id = Defaults.OrderId,
            Status = "New",
            Type = "Shipped",
            CreatedDateTimeUtc = Defaults.DateCreated,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = Defaults.DateCompleted,
            Shipping = 7.25M,
            LineItemTotal = 105M,
            DiscountTotal = 5M,
            TotalCost = 107.25M,  
            Customer = new()
            {
                Id = Defaults.CustomerId,
                Name = Defaults.CustomerName,
                DateOfBirth = Defaults.CustomerDateOfBirth,
                DateHired = Defaults.CustomerDateHired,
                AnnualSalary = Defaults.CustomerAnnualSalary
            },
            ShippingAddress = new()
            {
                Line1  = "1121 Ash Lane",
                Line2 = "Southwest",
                City  = "Beverly Hills",
                State  = "CA",
                ZipCode  = "90210"
            },
            Discounts = new()
            {
                new() {Type = "LargeOrder", PercentDiscount = .10M },
                new()  {Type = "SeniorCitizen", PercentDiscount = .15M}
            },
            Items = new()
            {
                new()
                {
                    MovieId = "tt0083658",
                    MovieYear = "1984",
                    MovieMetascore = "81",
                    Quantity = 3,
                    Price = 10.25M
                },
                new()
                {
                    MovieId = "tt0089999",
                    MovieYear = "2002",
                    MovieMetascore = "54",
                    Quantity = 1,
                    Price = 17.54M
                },
            }  
        });

        // then I expect the provider to have been called with the following
        _liteDbProviderMock.Verify(a=>a.GetOrderAsync(Defaults.OrderId), Times.Once());
    }

    #region Helpers

    public async Task<HttpResponseMessage> GetOrderAsync(string orderId)
    {
        return await _webApplicationFactory.CreateClient().GetAsync($"Orders/{orderId}");
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }

    #endregion
}