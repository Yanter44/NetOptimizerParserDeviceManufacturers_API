using Microsoft.EntityFrameworkCore;
using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.DbContext;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.DbEntities;
using NetOptimizerParserApi.Models.DeviceDetails;
using NetOptimizerParserApi.Models.Dto_s;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Services.Business
{
    public class CommutatorService : ICommutatorService, IDeviceSaveStrategy
    {
        private readonly AppDbContext _dbContext;
        public ParseDevice DeviceType => ParseDevice.Switches;

        public CommutatorService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ServiceResponse<bool>> ProcessAndSaveAsync(List<ProductsModel> products, SitesToParse site)
        {
            var parsedSwitchDevices = new List<CommutatorModelRequestDto>();
            foreach (var product in products)
            {
                if (product.DeviceDetails is CommutatorProductDetailsModel switchDetails)
                {
                    var entityDto = new CommutatorModelRequestDto
                    {
                        Model = product.ProductModel,
                        AveragePrice = product.AveragePrice,
                        Vendor = site.ToString(),
                        Layer = switchDetails.Layer,
                        IsManaged = switchDetails.IsManaged,
                        PoeSpecs = switchDetails.PoeSpecs,
                        Ports = switchDetails.Ports,
                        SwitchRoleType = switchDetails.SwitchRoleType,
                        PerformanceSpecs = switchDetails.PerformanceSpecs,
                        ProtocolSupport = switchDetails.ProtocolSupport,
                        TotalPorts = switchDetails.Ports.Sum(x => x.Count)
                    };
                    parsedSwitchDevices.Add(entityDto);
                }
            }
            var addSwitchOperationResult = await AddCommutatorsToDbAsync(parsedSwitchDevices);
            if (addSwitchOperationResult.Success)
            {
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Устройства успешно добавлены"
                };
            }

            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Не удалось добавить устройства"
            };
        }
        public async Task<ServiceResponse<List<CommutatorResponceDto>>> GetAllCommutatorsAsync()
        {
            try
            {
                var result = await _dbContext.CommutatorsTable
                    .AsNoTracking()
                    .Select(model => new CommutatorResponceDto
                    {
                        ExternalId = model.ExternalId,
                        Layer = model.Layer,
                        Model = model.Model,
                        Vendor = model.Vendor,             
                        PoeSpecs = model.PoeSpecs,
                        Ports = model.Ports,
                        IsManaged = model.IsManaged,
                        PerformanceSpecs = model.PerformanceSpecs,
                        SwitchRoleType = model.SwitchRoleType,  
                        AveragePrice = model.AveragePrice,
                        ProtocolSupport = model.ProtocolSupport,
                        TotalPorts = model.Ports.Select(x => x.Count).Sum()
                    }).ToListAsync();

                return new ServiceResponse<List<CommutatorResponceDto>>
                {
                    Data = result,
                    Success = true,
                    Message = result.Any() ? "Данные успешно получены." : "Список коммутаторов пуст."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CommutatorResponceDto>>
                {
                    Success = false,
                    Message = "Ошибка при получении данных из базы."
                };
            }
        }

        public async Task<ServiceResponse<bool>> AddCommutatorToDbAsync(CommutatorModelRequestDto model)
        {
            try
            {
                var exists = await _dbContext.CommutatorsTable
                        .AnyAsync(c => c.Vendor == model.Vendor && c.Model == model.Model);

                if (exists)
                    return new ServiceResponse<bool> { Data = false, Success = true, Message = "Такое устройство уже существует." };

                var entity = new CommutatorEntity
                {
                    ExternalId = Guid.NewGuid(),
                    Layer = model.Layer,
                    Model = model.Model,
                    Vendor = model.Vendor,
                    PoeSpecs = model.PoeSpecs,
                    SwitchRoleType = model.SwitchRoleType,
                    Ports = model.Ports,
                    IsManaged = model.IsManaged,
                    PerformanceSpecs = model.PerformanceSpecs,   
                    ProtocolSupport = model.ProtocolSupport,
                    AveragePrice = model.AveragePrice,
                    UpdatedAt = DateTime.UtcNow
                };
                await _dbContext.CommutatorsTable.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceResponse<bool> { Data = true, Success = true, Message = "Устройство успешно добавлено." };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Критическая ошибка при сохранении в базу." };
            }
        }

        public async Task<ServiceResponse<bool>> AddCommutatorsToDbAsync(List<CommutatorModelRequestDto> models)
        {
            try
            {
                var modelNames = models.Select(m => m.Model).ToList();
                var existingNames = await _dbContext.CommutatorsTable
                    .Where(c => modelNames.Contains(c.Model))
                    .Select(c => new { c.Vendor, c.Model })
                    .ToListAsync();

                var entitiesToAdd = new List<CommutatorEntity>();
                foreach (var model in models)
                {
                    bool exists = existingNames.Any(e => e.Vendor == model.Vendor && e.Model == model.Model);

                    if (!exists)
                    {
                        entitiesToAdd.Add(new CommutatorEntity
                        {
                            ExternalId = Guid.NewGuid(),
                            Layer = model.Layer,
                            Model = model.Model,
                            Vendor = model.Vendor,
                            PoeSpecs = model.PoeSpecs,
                            Ports = model.Ports,
                            IsManaged = model.IsManaged,
                            PerformanceSpecs = model.PerformanceSpecs,
                            SwitchRoleType = model.SwitchRoleType,
                            ProtocolSupport = model.ProtocolSupport,
                            AveragePrice = model.AveragePrice,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
                if (!entitiesToAdd.Any())
                    return new ServiceResponse<bool> { Data = true, Success = true, Message = "Все устройства уже есть в базе." };

                await _dbContext.CommutatorsTable.AddRangeAsync(entitiesToAdd);
                int count = await _dbContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = count > 0,
                    Success = true,
                    Message = $"Успешно добавлено {entitiesToAdd.Count} новых устройств."
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Ошибка при массовом сохранении в БД." };
            }
        }

        public async Task<ServiceResponse<List<CommutatorResponceDto>>> GetCommutatorsByPriceRange(PriceRangeRequestDto priceRangeModel)
        {
            try
            {
                var entities = await _dbContext.CommutatorsTable
                    .AsNoTracking()
                    .Where(x => x.AveragePrice >= priceRangeModel.Min && x.AveragePrice <= priceRangeModel.Max)
                    .ToListAsync();

                var result = entities.Select(model => new CommutatorResponceDto
                {
                    Layer = model.Layer,
                    Model = model.Model,
                    Vendor = model.Vendor,
                    PoeSpecs = model.PoeSpecs,
                    Ports = model.Ports,
                    IsManaged = model.IsManaged,
                    PerformanceSpecs = model.PerformanceSpecs,
                    ProtocolSupport = model.ProtocolSupport,
                    AveragePrice = model.AveragePrice,
                }).ToList();

                return new ServiceResponse<List<CommutatorResponceDto>>
                {
                    Data = result,
                    Success = true,
                    Message = $"Найдено устройств: {result.Count}"
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<List<CommutatorResponceDto>>
                {
                    Success = false,
                    Message = "Ошибка при фильтрации по цене."
                };
            }
        }

        public async Task<ServiceResponse<bool>> RemoveCommutatorFromDbAsync(string ExternalId)
        {
            var GuidFromString = Guid.Parse(ExternalId);
            var existmodel = await _dbContext.CommutatorsTable.Where(x => x.ExternalId == GuidFromString).FirstOrDefaultAsync();
            if(existmodel != null)
            {
                _dbContext.Remove(existmodel);
                await _dbContext.SaveChangesAsync();
                return new ServiceResponse<bool> { Success = true, Message = "Модель была найдена и успешно удалена" };
            }
            return new ServiceResponse<bool> { Success = false, Message = "Не удалось найти модель по заданному Id" };
        }

        public async Task<ServiceResponse<bool>> RemoveCommutatorsFromDbAsync(List<string> ExternalIds)
        {
            var guids = ExternalIds.Select(Guid.Parse).ToList();

            var existModels = await _dbContext.CommutatorsTable
                .Where(x => guids.Contains(x.ExternalId))
                .ToListAsync();

            if (!existModels.Any())
                return new ServiceResponse<bool> { Data = false, Message = "Ничего не найдено" };

            _dbContext.CommutatorsTable.RemoveRange(existModels);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Message = "Успешно удалено" };
        }

        public async Task<ServiceResponse<bool>> UpdateCommutatorAsync(string commutatorExternalId, CommutatorModelRequestDto commutatorModeldto)
        {
            var parsedToGuidExternalId = Guid.Parse(commutatorExternalId);
            var existModel = await _dbContext.CommutatorsTable
                .FirstOrDefaultAsync(x => x.ExternalId == parsedToGuidExternalId);

            if (existModel == null)
                return new ServiceResponse<bool> { Success = false, Message = "Не удалось найти модель по заданному Id" };

            var dtoProperties = typeof(CommutatorModelRequestDto).GetProperties();
            var modelType = typeof(CommutatorEntity);

            foreach (var dtoProp in dtoProperties)
            {
                var value = dtoProp.GetValue(commutatorModeldto);
                if (value != null)
                {
                    var modelProp = modelType.GetProperty(dtoProp.Name);

                    if (modelProp != null && modelProp.CanWrite)
                    {
                        modelProp.SetValue(existModel, value);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse<bool> { Success = true, Data = true };
        }
    }
}
