using Microsoft.AspNetCore.Mvc;
using SimpleOrderingSystem.ViewModels;
using SimpleOrderingSystem.Domain;
using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Controllers;


[Route("[controller]")]
[ApiController]
public class OrdersController: ControllerBase
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
    /// Search for orders
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("search")]
    [HttpGet]
    public async Task<ActionResult<List<OrderViewModel>>> SearchOrders([FromQuery] OrderSearchRequestViewModel request)
    {
        var results = await _orderService.SearchOrdersAsync(
            new OrderSearchRequest
            {
                CustomerId = request.CustomerId,
                NoOlderThan = request.NoOlderThan
            });

        return results.Select(a=> Map(a)).ToList();
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

        return CreatedAtAction(
            nameof(GetOrder), 
            new { OrderId = result.Id.ToString()}, 
            Map(result));
    }

    /// <summary>
    /// Adds items to an existing order. Order must be in a New status.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}/items")]
    [HttpPost]
    public async Task<ActionResult<OrderViewModel>> AddOrderItem(string orderId, AddOrderItemRequestViewModel request)
    {
        return Map(await _orderService.AddOrderItemAsync(new()
        {
            OrderId = orderId,
            MovieId = request.MovieId,
            Quantity = request.Quantity.GetValueOrDefault(1)
        }));
    }

    /// <summary>
    /// Cancel an existing order. Order must be in a New status.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}")]
    [HttpDelete]
    public async Task<ActionResult<OrderViewModel>> CancelOrder(string orderId)
    {
        return Map(await _orderService.CancelOrderAsync(orderId));
    }

    /// <summary>
    /// Complete an existing order. Order must be in a New status.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}/complete")]
    [HttpPost]
    public async Task<ActionResult<OrderViewModel>> CompleteOrder(string orderId)
    {
        return Map(await _orderService.CompleteOrderAsync(orderId));
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
            Items = order.Items.Select(a => Map(a)).ToList(),
            Discounts = order.Discounts.Select(a => Map(a)).ToList(),
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

    private OrderDiscountViewModel Map(OrderDiscount orderDiscount)
    {
        return new()
        {
            Type = orderDiscount.Type,
            PercentDiscount = orderDiscount.PercentDiscount
        };
    }

    private OrderItemViewModel Map(OrderItem orderItem)
    {
        return new()
        {
            MovieId = orderItem.MovieId,
            MovieYear = orderItem.MovieYear,
            MovieMetascore = orderItem.MovieMetascore,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity
        };
    }
    #endregion
}
