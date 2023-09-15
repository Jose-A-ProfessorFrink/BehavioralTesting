using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Domain.Providers;
using System.Diagnostics.CodeAnalysis;


namespace SimpleOrderingSystem.Tests.Behavioral.Orders;

public class CompleteOrderSpecification : IDisposable
{
    // sut
    private readonly WebApplicationFactory _webApplicationFactory;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    
    // default return objects bound to mocks
    private OrderDataModel? _orderDataModel = 
        new()
        {
            Id = Defaults.OrderId,
            Status = Domain.Models.OrderStatus.New,
            Type = Domain.Models.OrderType.Shipped,
            CreatedDateTimeUtc = Defaults.DateCreated,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = default,
            Shipping = 7.25M,
            TotalCost = 105M,
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
                ZipCode  = Defaults.ZipCode
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
    private bool _updateResult = true;

    // default request
    private TestCreateOrderRequestViewModel _request = new()
    {
        CustomerId = Defaults.CustomerId.ToString(),
        Type = "Shipped",
        ShippingAddress = new()
        {
            Line1  = "1121 Ash Lane",
            Line2 = "Southwest",
            City  = "Beverly Hills",
            State  = "CA",
            ZipCode  = "90210"           
        }
    };

    public CompleteOrderSpecification()
    {
        // given I have a web application factory
        _webApplicationFactory = WebApplicationFactory.Create();

        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = _webApplicationFactory.Mock<ILiteDbProvider>();
        _liteDbProviderMock
            .Setup(a=>a.GetOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => _orderDataModel);
        _liteDbProviderMock
            .Setup(a=> a.UpdateOrderAsync(It.IsAny<OrderDataModel>()))
            .ReturnsAsync(() => _updateResult);

        // given I mock out the datetime provider and setup valid defaults
        _dateTimeProviderMock = _webApplicationFactory.Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(a=> a.UtcNow())
            .Returns(() => Defaults.UtcNow);
    }

    [Fact(DisplayName = "Complete order should return bad request with appropriate error code when order id is invalid")]
    public async Task Test1()
    {
        // given the orderId is invalid
        var orderId = "not a guid";

        // when I complete an order
        var response = await CompleteOrderAsync(orderId);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "OrderIdInvalid",
            "The supplied order id is invalid");
    }

    [Fact(DisplayName = "Complete order should return bad request with appropriate error code when order is not found")]
    public async Task Test2()
    {
        // given the customer is not found
        _orderDataModel = default;

        // when I complete an order
        var response = await CompleteOrderAsync(Defaults.OrderId.ToString());

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "OrderIdInvalid",
            "The supplied order id is invalid");
    }

    [Fact(DisplayName = "Complete order should return bad request with appropriate error code when trying to complete order without New status")]
    public async Task Test3()
    {
        // given the order is not new
        _orderDataModel = _orderDataModel! with { Status = Domain.Models.OrderStatus.Completed};

        // when I complete an order
        var response = await CompleteOrderAsync(Defaults.OrderId.ToString());

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "InvalidRequest",
            "There is something wrong with your request. Please see details for more information.",
            "Cannot complete order because the order status does not allow completion.");
    }

    [Fact(DisplayName = "Complete order should return bad request with appropriate error code when trying to complete order without items")]
    public async Task Test4()
    {
        // given the order has no items
        _orderDataModel = _orderDataModel! with { Items = new List<OrderItemDataModel>()};

        // when I complete an order
        var response = await CompleteOrderAsync(Defaults.OrderId.ToString());

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "InvalidRequest",
            "There is something wrong with your request. Please see details for more information.",
            "Cannot complete order because the order does not contain any items!");
    }

    [Fact(DisplayName = "Complete order should store new order in the database correctly and return the correct order data")]
    public async Task Test9()
    {
        // when I complete an order
        var response = await CompleteOrderAsync(Defaults.OrderId.ToString());

        // then I expect the response to be created and contain the following
        await response.ShouldBeOkWithResponseAsync(new TestOrderViewModel
        {
            Id = Defaults.OrderId,
            Status = "Completed",
            Type = "Shipped",
            CreatedDateTimeUtc = Defaults.DateCreated,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = Defaults.UtcNow,
            Shipping = 7.25M,
            TotalCost = 105M,
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
    
        // then I expect the database to have been called with the following
        _liteDbProviderMock
            .Verify(a=> a.UpdateOrderAsync(ItShould.Be(new OrderDataModel()
            {
                Id = Defaults.OrderId,
                Status = Domain.Models.OrderStatus.Completed,
                Type = Domain.Models.OrderType.Shipped,
                CreatedDateTimeUtc = Defaults.DateCreated,
                CancelledDateTimeUtc = default,
                CompletedDateTimeUtc = Defaults.UtcNow,
                Shipping = 7.25M,
                TotalCost = 105M,
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
                    ZipCode  = Defaults.ZipCode
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
            })), Times.Once());
    }

    [Fact(DisplayName = "Complete order should return internal server error when update fails")]
    public async Task Test10()
    {
        // given i setup the update to fail
        _updateResult = false;

        // when I create an order
        var response = await CompleteOrderAsync(Defaults.OrderId.ToString());

        // the I expect the response to be the following error
        response.ShouldBeInternalServerError();
    }

    #region Helpers

    public async Task<HttpResponseMessage> CompleteOrderAsync(string? orderId)
    {
        return await _webApplicationFactory.CreateClient().PostAsync($"Orders/{orderId}/complete" , default);
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }

    #endregion
}