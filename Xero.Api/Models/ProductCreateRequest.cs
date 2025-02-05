using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Xero.Api.Models;

public class ProductCreateRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public decimal DeliveryPrice { get; set; }
}
