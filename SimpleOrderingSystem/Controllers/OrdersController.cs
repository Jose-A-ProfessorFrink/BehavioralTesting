using Microsoft.AspNetCore.Mvc;
using SimpleOrderingSystem.ViewModels;
using SimpleOrderingSystem.Domain;
using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Controllers;


[Route("[controller]")]
[ApiController]
public class OrdersController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Testinng
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}")]
    [HttpGet]
    public async Task<ActionResult<OrderViewModel>> GetOrder(string orderId)
    {
        var result = await _orderService.GetOrderAsync(orderId);

        if (result is null)
        {
            return new NotFoundResult();
        }

        return Map(result);
    }

    /// <summary>
    /// Creates an order for a given employee
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult<OrderViewModel>> CreateOrder(CreateOrderRequestViewModel request)
    {
        var result = await _orderService.CreateOrderAsync(new CreateOrderRequest
        {
            CustomerId = request.CustomerId!,
            Type = request.Type,
            ShippingAddress = request.ShippingAddress is null? null: new()
            {
                Line1 = request.ShippingAddress.Line1,
                Line2 = request.ShippingAddress.Line2,
                ZipCode = request.ShippingAddress.ZipCode,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State
            }
        });

        return Map(result);
    }

    #region Helpers

    private OrderViewModel Map(Order order)
    {
        return new()
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            Type = order.Type.ToString(),
            CreatedDateTimeUtc = order.CreatedDateTimeUtc,
            CancelledDateTimeUtc = order.CancelledDateTimeUtc,
            CompletedDateTimeUtc = order.CompletedDateTimeUtc,
            Shipping = order.Shipping,
            TotalCost = order.TotalCost,
            Customer = Map(order.Customer),
            ShippingAddress = order.ShippingAddress is null? default: Map(order.ShippingAddress),
        };
    }

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
    }

    private AddressViewModel Map(Address address)
    {
        return new()
        {
            Line1 = address.Line1,
            Line2 = address.Line2, 
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode
        };
    }

    #endregion
}
