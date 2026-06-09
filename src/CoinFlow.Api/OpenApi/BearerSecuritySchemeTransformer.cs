using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CoinFlow.Api.OpenApi;

internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) => _authenticationSchemeProvider = authenticationSchemeProvider;

    public async Task TransformAsync(
        OpenApiDocument document, 
        OpenApiDocumentTransformerContext context, 
        CancellationToken cancellationToken)
    {
        document.Info = new()
        {
            Title = "CoinFlow API",
            Description = "API para gerenciamento financeiro pessoal.",
            Version = "v1",
            Contact = new() { Name = "CoinFlow Team" }
        };

        var authSchemas = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (!authSchemas.Any(x => x.Name == "Bearer"))
            return;
        
        var components = document.Components ?? new OpenApiComponents();
        document.Components = components;

        components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Token JWT. O prefixo 'Bearer' é adicionado automaticamente."
        };
    }
}
