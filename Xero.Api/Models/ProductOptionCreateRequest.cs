using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Xero.Api.Models;

public class ProductOptionCreateRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid ProductId { get; set; }

    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
}
