using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using System.Net;
using SimpleOrderingSystem.Domain.Providers;

namespace SimpleOrderingSystem.Tests.Behavioral.Orders;

public class SearchOrdersSpecification : IClassFixture<WebApplicationFactoryFixture>
{
    // sut
    private readonly HttpClient _httpClient;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    
    // default return objects bound to mocks
    private List<OrderDataModel> _orderDataModelList =new()
    {
        new()
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
        }
    };

    public SearchOrdersSpecification(WebApplicationFactoryFixture webApplicationFactory)
    {
        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = webApplicationFactory.Mock<ILiteDbProvider>();
        _liteDbProviderMock
            .Setup(a=>a.SearchOrdersAsync(It.IsAny<DateTime>(),It.IsAny<Guid?>()))
            .ReturnsAsync(() => _orderDataModelList);

        // given I mock out the datetime provider and setup valid defaults
        _dateTimeProviderMock = webApplicationFactory.Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(a=> a.UtcNow())
            .Returns(() => Defaults.UtcNow);

        // given I have an HttpClient
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact(DisplayName = "Search orders should return valid order data from database and invoke database with correct parameters when no search criteria are supplied")]
    public async Task Test1()
    {
        // when I search orders
        var response = await SearchOrdersAsync();

        // then I expect the following 
        await response.ShouldBeOkWithResponseAsync(new TestOrderSearchResponseViewModel
        { 
            Orders = new List<TestOrderViewModel>()
            { 
                new TestOrderViewModel
                {
                    Id = Defaults.OrderId,
                    Status = "New",
                    Type = "Shipped",
                    CreatedDateTimeUtc = Defaults.DateCreated,
                    CancelledDateTimeUtc = default,
                    CompletedDateTimeUtc = Defaults.DateCompleted,
                    Shipping = 7.25M,
                    DiscountTotal = 5M,
                    LineItemTotal = 105M,
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
                }
            }
        });

        // then I expect the order database to have been called with the following
        _liteDbProviderMock
            .Verify(a=>a.SearchOrdersAsync(Defaults.UtcNow.Subtract(TimeSpan.FromDays(1)), default),Times.Once());
    }

    
    [Fact(DisplayName = "Search orders should search using supplied search parameters allowing noOlderThan to be greater than 7 days when a customerId is supplied")]
    public async Task Test2()
    {
        // given i have a no older than date
        var noOlderThan = Defaults.UtcNow.Subtract(TimeSpan.FromDays(50));

        // when I get an order
        var response = await SearchOrdersAsync(Defaults.CustomerId.ToString(), noOlderThan);

        // the I expect the response to be Ok
        response.ShouldBeOk();

        // then I expect the order database to have been called with the following
        _liteDbProviderMock
            .Verify(a=>a.SearchOrdersAsync(noOlderThan, Defaults.CustomerId),Times.Once());
    }

    [Fact(DisplayName = "Search orders should limit the no older date to 7 days from current date when no customerId is supplied and noOlderThan date that is greater than 7 days is supplied")]
    public async Task Test3()
    {
        // given i have a no older than date
        var noOlderThan = Defaults.UtcNow.Subtract(TimeSpan.FromDays(55));

        // when I get an order
        var response = await SearchOrdersAsync(default, noOlderThan);

        // the I expect the response to be Ok
        response.ShouldBeOk();

        // then I expect the order database to have been called with the following
        _liteDbProviderMock
            .Verify(a=>a.SearchOrdersAsync(Defaults.UtcNow.Subtract(TimeSpan.FromDays(7)), default),Times.Once());
    }

    #region Helpers

    public async Task<HttpResponseMessage> SearchOrdersAsync(string? customerId = default, DateTime? noOlderThan = default)
    {
        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        if(customerId is not null)
        {
            queryString.Add("customerId", customerId);
        }
        if(noOlderThan.HasValue)
        {
            queryString.Add("noOlderThan", noOlderThan.Value.ToString()); // noOlderThan.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
        }

        return await _httpClient.GetAsync($"Orders/search?{queryString}");
    }

    #endregion
}