using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.DbContext;
using NetOptimizerParserApi.Enums;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.DbEntities;
using NetOptimizerParserApi.Models.Dto_s;

namespace NetOptimizerParserApi.Services.Business
{
    public class RouterService : IRouterService, IDeviceSaveStrategy
    {
        private readonly AppDbContext dbcontext;
        public ParseDevice DeviceType => ParseDevice.Routers;

        public RouterService(AppDbContext context)
        {
            dbcontext = context;
        }

        public async Task<ServiceResponse<bool>> ProcessAndSaveAsync(List<ProductsModel> products, SitesToParse site)
        {
            var parsedRoutersDevices = new List<RouterModelRequestDto>();

            foreach (var product in products)
            {
                if (product.DeviceDetails is RouterProductDetailsModel router)
                {
                    var modelDtoToAdd = new RouterModelRequestDto()
                    {
                        Model = product.ProductModel,
                        AveragePrice = product.AveragePrice,
                        Vendor = site.ToString(),
                        Ports = router.Ports,
                        IsManaged = router.IsManaged,
                        Performance = router.Performance,
                        ProtocolSupport = router.ProtocolSupport,
                        WifiOptions = router.WifiOptions,
                        TotalPorts = router.Ports.Sum(p => p.Count)
                    };
                    parsedRoutersDevices.Add(modelDtoToAdd);
                }
            }

            var AddRoutersOperationResult = await AddRoutersToDbAsync(parsedRoutersDevices);

            if (AddRoutersOperationResult.Success)
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
                Data = false,
                Success = false,
                Message = "Не удалось добавить устройства"
            };
        }
        public async Task<ServiceResponse<bool>> AddRoutersToDbAsync(List<RouterModelRequestDto> models)
        {
            var response = new ServiceResponse<bool>();

            if (models == null || !models.Any())
            {
                response.Success = false;
                response.Message = "Список для добавления пуст.";
                return response;
            }
            try
            {
                var existingNames = await dbcontext.RoutersTable
                    .Select(r => new { r.Vendor, r.Model })
                    .ToListAsync();

                var newRouters = new List<RouterEntity>();

                foreach (var model in models)
                {
                    bool exists = existingNames.Any(e =>
                        e.Vendor == model.Vendor &&
                        e.Model == model.Model);

                    if (!exists)
                    {
                        newRouters.Add(new RouterEntity
                        {
                            Model = model.Model,
                            Vendor = model.Vendor,
                            AvveragePrice = model.AveragePrice,
                            IsManaged = model.IsManaged,
                            Ports = model.Ports,
                            PerformanceSpecs = model.Performance,
                            ProtocolSupport = model.ProtocolSupport,
                            WifiOptions = model.WifiOptions,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (newRouters.Any())
                {
                    await dbcontext.RoutersTable.AddRangeAsync(newRouters);
                    await dbcontext.SaveChangesAsync();

                    response.Data = true;
                    response.Success = true;
                    response.Message = $"Успешно добавлено {newRouters.Count} новых роутеров.";
                }
                else
                {
                    response.Data = false;
                    response.Success = true; 
                    response.Message = "Все устройства из списка уже существуют в базе.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ошибка при сохранении в БД.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> AddRouterToDbAsync(RouterModelRequestDto model)
        {
            var response = new ServiceResponse<bool>();

            if (model == null)
                return new ServiceResponse<bool> {
                    Success = false,
                    Message = "Данные модели отсутствуют." };
            
            try
            {
                var existRouter = await dbcontext.RoutersTable
                    .FirstOrDefaultAsync(x => x.Model == model.Model && x.Vendor == model.Vendor);

                if (existRouter == null)
                {
                    var entity = new RouterEntity
                    {
                        Model = model.Model,
                        Vendor = model.Vendor,
                        AvveragePrice = model.AveragePrice,
                        Ports = model.Ports,
                        PerformanceSpecs = model.Performance,
                        IsManaged   = model.IsManaged,
                        ProtocolSupport = model.ProtocolSupport,
                        WifiOptions = model.WifiOptions,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await dbcontext.RoutersTable.AddAsync(entity); 
                    await dbcontext.SaveChangesAsync();

                    response.Data = true;
                    response.Success = true;
                    response.Message = "Роутер успешно добавлен.";
                }
                else
                {
                    response.Data = false;
                    response.Success = false;
                    response.Message = "Такой роутер уже существует.";
                }
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Критическая ошибка при сохранении роутера.";
            }
            return response;
        }

        public async Task<ServiceResponse<List<RouterResponceDto>>> GetAllRoutersAsync()
        {
            try
            {
                var result = await dbcontext.RoutersTable
                    .AsNoTracking()
                    .Select(model => new RouterResponceDto
                    {
                        IsManaged = model.IsManaged,
                        Vendor = model.Vendor,
                        TotalPorts = model.Ports.Select(x => x.Count).Sum(),
                        Model = model.Model,
                        Ports = model.Ports,
                        Performance = model.PerformanceSpecs,
                        ProtocolSupport = model.ProtocolSupport,
                        WifiOptions = model.WifiOptions,
                        AveragePrice = model.AvveragePrice,
                    }).ToListAsync();
                return new ServiceResponse<List<RouterResponceDto>>
                {
                    Data = result,
                    Success = true,
                    Message = result.Any() ? "Данные успешно получены." : "Список маршрутизаторов пуст."
                };
            }
            catch
            {
                return new ServiceResponse<List<RouterResponceDto>>
                {
                    Success = false,
                    Message = "Ошибка при получении данных из базы."
                };
            }
          
        }

        public async Task<ServiceResponse<List<RouterResponceDto>>> GetRoutersByPriceRange(PriceRangeRequestDto priceRangeModel)
        {
            throw new NotImplementedException();
        }

   
    }
}
