using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models
{
    [Owned]
    public class ProtocolSupport
    {
        public bool SupportsOspf { get; set; }
        public bool SupportsVrrp { get; set; }
        public bool SupportsIpsec { get; set; }
        public bool SupportsNat { get; set; }
    }
}
