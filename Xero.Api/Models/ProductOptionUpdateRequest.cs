using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Xero.Api.Models;

public class ProductOptionUpdateRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid ProductId { get; set; }

    public string? Name { get; set; }
    
    public string? Description { get; set; }
}
