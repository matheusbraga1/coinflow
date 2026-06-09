using CoinFlow.Api.OpenApi;

namespace CoinFlow.Api.Extensions;

public static class OpenApiExtension
{
    public static IServiceCollection AddOpenApiWithJwt(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<AuthorizeOperationTransformer>();
        });

        return services;
    }
}
