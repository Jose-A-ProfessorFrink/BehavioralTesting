namespace SimpleOrderingSystem.Models;

public enum SimpleOrderingSystemErrorType
{
    EmployeeNotFound,
    ProductNotFound,
    OrderRequestInvalid,
    CannotDeleteOrderUnlessItsNew,
    CannotDeleteHighPriceOrder,
    CannotDeleteOrderTooOld,
    InvalidShippingZipCode
}
