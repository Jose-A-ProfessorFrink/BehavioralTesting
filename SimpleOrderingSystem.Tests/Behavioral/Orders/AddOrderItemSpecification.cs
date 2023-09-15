using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Domain.Providers;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;


namespace SimpleOrderingSystem.Tests.Behavioral.Orders;

public class AddOrderItemSpecification : IDisposable
{
    // sut
    private readonly WebApplicationFactory _webApplicationFactory;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IMovieProvider> _movieProviderMock;
    
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

            }  
        };
    private GetMovieApiResponse _getMovieApiResponse = new()
    {
        ImdbId= "tt1856101",
        Title= "Blade Runner 2049",
        Year= "2017",
        Rated= "R",
        Released= "06 Oct 2017",
        Plot= "a plot",
        Poster= "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_SX300.jpg",
        Metascore= "81",
        ImdbRating= "8.0",
        Type= "movie",
        Response= "True",
        Error = default
    };  
    private bool _updateResult = true;

    // default request
    private string _orderId = Defaults.OrderId.ToString();
    private TestAddOrderItemRequestViewModel _request = new()
    {
        MovieId = Defaults.MovieId,
        Quantity = 1
    };

    public AddOrderItemSpecification()
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

        // given I mock out the movie provider and setup appropriate defaults
        _movieProviderMock = _webApplicationFactory.Mock<IMovieProvider>();
        _movieProviderMock
            .Setup(a=>a.GetMovieAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(() => _getMovieApiResponse);

        // given I mock out the datetime provider and setup valid defaults
        _dateTimeProviderMock = _webApplicationFactory.Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(a=> a.UtcNow())
            .Returns(() => Defaults.UtcNow);
    }

    [Fact(DisplayName = "Add order item should return bad request with appropriate error code when order id is invalid")]
    public async Task Test1()
    {
        // given the orderId is invalid
        _orderId = "not a guid";

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "OrderIdInvalid",
            "The supplied order id is invalid");
    }

    [Fact(DisplayName = "Add order item should return bad request with appropriate error code when order is not found")]
    public async Task Test2()
    {
        // given the order is not found
        _orderDataModel = default;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "OrderIdInvalid",
            "The supplied order id is invalid");
    }

    [Fact(DisplayName = "Add order item order should return bad request with appropriate error code when trying to add item to order without New status")]
    public async Task Test3()
    {
        // given the order is not new
        _orderDataModel = _orderDataModel! with { Status = Domain.Models.OrderStatus.Completed};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "InvalidRequest",
            "There is something wrong with your request. Please see details for more information.",
            "Cannot add item to order because the order status does not allow adding items.");
    }

    [Fact(DisplayName = "Add order item order should return bad request with appropriate error code when trying to add item that does not exist")]
    public async Task Test4()
    {
        // given I set the movie response to invalid
        _getMovieApiResponse = _getMovieApiResponse with { Response = "False" } ;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "MovieIdInvalid",
            "Invalid movie id was supplied. Please provide a valid imdb movie id (i.e. like 'tt3896198'). See https://developer.imdb.com/documentation/key-concepts#imdb-ids for more information.");
    }

    [Fact(DisplayName = "Add order item should return internal server error when update fails")]
    public async Task Test10()
    {
        // given i setup the update to fail
        _updateResult = false;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        response.ShouldBeInternalServerError();
    }

    #region Helpers

    public async Task<HttpResponseMessage> AddOrderItemAsync(string? orderId, TestAddOrderItemRequestViewModel request)
    {
        return await _webApplicationFactory.CreateClient().PostAsJsonAsync($"Orders/{orderId}/items" , request);
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }

    #endregion
}