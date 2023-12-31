using System.ComponentModel;

namespace SimpleOrderingSystem.Domain.Models;

public enum SimpleOrderingSystemErrorType
{
    [Description("There is something wrong with your request. Please see details for more information.")]
    InvalidRequest,
    [Description("You need to widen your search because the supplied search parameter was too broad.")]
    SearchMovieRequestTooBroad,
    [Description("Invalid movie id was supplied. Please provide a valid imdb movie id (i.e. like 'tt3896198'). See https://developer.imdb.com/documentation/key-concepts#imdb-ids for more information.")]
    MovieIdInvalid,
    [Description("You must provide a shipping address.")]
    ShippingAddressRequired,
    [Description("The provided shipping address is invalid. See details for more information.")]
    ShippingAddressInvalid,
    [Description("The supplied customer id is invalid")]
    CustomerIdInvalid,
    [Description("The supplied order id is invalid")]
    OrderIdInvalid

}
