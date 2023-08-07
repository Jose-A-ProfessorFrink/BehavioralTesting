using Microsoft.AspNetCore.Mvc;
namespace SimpleOrderingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController
{
    //private readonly IOrderService _orderService;

    public OrdersController()//IOrderService orderService)
    {
        //_orderService = orderService;
    }

    /// <summary>
    /// Testinng
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Route("{orderId}")]
    [HttpGet]
    public async Task<ActionResult<string>> GetOrder(string orderId)
    {
        //var result = await _orderService.GetOrderAsync(orderId);

        if (orderId is "100")
        {
            return new NotFoundResult();
        }

        await Task.CompletedTask;

        return $"The order you asked for was {orderId}";
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
}

