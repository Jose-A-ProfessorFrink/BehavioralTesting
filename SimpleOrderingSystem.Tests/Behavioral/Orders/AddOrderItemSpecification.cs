using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Domain.Providers;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Domain.Models;


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
            DiscountTotal = 0M,
            LineItemTotal = 105M,
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
        ImdbId = Defaults.MovieId,
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

    [Fact(DisplayName = "Add order item should return bad request when order id is invalid")]
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

    [Fact(DisplayName = "Add order item should return bad request when order is not found")]
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

    [Fact(DisplayName = "Add order item should return bad request when trying to add item to order without New status")]
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

    [Fact(DisplayName = "Add order item should return bad request when trying to add item that does not exist")]
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

    [Fact(DisplayName = "Add order item should return bad request when adding new order item with quantity greater than 20")]
    public async Task Test5()
    {
        // given I set the request to add an order item with more than 20 quantity
        _request.Quantity = 21;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "InvalidRequest",
            "There is something wrong with your request. Please see details for more information.",
            "Unable to add items because that would exceed the maximum movies for a single order. An order can have up to 20 items."
        );
    }

    [Fact(DisplayName = "Add order item should return bad request when adding new order item to order with existing items whose new total quantity exceeds 20")]
    public async Task Test5_1()
    {
        // given I already have an order with other order items
        _orderDataModel = _orderDataModel! with { Items = new()
            {
                new() 
                { 
                    MovieId = "movie1",
                    Quantity = 7
                },
                new() 
                { 
                    MovieId = "movie2",
                    Quantity = 13
                }
            }
        };

        // given I set the request to add an order item with a single quantity
        _request.Quantity = 1;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "InvalidRequest",
            "There is something wrong with your request. Please see details for more information.",
            "Unable to add items because that would exceed the maximum movies for a single order. An order can have up to 20 items."
        );
    }

    [Theory(DisplayName = "Add order item should calculate item price correctly when ")]
    [InlineData("movie has no known released year", "", "100", 5)]
    [InlineData("movie has invalid movie year", "hello", "100", 5)]
    [InlineData("movie has no known metascore", "1984", "", 5)]
    [InlineData("movie has invalid metascore", "1984", "hello", 5)]
    [InlineData("movie was released before 1945", "1944", "100", 2)]
    [InlineData("movie was released in 1945", "1945", "100", 2)]
    [InlineData("movie was released in 1970", "1970", "100", 6)]
    [InlineData("movie was released in 2000", "2000", "100", 12)]
    [InlineData("movie was released in 2020", "2020", "100", 15)]
    [InlineData("movie was released after 2020", "2021", "100", 15)]
    [InlineData("movie metascore is 0", "2021", "0", 0)]
    [InlineData("movie metascore is 50", "2021", "50", 7.5)]
    [InlineData("movie metascore is 75", "2021", "75", 11.25)]
    public async Task Test6(string _, string movieYear, string metascore, decimal expectedPrice)
    {
        // given The movie being added has released year and metascore
        _getMovieApiResponse = _getMovieApiResponse with { Year = movieYear, Metascore = metascore};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // then I expect the response to be OK
        response.ShouldBeOk();

        // then I expect the line item price for the movie to be
        var order = await response.ReadContentAsAsync<TestOrderViewModel>();
        order.Items.Single().Price.Should().Be(expectedPrice);
    }

    [Fact(DisplayName = "Add order item should calculate large order discount correctly (order of 100$ or more)")]
    public async Task Test7()
    {
        // given I set the request to add an order item with a high quantity 
        _request.Quantity = 19;

        // given I have a movie that is very expensive
        _getMovieApiResponse = _getMovieApiResponse with {Metascore = "100", Released = "3000"};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // then I expect the order to be more than $100
        var order = await response.ReadContentAsAsync<TestOrderViewModel>();
        order.LineItemTotal.Should().BeGreaterThan(100M);

        // then I expect the order discount to be 10% of the line item total
        order.DiscountTotal.Should().Be(Math.Round(order.LineItemTotal * .1M, 2));

        // then I expect the discount to be exist
        order.Discounts.Should().BeEquivalentTo(new List<TestOrderDiscountViewModel>()
        {
            new() { Type = "LargeOrder", PercentDiscount = .1M}
        });
    }

    [Fact(DisplayName = "Add order item should calculate senior citizen order discount correctly (order for employee 65 or older)")]
    public async Task Test8()
    {
        // given I have an employee who is older than 65 (calculated from order dates year only)
        _orderDataModel = _orderDataModel! with { Customer = _orderDataModel.Customer with { DateOfBirth = new DateOnly(_orderDataModel.CreatedDateTimeUtc.Year - 65, 1, 1)}};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        var order = await response.ReadContentAsAsync<TestOrderViewModel>();

        // then I expect the order discount to be
        order.DiscountTotal.Should().Be(Math.Round(order.LineItemTotal * .15M, 2));

        // then I expect the discount to be exist
        order.Discounts.Should().BeEquivalentTo(new List<TestOrderDiscountViewModel>()
        {
            new() { Type = "SeniorCitizen", PercentDiscount = .15M}
        });
    }

    [Fact(DisplayName = "Add order item should calculate loyal employee discount correctly (order for employee that has 15 years seniority or more)")]
    public async Task Test9()
    {
        // given I have an employee who is a loyal employee (calculated from order dates year only)
        _orderDataModel = _orderDataModel! with { Customer = _orderDataModel.Customer with { DateHired = new DateOnly(_orderDataModel.CreatedDateTimeUtc.Year - 15, 1, 1)}};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        var order = await response.ReadContentAsAsync<TestOrderViewModel>();

        // then I expect the order discount to be
        order.DiscountTotal.Should().Be(Math.Round(order.LineItemTotal * .25M, 2));

        // then I expect the discount to be exist
        order.Discounts.Should().BeEquivalentTo(new List<TestOrderDiscountViewModel>()
        {
            new() { Type = "LoyalEmployee", PercentDiscount = .25M}
        });
    }

    [Fact(DisplayName = "Add order item should calculate discount correctly when order qualifies for senior citizen and loyal employee discount")]
    public async Task Test10()
    {
        // given I have an employee who is older than 65 (calculated from order dates year only)
        _orderDataModel = _orderDataModel! with { Customer = _orderDataModel.Customer with { DateOfBirth = new DateOnly(_orderDataModel.CreatedDateTimeUtc.Year - 65, 1, 1)}};

        // given I have an employee who is a loyal employee (calculated from order dates year only)
        _orderDataModel = _orderDataModel! with { Customer = _orderDataModel.Customer with { DateHired = new DateOnly(_orderDataModel.CreatedDateTimeUtc.Year - 15, 1, 1)}};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        var order = await response.ReadContentAsAsync<TestOrderViewModel>();

        // then I expect the order discount to be
        order.DiscountTotal.Should().Be(Math.Round(order.LineItemTotal * .40M, 2));

        // then I expect the discount to be exist
        order.Discounts.Should().BeEquivalentTo(new List<TestOrderDiscountViewModel>()
        {
            new() { Type = "LoyalEmployee", PercentDiscount = .25M},
            new() { Type = "SeniorCitizen", PercentDiscount = .15M}
        });
    }

    [Fact(DisplayName = "Add order item should not include large order discount when order qualifies for senior citizen discount and large order discount")]
    public async Task Test11()
    {
        // given I have an employee who is older than 65 (calculated from order dates year only)
        _orderDataModel = _orderDataModel! with { Customer = _orderDataModel.Customer with { DateOfBirth = new DateOnly(_orderDataModel.CreatedDateTimeUtc.Year - 65, 1, 1)}};

        // given I set the request to add an order item with a high quantity 
        _request.Quantity = 19;

        // given I have a movie that is very expensive
        _getMovieApiResponse = _getMovieApiResponse with {Metascore = "100", Released = "3000"};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // then I expect the order to be more than $100
        var order = await response.ReadContentAsAsync<TestOrderViewModel>();
        order.LineItemTotal.Should().BeGreaterThan(100M);


        // then I expect only the senior citizen discount
        order.DiscountTotal.Should().Be(Math.Round(order.LineItemTotal * .15M, 2));
        order.Discounts.Should().BeEquivalentTo(new List<TestOrderDiscountViewModel>()
        {
            new() { Type = "SeniorCitizen", PercentDiscount = .15M}
        });
    }

    [Fact(DisplayName = "Add order item should calculate totals correctly for order with multiple items")]
    public async Task Test12()
    {
        // given I have an existing item worth $15 with quantity of 10
        _orderDataModel = _orderDataModel! with 
        { 
            Items = new List<OrderItemDataModel>() 
            { 
                new()
                {
                    MovieId = "Movie1",
                    Price = 15,
                    Quantity = 10  
                }
            }
        };

        // given my request quantity is 5
        _request.Quantity = 5;

        // given the movie year is 2000 and the metascore is 100
        _getMovieApiResponse = _getMovieApiResponse with { Metascore = "100", Year = "2000"};

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // then I expect the order line item total to be the following
        var order = await response.ReadContentAsAsync<TestOrderViewModel>();
        order.LineItemTotal.Should().Be(15*10 + 12 * 5);
    }

    [Fact(DisplayName = "Add order item should merge quantities with existing order item when added movie that already exists on order")]
    public async Task Test13()
    {
        // given I have a movie in the order with the same id as the movie I will be adding
        _orderDataModel = _orderDataModel! with 
        { 
            Items = new List<OrderItemDataModel>() 
            { 
                new()
                {
                    MovieId = _request.MovieId,
                    MovieYear = "2017",
                    MovieMetascore = "81",
                    Price = 15,
                    Quantity = 10  
                }
            }
        };

        // given my request quantity is 5
        _request.Quantity = 5;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // then I expect the order to contain a single line item with the following quantity
        var order = await response.ReadContentAsAsync<TestOrderViewModel>();
        order.Items.Should().BeEquivalentTo(new List<TestOrderItemViewModel>()
        {
            new()
            {
                MovieId =  _request.MovieId,
                MovieYear = "2017",
                MovieMetascore = "81",
                Quantity = 15,
                Price = 15M
            }
        });
    }


    [Fact(DisplayName = "Add order item should store new order in the database correctly and return the correct order data")]
    public async Task Test90()
    {
        // given I have a high quantity to trigger a discount
        _request.Quantity = 20;

        // when I add an order item
        var response = await AddOrderItemAsync(_orderId, _request);

        // then I expect the response to be created and contain the following
        await response.ShouldBeOkWithResponseAsync(new TestOrderViewModel
        {
            Id = Defaults.OrderId,
            Status = "New",
            Type = "Shipped",
            CreatedDateTimeUtc = Defaults.DateCreated,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = default,
            Shipping = 5M,
            LineItemTotal = 243M,
            DiscountTotal = 24.30M,
            TotalCost = 223.70M,
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
                    MovieId = Defaults.MovieId,
                    MovieYear = "2017",
                    MovieMetascore = "81",
                    Quantity = 20,
                    Price = 12.15M
                }
            },
            Discounts = new()
            {
                new()
                {
                    PercentDiscount = .1M,
                    Type = "LargeOrder"
                }           
            }   
        });
    
        // then I expect the database to have been called with the following
        _liteDbProviderMock
            .Verify(a=> a.UpdateOrderAsync(ItShould.Be(new OrderDataModel()
            {
                Id = Defaults.OrderId,
                Status = Domain.Models.OrderStatus.New,
                Type = Domain.Models.OrderType.Shipped,
                CreatedDateTimeUtc = Defaults.DateCreated,
                CancelledDateTimeUtc = default,
                CompletedDateTimeUtc = default,
                Shipping = 5M,
                LineItemTotal = 243M,
                DiscountTotal = 24.30M,
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
                        MovieId = Defaults.MovieId,
                        MovieYear = "2017",
                        MovieMetascore = "81",
                        Quantity = 20,
                        Price = 12.15M
                    }
                } ,
                Discounts = new()
                {
                    new()
                    {
                        PercentDiscount = .1M,
                        Type = Domain.Models.DiscountType.LargeOrder
                    }           
                }          
            })), Times.Once());
    }


    [Fact(DisplayName = "Add order item should return internal server error when update fails")]
    public async Task Test100()
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