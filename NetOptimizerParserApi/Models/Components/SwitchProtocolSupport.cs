using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models.Components
{
    [Owned]
    public class SwitchProtocolSupport
    {
        public bool SupportsLag { get; set; }
        public bool SupportsLacp { get; set; }
        public bool SupportsLoopProtection { get; set; }
    }
}
