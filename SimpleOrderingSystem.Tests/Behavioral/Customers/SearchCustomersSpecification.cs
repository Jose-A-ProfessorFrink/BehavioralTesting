
using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;

namespace SimpleOrderingSystem.Tests.Behavioral.Customers;

public class SearchCustomersSpecification : IClassFixture<WebApplicationFactoryFixture>
{
    // sut
    private readonly HttpClient _httpClient;

    // mocks
    private readonly Mock<ILiteDbProvider> _liteDbProviderMock;
    
    // default return objects bound to mocks
    private List<CustomerDataModel> _customerSearchResults = new List<CustomerDataModel>()
    { 
        new()
        {
            Id = Defaults.CustomerId,
            Name = Defaults.CustomerName,
            DateOfBirth = Defaults.CustomerDateOfBirth,
            DateHired = Defaults.CustomerDateHired,
            AnnualSalary = Defaults.CustomerAnnualSalary
        },
        new()
        {
            Id = Guid.Parse("1838df91-a794-4ddd-8015-172574ad7782"),
            Name = "Bob Marley",
            DateOfBirth = new DateOnly(1965,1,15),
            DateHired = new DateOnly(1990,1,15),
            AnnualSalary = 3450000M
        }
    };

    // custom application settings
    private Dictionary<string,string?> _appSettings = new()
    {
        {"LiteDbConnectionString", "Blah"}
    };

    public SearchCustomersSpecification(WebApplicationFactoryFixture webApplicationFactory)
    {
        // given I mock out the lite db provider and setup appropriate defaults
        _liteDbProviderMock = webApplicationFactory.Mock<ILiteDbProvider>();
        _liteDbProviderMock
            .Setup(a=>a.SearchCustomersAsync(It.IsAny<string>()))
            .ReturnsAsync(() => _customerSearchResults);

        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact(DisplayName = "Search customer should return bad request when search is less than 2 characters")]
    public async Task Test1()
    {
        // when I get a customer
        var response = await SearchCustomerAsync("a");

        // the I expect the response to be not found
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("name", "The name search must be at least 2 characters long."));
    }

    [Fact(DisplayName = "Search customer should return bad request when search is more than 100 characters")]
    public async Task Test2()
    {
        // when I get a customer
        var response = await SearchCustomerAsync("a".Repeat(101));

        // the I expect the response to be not found
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("name", "The name search must not exceed 100 characters in length."));
    }

    [Fact(DisplayName = "Search customer should return OK with no results when database search yields no results")]
    public async Task Test2_1()
    {
        // given I setup the database search to return no results
        _customerSearchResults.Clear();
        // when I get a customer
        var response = await SearchCustomerAsync("bob");

                // then I expect the following 
        await response.ShouldBeOkWithResponseAsync(new TestCustomerSearchResponseViewModel
        {

        });
    }
   
    [Fact(DisplayName = "Search customer should return correct customer data from database")]
    public async Task Test3()
    {
        // when I get a customer
        var response = await SearchCustomerAsync("Bob");

        // then I expect the following 
        await response.ShouldBeOkWithResponseAsync(new TestCustomerSearchResponseViewModel
        {
            Customers = new()
            {
                new()
                {
                    Id = Defaults.CustomerId,
                    Name = Defaults.CustomerName,
                    DateOfBirth = Defaults.CustomerDateOfBirth,
                    DateHired = Defaults.CustomerDateHired,
                    AnnualSalary = Defaults.CustomerAnnualSalary
                },
                new()
                {
                    Id = Guid.Parse("1838df91-a794-4ddd-8015-172574ad7782"),
                    Name = "Bob Marley",
                    DateOfBirth = new DateOnly(1965,1,15),
                    DateHired = new DateOnly(1990,1,15),
                    AnnualSalary = 3450000M
                }
            }
        });

        // then I expect the provider to have been called with the following customer search name
        _liteDbProviderMock.Verify(a=>a.SearchCustomersAsync("Bob"), Times.Once());
    }

    [Fact(DisplayName = "Search customer should return no more than 20 customers for a given search")]
    public async Task Test4()
    {
        // given I setup the data provider to return more than 20 results
        for(int i = 0; i < 21;i++)
        {
            _customerSearchResults.Add(_customerSearchResults.First());
        }

        // when I get a customer
        var response = await SearchCustomerAsync("Bob");

        // then the response should be OK
        response.ShouldBeOk();

        // then I should get no more than 20 results back
        var results = await response.ReadContentAsAsync<TestCustomerSearchResponseViewModel>();
        results.Customers.Count.Should().Be(20);
    }

    #region Helpers

    public async Task<HttpResponseMessage> SearchCustomerAsync(string name)
    {
        return await this._httpClient.GetAsync($"Customers/search?name={name}");
    }

    #endregion
}