using Google.GenAI;
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
    public class PcService : IPcService, IDeviceSaveStrategy
    {
        private readonly AppDbContext _dbContext;

        public ParseDevice DeviceType => ParseDevice.PС;

        public PcService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResponse<bool>> ProcessAndSaveAsync(List<ProductsModel> products, SitesToParse site)
        {
            var parsedPcsDevices = new List<PcModelRequestDto>();
            foreach (var product in products)
            {
                if (product.DeviceDetails is PcProductDetailsModel pcDetails)
                {
                    var entityDto = new PcModelRequestDto
                    {                      
                        Model = product.ProductModel,
                        AveragePrice = product.AveragePrice,
                        Vendor = site.ToString(),
                        Ports = pcDetails.Ports,
                        TotalPorts = pcDetails.Ports.Sum(x => x.Count), 
                        WifiOptions = pcDetails.WifiOptions, 
                        HardwareSpecs = pcDetails.HardwareSpecs
                        
                    };
                    parsedPcsDevices.Add(entityDto);
                }
            }
            var addPcsOperationResult = await AddPcsToDatabase(parsedPcsDevices);
            if (addPcsOperationResult.Success)
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

        public async Task<ServiceResponse<bool>> AddPcToDatabase(PcModelRequestDto model)
        {
            try
            {
                if (model == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Список устройств пуст." };

                var existModel = await _dbContext.PcTable.Where(x => x.Model == model.Model &&
                                                                     x.Vendor == model.Vendor &&
                                                                     x.AveragePrice == model.AveragePrice &&
                                                                     x.HardwareSpecs == model.HardwareSpecs).FirstOrDefaultAsync();
                if(existModel == null)
                {
                    var pcRequestModelToEntityModel = new PcEntity()
                    {
                        ExternalId = Guid.NewGuid(),
                        Model = model.Model,
                        Vendor = model.Vendor,
                        Ports = model.Ports,
                        HardwareSpecs = model.HardwareSpecs,
                        WifiOptions = model.WifiOptions,
                        AveragePrice = model.AveragePrice,
                        UpdatedAt = DateTime.UtcNow,
                    };
                    await _dbContext.PcTable.AddAsync(pcRequestModelToEntityModel);
                    var tryaddresult = await _dbContext.SaveChangesAsync();
                    if (tryaddresult > 0)
                        return new ServiceResponse<bool> { Data = true, Success = true, Message = "Устройства успешно добавлены" };
                    else { return new ServiceResponse<bool> { Success = false, Message = "Не удалось добавить устройства" }; }
                }
                return new ServiceResponse<bool> { Success = false, Message = "Устройство уже существует в бд" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Не удалось добавить устройства" };
            }
        }

        public async Task<ServiceResponse<bool>> AddPcsToDatabase(List<PcModelRequestDto> listOfModel)
        {
            try
            {
                if (listOfModel == null || !listOfModel.Any())
                    return new ServiceResponse<bool> { Success = false, Message = "Список устройств пуст" };

                int addedCount = 0;

                foreach (var model in listOfModel)
                {
                    var exists = await _dbContext.PcTable.AnyAsync(x =>
                        x.Vendor == model.Vendor &&
                        x.Model == model.Model &&
                        x.HardwareSpecs.CpuModel == model.HardwareSpecs.CpuModel &&
                        x.HardwareSpecs.RamAmountGb == model.HardwareSpecs.RamAmountGb &&
                        x.HardwareSpecs.StorageAmountGb == model.HardwareSpecs.StorageAmountGb);

                    if (!exists)
                    {
                        var pcRequestModelToEntityModel = new PcEntity()
                        {
                            ExternalId = Guid.NewGuid(),
                            Model = model.Model,
                            Vendor = model.Vendor,
                            Ports = model.Ports,
                            HardwareSpecs = model.HardwareSpecs,
                            WifiOptions = model.WifiOptions,
                            AveragePrice = model.AveragePrice,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _dbContext.PcTable.AddAsync(pcRequestModelToEntityModel);
                        addedCount++;
                    }
                }
                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                    return new ServiceResponse<bool> { Success = true, Message = $"Добавлено новых устройств: {addedCount}" };
                }
                return new ServiceResponse<bool> { Success = true, Message = "Все устройства уже есть в базе" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = $"Ошибка: {ex.Message}" };
            }
        }
        public async Task<ServiceResponse<List<PcResponceDto>>> GetAllPcsFromDatabaseAsync()
        {
            try
            {
                var pcs = await _dbContext.PcTable.AsNoTracking().Select(model => new PcResponceDto
                {
                    ExternalId = model.ExternalId,
                    Vendor = model.Vendor,
                    Model = model.Model,
                    AveragePrice = model.AveragePrice,
                    HardwareSpecs = model.HardwareSpecs,
                    Ports = model.Ports,
                    WifiOptions = model.WifiOptions
                }).ToListAsync();

                return new ServiceResponse<List<PcResponceDto>>()
                {
                    Data = pcs,
                    Success = true,
                    Message = pcs.Any() ? "Данные успешно получены." : "Список компьютеров пуст"
                }; 
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PcResponceDto>>
                {
                    Success = false,
                    Message = "Ошибка при получении данных из базы."
                };
            }
        }

        public async Task<ServiceResponse<List<PcResponceDto>>> GetPcsByPriceRange(PriceRangeRequestDto priceRangeModel)
        {
            try
            {
                var entities = await _dbContext.PcTable
                    .AsNoTracking()
                    .Where(x => x.AveragePrice >= priceRangeModel.Min && x.AveragePrice <= priceRangeModel.Max)
                    .ToListAsync();

                var result = entities.Select(model => new PcResponceDto
                {
                    ExternalId = model.ExternalId,
                    Model = model.Model,
                    Vendor = model.Vendor,
                    Ports = model.Ports,
                    AveragePrice = model.AveragePrice,
                    HardwareSpecs = model.HardwareSpecs,
                    WifiOptions = model.WifiOptions
                }).ToList();

                return new ServiceResponse<List<PcResponceDto>>
                {
                    Data = result,
                    Success = true,
                    Message = $"Найдено устройств: {result.Count}"
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<List<PcResponceDto>>
                {
                    Success = false,
                    Message = "Ошибка при фильтрации по цене."
                };
            }
        }

        public async Task<ServiceResponse<bool>> RemovePcFromDbAsync(string ExternalId)
        {
            var GuidFromString = Guid.Parse(ExternalId);
            var existmodel = await _dbContext.PcTable.Where(x => x.ExternalId == GuidFromString).FirstOrDefaultAsync();
            if (existmodel != null)
            {
                _dbContext.Remove(existmodel);
                await _dbContext.SaveChangesAsync();
                return new ServiceResponse<bool> { Success = true, Message = "Модель была найдена и успешно удалена" };
            }
            return new ServiceResponse<bool> { Success = false, Message = "Не удалось найти модель по заданному Id" };
        }

        public async Task<ServiceResponse<bool>> RemovePcsFromDbAsync(List<string> ExternalIds)
        {
            var guids = ExternalIds.Select(Guid.Parse).ToList();

            var existModels = await _dbContext.PcTable
                .Where(x => guids.Contains(x.ExternalId))
                .ToListAsync();

            if (!existModels.Any())
                return new ServiceResponse<bool> { Data = false, Message = "Не найдено ни одной модели" };

            _dbContext.PcTable.RemoveRange(existModels);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Message = "Успешно удалено" };
        }

        public async Task<ServiceResponse<bool>> UpdatePcAsync(string pcExternalId, PcModelRequestDto PcModelDto)
        {
            var parsedToGuidExternalId = Guid.Parse(pcExternalId);
            var existModel = await _dbContext.PcTable
                .FirstOrDefaultAsync(x => x.ExternalId == parsedToGuidExternalId);

            if (existModel == null)
                return new ServiceResponse<bool> { Success = false, Message = "Не удалось найти модель по заданному Id" };

            var dtoProperties = typeof(PcModelRequestDto).GetProperties();
            var modelType = typeof(PcEntity);

            foreach (var dtoProp in dtoProperties)
            {
                var value = dtoProp.GetValue(PcModelDto);
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
