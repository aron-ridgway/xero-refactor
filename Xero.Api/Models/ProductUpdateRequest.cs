using System.Text.Json.Serialization;

namespace Xero.Api.Models;

public class ProductUpdateRequest
{
    [JsonIgnore]
    public Guid Id { get;  set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public decimal? DeliveryPrice { get; set; }
}
