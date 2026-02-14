using System.Reflection;
using app.api.Abstracts;

namespace app.api.ServiceExtensions;

public static class EndpointGroupExtensions
{
    public static WebApplication MapFeatureEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(IEndpointGroup);

        var endpointGroupTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && endpointGroupType.IsAssignableFrom(t));

        foreach (var groupType in endpointGroupTypes)
        {

            // Call the static MapGroup method of each found class
            var mapMethod = groupType.GetMethod(nameof(IEndpointGroup.MapGroup), BindingFlags.Public | BindingFlags.Static);
            mapMethod?.Invoke(null, [app]);
        }

        return app;
    }
}
