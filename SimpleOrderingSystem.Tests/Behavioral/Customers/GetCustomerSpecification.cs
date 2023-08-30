
using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;

namespace SimpleOrderingSystem.Tests.Behavioral.Customers;

public class GetCustomerSpecification : IDisposable
{
    // sut
    private readonly WebApplicationFactory _webApplicationFactory;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    
    // default return objects bound to mocks
    private CustomerDataModel? _customerDataModel = new()
    {
        Id = Defaults.CustomerId,
        Name = Defaults.CustomerName,
        DateOfBirth = Defaults.CustomerDateOfBirth,
        DateHired = Defaults.CustomerDateHired,
        AnnualSalary = Defaults.CustomerAnnualSalary
    };

    // custom application settings
    private Dictionary<string,string> _appSettings = new()
    {
        {"LiteDbConnectionString", "Blah"}
    };

    public GetCustomerSpecification()
    {
        // given I have a web application factory
        _webApplicationFactory = WebApplicationFactory.Create((a) => a.AddInMemoryCollection(_appSettings));

        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = _webApplicationFactory.Mock<ILiteDbProvider>();
        _liteDbProviderMock
            .Setup(a=>a.GetCustomerAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => _customerDataModel);
    }

    [Fact(DisplayName = "Get customer should return not found when invalid customer id(not Guid parseable) is used")]
    public async Task Test1()
    {
        // when I get a customer
        var response = await GetCustomerAsync("Invalid Customer Id (not Guid parseable)");

        // the I expect the response to be not found
        response.ShouldBeNotFound();
    }

    [Fact(DisplayName = "Get customer should return not found when customer does not exist")]
    public async Task Test2()
    {
        // given I setup the customer provider to return null
        _customerDataModel = default;

        // when I get a customer
        var response = await GetCustomerAsync(Defaults.CustomerId.ToString());

        // then I expect the response to be not found
        response.ShouldBeNotFound();

        // then I expect the provider to have been called
        _liteDbProviderMock.Verify(a=>a.GetCustomerAsync(Defaults.CustomerId), Times.Once());
    }

    [Fact(DisplayName = "Get customer should return correct customer data from database")]
    public async Task Test3()
    {
        // when I get a customer
        var response = await GetCustomerAsync(Defaults.CustomerId.ToString());

        // then I expect the following 
        await response.ShouldBeOkWithResponseAsync(new TestCustomerViewModel
        {
            Id = Defaults.CustomerId,
            Name = Defaults.CustomerName,
            DateOfBirth = Defaults.CustomerDateOfBirth,
            DateHired = Defaults.CustomerDateHired,
            AnnualSalary = Defaults.CustomerAnnualSalary
        });

        // then I expect the provider to have been called with the following customer id
        _liteDbProviderMock.Verify(a=>a.GetCustomerAsync(Defaults.CustomerId), Times.Once());
    }

    #region Helpers

    public async Task<HttpResponseMessage> GetCustomerAsync(string customerId)
    {
        return await _webApplicationFactory.CreateClient().GetAsync($"Customers/{customerId}");
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }

    #endregion
}