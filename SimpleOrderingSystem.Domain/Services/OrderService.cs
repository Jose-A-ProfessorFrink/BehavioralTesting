using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Providers;
using SimpleOrderingSystem.Domain.Repositories;


namespace SimpleOrderingSystem.Domain.Services;

internal class OrderService:IOrderService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly ICustomersRepository _customersRepository;
    private readonly IZipCodeRepository _zipCodeRepository;
    private readonly IMoviesRepository _moviesRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrderService(
        IOrdersRepository ordersRepository, 
        ICustomersRepository customersRepository,
        IZipCodeRepository zipCodeRepository,
        IMoviesRepository moviesRepository,
        IGuidProvider guidProvider, 
        IDateTimeProvider dateTimeProvider)
    {
        _ordersRepository = ordersRepository;
        _customersRepository = customersRepository;
        _zipCodeRepository = zipCodeRepository;
        _moviesRepository = moviesRepository;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        Customer? customer;
        if(!Guid.TryParse(request.CustomerId, out var customerId) || 
            ((customer = await _customersRepository.GetAsync(customerId)) is null))
        {
            throw new SimpleOrderingSystemException(
                SimpleOrderingSystemErrorType.CustomerIdInvalid);
        }

        await ValidateShippingAddressAsync(request);

        var newOrder = new Order()
        {
            Id = _guidProvider.NewGuid(),
            Type = request.Type,
            CreatedDateTimeUtc = _dateTimeProvider.UtcNow(),
            ShippingAddress = request.ShippingAddress is null? default: new Address
            {
                Line1 = request.ShippingAddress.Line1,
                Line2 = request.ShippingAddress.Line2,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State,
                ZipCode = request.ShippingAddress.ZipCode
            },
            Customer = customer,
            Shipping = 0M,
            DiscountTotal = 0M,
            LineItemTotal = 0M      
        };

        await _ordersRepository.CreateOrderAsync(newOrder);
        
        return newOrder;
    }

    public async Task<Order?> GetOrderAsync(string orderId)
    {
        if(Guid.TryParse(orderId, out var orderIdGuid))
        {
            return await _ordersRepository.GetOrderAsync(orderIdGuid);
        }

        return default;
    }

    public async Task<List<Order>> SearchOrdersAsync(OrderSearchRequest request)
    {
        var nowUtc = _dateTimeProvider.UtcNow();
        DateTime? noOlderThan = request.NoOlderThan;

        if(!request.CustomerId.HasValue && noOlderThan.HasValue)
        {
            if(nowUtc.Subtract(noOlderThan.Value).TotalDays> 7D)
            {
                noOlderThan = nowUtc.Subtract(TimeSpan.FromDays(7));
            }
        }

        return await _ordersRepository.SearchOrdersAsync(
            noOlderThan.GetValueOrDefault(nowUtc.Subtract(TimeSpan.FromDays(1))), 
            request.CustomerId);   
    }

    public async Task<Order> CancelOrderAsync(string orderId)
    {
        Order? order;
        if(!Guid.TryParse(orderId, out var orderIdGuid) || 
           (order = await _ordersRepository.GetOrderAsync(orderIdGuid)) is null)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.OrderIdInvalid);
        }

        order.Cancel(_dateTimeProvider.UtcNow());

        await _ordersRepository.UpdateOrderAsync(order);

        return order;
    }

    public async Task<Order> CompleteOrderAsync(string orderId)
    {
        Order? order;
        if(!Guid.TryParse(orderId, out var orderIdGuid) || 
           (order = await _ordersRepository.GetOrderAsync(orderIdGuid)) is null)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.OrderIdInvalid);
        }

        order.Complete(_dateTimeProvider.UtcNow());

        await _ordersRepository.UpdateOrderAsync(order);

        return order;
    }

    public async Task<Order> AddOrderItemAsync(AddOrderItemRequest request)
    {
        Order? order;
        if(!Guid.TryParse(request.OrderId, out var orderIdGuid) || 
           (order = await _ordersRepository.GetOrderAsync(orderIdGuid)) is null)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.OrderIdInvalid);
        }

        if(order.Status != OrderStatus.New)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                $"Cannot add item to order because the order status does not allow adding items."); 
        }

        var movie = await _moviesRepository.GetMovieAsync(request.MovieId);
        if(movie is null)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.MovieIdInvalid);
        }

        order.AddOrderItem(movie, request.Quantity);

        await _ordersRepository.UpdateOrderAsync(order);

        return order;
    }

    public async Task<Order> DeleteOrderItemAsync(DeleteOrderItemRequest request)
    {
        Order? order;
        if(!Guid.TryParse(request.OrderId, out var orderIdGuid) || 
           (order = await _ordersRepository.GetOrderAsync(orderIdGuid)) is null)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.OrderIdInvalid);
        }

        if(order.Status != OrderStatus.New)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                $"Cannot add item to order because the order status does not allow adding items."); 
        }

        order.DeleteOrderItem(request.MovieId);

        await _ordersRepository.UpdateOrderAsync(order);

        return order;
    }

    #region Helpers

    private async Task ValidateShippingAddressAsync(CreateOrderRequest request)
    {
        if(request.Type == OrderType.Shipped)
        {
            if(request.ShippingAddress is null)
            {
                throw new SimpleOrderingSystemException(
                    SimpleOrderingSystemErrorType.ShippingAddressRequired,        
                    "Shipping address required for shipped orders.");
            }

            var zipCodeInfo = await _zipCodeRepository.GetZipCodeInformationAsync(request.ShippingAddress.ZipCode);
            if(zipCodeInfo is null)
            {
                throw new SimpleOrderingSystemException(
                    SimpleOrderingSystemErrorType.ShippingAddressInvalid,        
                    "The zip code provided is invalid");                
            }

            if(zipCodeInfo.State != request.ShippingAddress.State)
            {
                throw new SimpleOrderingSystemException(
                    SimpleOrderingSystemErrorType.ShippingAddressInvalid,        
                    "The state code provided does not correspond to the supplied zip code.");                
            }

            if(!zipCodeInfo.Cities.Any(a=> a.Equals(request.ShippingAddress.City, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new SimpleOrderingSystemException(
                    SimpleOrderingSystemErrorType.ShippingAddressInvalid,        
                    "The city provided does not correspond to the supplied zip code");                  
            }
        }
    }

    #endregion
}