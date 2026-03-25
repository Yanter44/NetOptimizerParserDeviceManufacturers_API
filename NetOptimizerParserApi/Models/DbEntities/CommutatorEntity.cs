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
        public bool SupportsPoe { get; set; }
        public int PoeBudgetW { get; set; }

        public decimal ThroughputGbps { get; set; }
        public int MacTableSize { get; set; }      
        public int MaxVlans { get; set; }    

        public decimal AveragePrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
