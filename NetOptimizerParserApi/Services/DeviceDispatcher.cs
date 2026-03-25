using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Services
{
    public class DeviceDispatcher
    {
        private readonly IEnumerable<IDeviceSaveStrategy> _processors;
        public DeviceDispatcher(IEnumerable<IDeviceSaveStrategy> processors)
        {
            _processors = processors;
        }
        public IDeviceSaveStrategy GetProcessor(ParseDevice deviceType)
        {
            return _processors.FirstOrDefault(p => p.DeviceType == deviceType)
                   ?? throw new Exception($"Обработчик для типа устройства {deviceType} не найден");
        }
    }
}
