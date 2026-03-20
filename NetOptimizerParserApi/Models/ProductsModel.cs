using NetOptimizerParserApi.Enums;

namespace NetOptimizerParserApi.Models
{
    public class ProductsModel
    {
        public string ProductModel { get; set; }
        public DeviceType DeviceType { get; set; }
        public decimal AveragePrice { get; set; }
        public object DeviceDetails { get; set; }
    }
}
