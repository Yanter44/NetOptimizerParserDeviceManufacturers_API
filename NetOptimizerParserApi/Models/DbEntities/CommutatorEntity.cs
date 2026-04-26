using NetOptimizerParserApi.Models.Components;
using NetOptimizerParserApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace NetOptimizerParserApi.Models.DbEntities
{
    public class CommutatorEntity
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Vendor { get; set; }
        public string Model { get; set; } 
        public int Layer { get; set; }      
        public bool IsManaged { get; set; } 
        public List<Port> Ports { get; set; } = new();
        public PoeSpecs PoeSpecs { get; set; }
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; }
        public SwitchProtocolSupport ProtocolSupport { get; set; }
        public SwitchRoleType SwitchRoleType { get; set; }
        public decimal AveragePrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
