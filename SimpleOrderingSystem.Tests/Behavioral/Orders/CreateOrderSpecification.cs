using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Domain.Providers;
using System.Diagnostics.CodeAnalysis;


namespace SimpleOrderingSystem.Tests.Behavioral.Orders;

public class CreateOrderSpecification : IClassFixture<WebApplicationFactoryFixture>
{
    // sut
    private readonly HttpClient _httpClient;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IGuidProvider> _guidProviderMock;
    private readonly Mock<IZipCodeProvider> _zipCodeProviderMock;
    
    // default return objects bound to mocks
    private CustomerDataModel? _customerDataModel = new()
    {
        Id = Defaults.CustomerId,
        Name = Defaults.CustomerName,
        DateOfBirth = Defaults.CustomerDateOfBirth,
        DateHired = Defaults.CustomerDateHired,
        AnnualSalary = Defaults.CustomerAnnualSalary
    };

    private OrderDataModel _orderDataModel = 
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
            }
        };

    private GetZipCodeApiResponse _getZipCodeApiResponse = new()
    {
        Results = new()
        {
            Error = default,
            Zip = Defaults.ZipCode,
            State = "CA",
            Cities = new()
            {
                new() { City = "Beverly Hills"}
            }
        }
    };

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

    public CreateOrderSpecification(WebApplicationFactoryFixture webApplicationFactory)
    {
        // given I have a web application factory
        webApplicationFactory.Setup();

        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = webApplicationFactory.Mock<ILiteDbProvider>()
            .SetupGetOrderAsync(() => _orderDataModel);
        _liteDbProviderMock
            .Setup(a=>a.GetCustomerAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => _customerDataModel);

        // given I mock out the zipcodeprovider and setup valid defaults
        _zipCodeProviderMock = webApplicationFactory.Mock<IZipCodeProvider>();
        _zipCodeProviderMock
            .Setup(a=> a.GetZipCodeAsync(It.IsAny<string>(),It.IsAny<string>()))
            .ReturnsAsync(() => _getZipCodeApiResponse);

        // given I mock out the datetime provider and setup valid defaults
        _dateTimeProviderMock = webApplicationFactory.Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(a=> a.UtcNow())
            .Returns(() => Defaults.UtcNow);

        // given I mock out the guid provider and setup valid defaults
        _guidProviderMock = webApplicationFactory.Mock<IGuidProvider>();
        _guidProviderMock
            .Setup(a=> a.NewGuid())
            .Returns(() => Defaults.OrderId);

        // given I have an HttpClient
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact(DisplayName = "Create order should return bad request when required fields are not supplied")]
    public async Task Test1()
    {
        // given all required fields are empty
        _request.CustomerId = default;
        _request.Type = default;
        _request.ShippingAddress = new TestAddressViewModel();

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("customerId", "The CustomerId field is required."),
            ("type", "The Type field is required."),
            ("shippingAddress.Line1", "The Line1 field is required."),
            ("shippingAddress.City", "The City field is required."),
            ("shippingAddress.State", "The State field is required."),
            ("shippingAddress.ZipCode", "The ZipCode field is required.")
            );
    }

    [Fact(DisplayName = "Create order should return bad request when the shipping address state is invalid")]
    public async Task Test2()
    {
        // given I have the following state
        _request.ShippingAddress!.State = "Alabama";

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("shippingAddress.State", "Invalid state supplied. Only states supported values are AL, CA, OR and WA.")
            );
    }


    [Theory(DisplayName = "Create order should return bad request when zip code is ")]
    [InlineData("Too long", "900210-5144")]
    [InlineData("Too short", "900")]
    public async Task Test3(string _, string zipCode)//we can ignore this warning since we are using the parameter as the test name
    {
        // given the zip code is the following
        _request.ShippingAddress!.ZipCode = zipCode;

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("shippingAddress.ZipCode", "Zip code should contain 5 digits")
            );
    }


    [Fact(DisplayName = "Create order should return bad request with appropriate error code when customer id is invalid")]
    public async Task Test4()
    {
        // given the customerid is invalid
        _request.CustomerId = "not a guid";

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "CustomerIdInvalid",
            "The supplied customer id is invalid");
    }

    [Fact(DisplayName = "Create order should return bad request with appropriate error code when customer is not found")]
    public async Task Test5()
    {
        // given the customer is not found
        _customerDataModel = default;

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "CustomerIdInvalid",
            "The supplied customer id is invalid");
    }

    [Fact(DisplayName = "Create order should return bad request with appropriate error code when zip code is invalid")]
    public async Task Test6()
    {
        // given the zip code is invalid (i.e. not returned from our service)
        _getZipCodeApiResponse.Results.Error = "is not a valid ZIP code.";

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "ShippingAddressInvalid",
            "The provided shipping address is invalid. See details for more information.",
            "The zip code provided is invalid");
    }

    [Fact(DisplayName = "Create order should return bad request with appropriate error code when zip code is invalid for the given state")]
    public async Task Test7()
    {
        // given the state does not match the zip code
        _getZipCodeApiResponse = _getZipCodeApiResponse with { Results = _getZipCodeApiResponse.Results with { State = "AL"}};

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "ShippingAddressInvalid",
            "The provided shipping address is invalid. See details for more information.",
            "The state code provided does not correspond to the supplied zip code.");
    }

    [Fact(DisplayName = "Create order should return bad request with appropriate error code when zip code is invalid for the given city")]
    public async Task Test8()
    {
        // given the state does not match the zip code
        _getZipCodeApiResponse = _getZipCodeApiResponse with 
        { 
            Results = _getZipCodeApiResponse.Results  with 
            { 
                Cities = new List<GetZipCodeCityResult>(){ new() { City = "something"} }
            }
        };

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "ShippingAddressInvalid",
            "The provided shipping address is invalid. See details for more information.",
            "The city provided does not correspond to the supplied zip code");
    }

    [Fact(DisplayName = "Create order should store new order in the database correctly and return the correct order data")]
    public async Task Test9()
    {
        // when I create an order
        var response = await CreateOrderAsync(_request);

        // then I expect the response to be created and contain the following
        await response.ShouldBeCreatedWithResponseAsync(new TestOrderViewModel
        {
            Id = Defaults.OrderId,
            Status = "New",
            Type = "Shipped",
            CreatedDateTimeUtc = Defaults.UtcNow,
            CancelledDateTimeUtc = default,
            CompletedDateTimeUtc = default,
            Shipping = 0M,
            TotalCost = 0M,
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
            } 
        });
    
        // then I expect the database to have been called with the following
        _liteDbProviderMock
            .Verify(a=> a.CreateOrderAsync(ItShould.Be(new OrderDataModel()
            {
                Id = Defaults.OrderId,
                Status = Domain.Models.OrderStatus.New,
                Type = Domain.Models.OrderType.Shipped,
                CreatedDateTimeUtc = Defaults.UtcNow,
                CancelledDateTimeUtc = default,
                CompletedDateTimeUtc = default,
                Shipping = default,
                LineItemTotal = default,
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
                }
            })), Times.Once());
    }

    [Fact(DisplayName = "Create order should return internal server error when unknown error is received from zip code service")]
    public async Task Test10()
    {
        // given the zip code service returns something unknown
        _getZipCodeApiResponse.Results.Error = "unhandled exception";

        // when I create an order
        var response = await CreateOrderAsync(_request);

        // the I expect the response to be the following error
        response.ShouldBeInternalServerError();
    }

    #region Helpers

    public async Task<HttpResponseMessage> CreateOrderAsync(TestCreateOrderRequestViewModel request)
    {
        return await _httpClient.PostAsJsonAsync($"Orders", request);
    }

    #endregion
}