namespace Domus.Api.Extensions;

public interface IEndpointGroup
{
    void Map(IEndpointRouteBuilder app);
}