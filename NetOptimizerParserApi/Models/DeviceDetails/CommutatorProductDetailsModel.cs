using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models.Components;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Models.DeviceDetails
{
    public class CommutatorProductDetailsModel : ISpecificationProvider
    {
        public bool IsManaged { get; set; }
        public int Layer { get; set; }
        public List<Port> Ports { get; set; } = new List<Port>();
        public PoeSpecs PoeSpecs { get; set; } = new();
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; } = new();
        public SwitchProtocolSupport ProtocolSupport { get; set; } = new();
        public SwitchRoleType SwitchRoleType { get; set; }
        public Dictionary<string, string> GetSpecificationsForAi()
        {
            var portsSummary = Ports != null && Ports.Any()
                ? string.Join(", ", Ports.GroupBy(p => new { p.Speed, p.Type })
                    .Select(g => $"{g.Sum(p => p.Count)} x {g.Key.Speed} {g.Key.Type}"))
                : "Данные о портах отсутствуют";

            return new Dictionary<string, string>
            {
                { "Тип:", IsManaged ? $"Управляемый (L{Layer})" : "Неуправляемый" },
                { "Уровень коммутации:", $"L{Layer}" },
                { "Интерфейсы:", portsSummary },
                { "Пропускная способность:", $"{PerformanceSpecs.ThroughputGbps} Gbps" },
                { "Таблица MAC-адресов:", $"{PerformanceSpecs.MacTableSize} записей" },
                { "Макс. количество VLAN:", PerformanceSpecs.MaxVlans.ToString() },
                { "Поддержка PoE:", PoeSpecs.SupportsPoe ? $"Да (Бюджет: {PoeSpecs.PoeBudgetW}W)" : "Нет" }
            };
        }

    }
}
