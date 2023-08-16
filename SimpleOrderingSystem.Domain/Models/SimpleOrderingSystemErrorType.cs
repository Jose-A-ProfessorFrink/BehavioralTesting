using System.ComponentModel;

namespace SimpleOrderingSystem.Domain.Models;

public enum SimpleOrderingSystemErrorType
{
    [Description("You need to widen your search because the supplied search parameter was too broad.")]
    SearchMovieRequestTooBroad,
    [Description("Invalid movie id was supplied. Please provide a valid imdb movie id (i.e. like 'tt3896198'). See https://developer.imdb.com/documentation/key-concepts#imdb-ids for more information.")]
    MovieIdInvalid,
    [Description("")]
    OrderIdInvalid
}
