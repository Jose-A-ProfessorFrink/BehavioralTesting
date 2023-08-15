using Microsoft.AspNetCore.Mvc;
using SimpleOrderingSystem.Services;
using LiteDB;
using SimpleOrderingSystem.Models;
using SimpleOrderingSystem.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace SimpleOrderingSystem.Controllers;

[Route("[controller]")]
[ApiController]
public class CustomersController
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Retrieves a customer by their id. Returns the customer
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [Route("{customerId}")]
    [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult<CustomerViewModel>> GetCustomer(string customerId)
    {
        var customer = await _customerService.GetCustomerAsync(customerId);

        if (customer is null)
        {
            return new NotFoundResult();
        }

        return Map(customer);
    }

    /// <summary>
    /// Searches for customers based on their name. Returns a maximum of 20 results.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [Route("search")]
    [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult<CustomerSearchResponseViewModel>> SearchCustomer([FromQuery] CustomerSearchRequestViewModel searchRequest)
    {
        var customers = await _customerService.SearchCustomersAsync(searchRequest.Name!);

        return new CustomerSearchResponseViewModel
        {
            Customers = customers.Take(20).Select(a=> Map(a)).ToList()
        };
    }

#region Helpers
    private CustomerViewModel Map(Customer customer)
    {
        return new()
        {
            Id = customer.Id,
            Name = customer.Name,
            DateHired = customer.DateHired,
            DateOfBirth = customer.DateOfBirth,
            AnnualSalary = customer.AnnualSalary
        };
#endregion
    }
}