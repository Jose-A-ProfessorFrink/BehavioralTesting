namespace SimpleOrderingSystem.Domain.Models;

public class SimpleOrderingSystemOptions
{
    public string? ZipwiseApiKey {get; set; }
    public string? ZipwiseApiUrl { get; set; }
    public string? OmdbApiKey { get; set; }
    public string? OmdbApiUrl { get; set; }
    public string? LiteDbConnectionString { get; set; }
}
