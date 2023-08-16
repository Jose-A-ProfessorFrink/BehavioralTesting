using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Providers;
using SimpleOrderingSystem.Domain.Repositories;


namespace SimpleOrderingSystem.Domain.Services;

internal class OrderService:IOrderService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly ICustomersRepository _customersRepository;
    private readonly IZipCodeRepository _zipCodeRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrderService(
        IOrdersRepository ordersRepository, 
        ICustomersRepository customersRepository,
        IZipCodeRepository zipCodeRepository,
        IGuidProvider guidProvider, 
        IDateTimeProvider dateTimeProvider)
    {
        _ordersRepository = ordersRepository;
        _customersRepository = customersRepository;
        _zipCodeRepository = zipCodeRepository;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        Customer? customer = default;
        if(!Guid.TryParse(request.CustomerId, out var customerId) || 
            ((customer = await _customersRepository.GetCustomerAsync(customerId)) is null))
        {
            throw new SimpleOrderingSystemException(
                SimpleOrderingSystemErrorType.CustomerIdInvalid);
        }

        await ValidateShippingAddressAsync(request);

        var newOrder = new Order()
        {
            Id = _guidProvider.NewGuid(),
            Status = OrderStatus.New,
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
            TotalCost = 0M      
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

        if(noOlderThan.HasValue)
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
                    "The state code provided is invalid or does not correspond to the supplied zip code.");                
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