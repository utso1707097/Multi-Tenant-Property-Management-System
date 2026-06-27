using System.Reflection;

namespace Domus.Api.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var groups = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpointGroup)) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IEndpointGroup>();

        foreach (var group in groups)
            group.Map(app);

        return app;
    }
}