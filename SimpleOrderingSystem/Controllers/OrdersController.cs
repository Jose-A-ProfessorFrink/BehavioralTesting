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

        };
    }

    #endregion
}


/*
    /// <summary>
    /// Creates an order for a given employee
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult<OrderResponseViewmodel>> CreateOrder(CreateOrderRequestViewmodel request)
    {
        var result = await _orderService.CreateOrderAsync(new CreateOrderIntent
        {
            EmployeeName = request.EmployeeName,
            Items = request.Items.Select(a => new CreateOrderItemIntent
            {
                ProductId = a.ProductCode,
                Quantity = a.Quantity
            })
        });

        return MapToOrderResponse(result);
    }

    /// <summary>
    /// Gets an existing order
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}")]
    [HttpGet]
    public async Task<ActionResult<OrderResponseViewmodel>> GetOrder(string orderId)
    {
        var result = await _orderService.GetOrderAsync(orderId);

        if (result == null)
        {
            return new NotFoundResult();
        }

        return new ActionResult<OrderResponseViewmodel>(MapToOrderResponse(result));
    }

    /// <summary>
    /// Deletes an existing order
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}")]
    [HttpDelete]
    public async Task<ActionResult> DeleteOrder(string orderId)
    {
        await _orderService.DeleteOrderAsync(orderId);

        return new OkResult();
    }

    #region Helpers
    private OrderResponseViewmodel MapToOrderResponse(Order order)
    {
        return new OrderResponseViewmodel
        {
            OrderId = order.Id,
            EmployeeName = order.EmployeeName,
            Status = order.Status,
            TotalCost = order.TotalCost(),
            EmployeeId = order.EmployeeId,
            DiscountReceived = order.DiscountType,
            Items = order.Items.Select(a => new OrderItemResponseViewmodel
            {
                Name = a.Name,
                Price = a.Price,
                Code = a.Code,
                ProductId = a.ProductId,
                Quantity = a.Quantity
            }).ToList()
        };
    }
    #endregion

    */


