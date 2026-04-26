using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models.Components
{
    [Owned]
    public class RouterPerformanceSpecs
    {
        public double RoutingThroughputGbps { get; set; }
        public int RamMb { get; set; }
        public int FlashMb { get; set; }
    }
}
