
namespace Xero.Api.Models;

public class ProductOption
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}
