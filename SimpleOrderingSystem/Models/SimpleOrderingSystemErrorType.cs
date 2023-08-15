namespace SimpleOrderingSystem.Models;

public enum SimpleOrderingSystemErrorType
{
    InvalidCustomerId,
    ProductNotFound,
    OrderRequestInvalid,
    CannotDeleteOrderUnlessItsNew,
    CannotDeleteHighPriceOrder,
    CannotDeleteOrderTooOld,
    InvalidShippingZipCode
}
