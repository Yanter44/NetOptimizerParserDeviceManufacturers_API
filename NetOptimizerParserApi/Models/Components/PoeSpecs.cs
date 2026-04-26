using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models.Components
{
    [Owned]
    public class PoeSpecs
    {
        public bool SupportsPoe { get; set; }
        public int PoeBudgetW { get; set; }
    }
}
