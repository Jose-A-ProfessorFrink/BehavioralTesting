namespace SimpleOrderingSystem.Domain.Models;

public class Order
{
    public const int MaximumAllowedOrderItems = 20;
    public const decimal LargeOrderDiscountThreshold = 100M;

    private readonly Dictionary<string, OrderItem> _orderItems;
    private readonly List<OrderDiscount> _discounts;
    private OrderStatus _orderStatus;
    private DateTime? _completedDateTimeUtc;
    private DateTime? _cancelledDateTimeUtc;
    private int LineItemsTotalQuantity()  => _orderItems.Values.Sum(a=>a.Quantity);
    private decimal LineItemsTotalPrice()  => _orderItems.Values.Sum(a=> a.Quantity * a.Price);

    public Guid Id { get; init;}
    public OrderStatus Status => _orderStatus;
    public OrderType Type { get; init;}
    public Customer Customer { get; init;} = default!;
    public Address? ShippingAddress { get; init;} = default!;
    public IEnumerable<OrderItem> Items => _orderItems.Values;
    public IEnumerable<OrderDiscount> Discounts => _discounts;
    public decimal Shipping { get; set;}
    public decimal LineItemTotal {get ;set;}
    public decimal DiscountTotal {get;set;}
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc => _cancelledDateTimeUtc;
    public DateTime? CompletedDateTimeUtc => _completedDateTimeUtc;

    public decimal TotalCost => LineItemTotal + Shipping - DiscountTotal;

    public Order()
        :this(Enumerable.Empty<OrderItem>(), Enumerable.Empty<OrderDiscount>(), OrderStatus.New)
    {

    }
    public Order(
        IEnumerable<OrderItem> orderItems, 
        IEnumerable<OrderDiscount> discounts, 
        OrderStatus orderStatus, 
        DateTime? completedDateTimeUtc = default, 
        DateTime? cancelledDateTimeUtc = default)
    {
        _orderItems = orderItems.ToDictionary(a=> a.MovieId, a=> a);
        _discounts = discounts.ToList();
        _orderStatus = orderStatus;
        _completedDateTimeUtc = completedDateTimeUtc;
        _cancelledDateTimeUtc = cancelledDateTimeUtc;
    }

    public void AddOrderItem(Movie movie, int quantity)
    {
        if(quantity <= 0)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                "Order item quantity must be greater than 0.");
        }

        if(LineItemsTotalQuantity() + quantity > MaximumAllowedOrderItems)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                $"Unable to add items because that would exceed the maximum movies for a single order. An order can have up to {MaximumAllowedOrderItems} items.");
        }

        if(_orderItems.TryGetValue(movie.Id, out var orderItem))
        {
            orderItem.Quantity += quantity;
        }
        else
        {
            _orderItems.Add(movie.Id, new OrderItem() 
            { 
                MovieId = movie.Id, 
                Quantity = quantity, 
                MovieYear = movie.Year, 
                MovieMetascore = movie.Metascore,
                Price = CalculateItemPrice(movie.Year, movie.Metascore)
            });
        }

        CalculatePricing();
    }

    public void DeleteOrderItem(string movieId)
    {
        if(_orderItems.Remove(movieId))
        {
            CalculatePricing();
        }
        else
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.MovieIdInvalid);
        }
    }
    
    public void Complete(DateTime dateTimeCompleted)
    {
        if(Status != OrderStatus.New)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                $"Cannot complete order because the order status does not allow completion."); 
        }

        if(!Items.Any())
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                $"Cannot complete order because the order does not contain any items!"); 
        }

        _orderStatus = OrderStatus.Completed;
        _completedDateTimeUtc = dateTimeCompleted;
    }

    public void Cancel(DateTime dateTimeCancelled)
    {
        if(Status != OrderStatus.New)
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                $"Cannot cancel order because the order status does not allow cancellation."); 
        }

        _orderStatus = OrderStatus.Cancelled;
        _cancelledDateTimeUtc = dateTimeCancelled;    
    }
    
    #region  Helpers

    private decimal CalculateItemPrice(string? movieYear, string? movieMetascore)
    {
        // set the price to the base price
        decimal price = 5M;

        if( int.TryParse(movieYear, out var year) && 
            decimal.TryParse(movieMetascore, out var metascore))
        {
            switch(year)
            {
                case var _ when year <= 1945:
                    price = 2M;
                    break;
                case var _ when year <= 1970:
                    price = 6M;
                    break;  
                case var _ when year <= 2000:
                    price = 12M;
                    break;   
                case var _ when year <= 2020:
                default:
                    price = 15M;
                    break;                  
            }

            return Math.Round(price * metascore/100M, 2);
        }

        return price;
    }

    private void CalculatePricing()
    {
        _discounts.Clear();
        _discounts.AddRange(CalculateDiscounts());

        Shipping = Type == OrderType.Shipped && _orderItems.Any()? 5M: 0M;
        LineItemTotal = LineItemsTotalPrice();
        DiscountTotal = Math.Round(Discounts.Sum(a=> a.PercentDiscount) * LineItemTotal, 2);
    }
    private List<OrderDiscount> CalculateDiscounts()
    {
        var discounts = new List<OrderDiscount>();

        if(CreatedDateTimeUtc.Date.Year - Customer.DateHired.Year >= 15)
        {
            discounts.Add(new OrderDiscount { Type = DiscountType.LoyalEmployee, PercentDiscount = .25M});
        }

        if(CreatedDateTimeUtc.Date.Year - Customer.DateOfBirth.Year >= 65)
        {
            discounts.Add(new OrderDiscount { Type = DiscountType.SeniorCitizen, PercentDiscount = .15M});
        }
        else if(LineItemsTotalPrice() >= LargeOrderDiscountThreshold)
        {
            discounts.Add(new OrderDiscount { Type = DiscountType.LargeOrder, PercentDiscount = .1M});            
        }

        return discounts;
    }
    #endregion
}

