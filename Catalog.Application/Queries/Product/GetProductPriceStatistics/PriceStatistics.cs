namespace Catalog.Application.Queries.Product.GetProductPriceStatistics;


public class PriceStatistics
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal AveragePrice { get; set; }
}