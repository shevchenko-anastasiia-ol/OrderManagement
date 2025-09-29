using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderManagementBLL.MappingProfiles;
using Xunit;

namespace OrderManagementTests;

public class AutoMapperTests
{
    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(OrderMappingProfile).Assembly);
        }, loggerFactory);

        config.AssertConfigurationIsValid();
    }
}