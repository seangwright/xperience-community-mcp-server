using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ModelContextProtocol.Protocol;

namespace XperienceCommunity.MCPServer;

/// <summary>
/// Extension methods for registering MCP server services with the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static Implementation Implementation { get; } = new Implementation()
    {
        Name = "XperienceCommunity.MCPServer",
        Version = "1.0.0" + $"+{DateTime.Now.Ticks}"
    };

    /// <summary>
    /// Adds MCP server services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    [Obsolete("XperienceCommunity.MCPServer is deprecated. Use Xperience by Kentico's native Content type management API + MCP server (https://docs.kentico.com/x/management_api_xp) and Documentation MCP server (https://docs.kentico.com/x/mcp_server_xp) instead.")]
    public static IServiceCollection AddXperienceMCPServer(this IServiceCollection services) =>
        services
            .AddXperienceMCPServer(options => { });

    /// <summary>
    /// Adds MCP server services to the service collection with configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    [Obsolete("XperienceCommunity.MCPServer is deprecated. Use Xperience by Kentico's native Content type management API + MCP server (https://docs.kentico.com/x/management_api_xp) and Documentation MCP server (https://docs.kentico.com/x/mcp_server_xp) instead.")]
    public static IServiceCollection AddXperienceMCPServer(
     this IServiceCollection services,
     Action<XperienceMCPServerConfiguration> configureOptions)
    {
        var configuration = new XperienceMCPServerConfiguration();
        configureOptions(configuration);
        XperienceMCPServerConfiguration.ValidateConfiguration(configuration);
        services.AddSingleton(Options.Create(configuration));

        var builder = services
            .AddMcpServer(options => options.ServerInfo = Implementation)
            .WithHttpTransport();

        configuration.ScannedAssemblies.Add(Assembly.GetExecutingAssembly()!);

        foreach (var assembly in configuration.ScannedAssemblies)
        {
            builder
                .WithToolsFromAssembly(assembly)
                .WithPromptsFromAssembly(assembly);
        }

        return services;
    }

    /// <summary>
    /// Configures the MCP server endpoint.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The application builder for chaining.</returns>
    [Obsolete("XperienceCommunity.MCPServer is deprecated. Use Xperience by Kentico's native Content type management API + MCP server (https://docs.kentico.com/x/management_api_xp) and Documentation MCP server (https://docs.kentico.com/x/mcp_server_xp) instead.")]
    public static IApplicationBuilder UseXperienceMCPServer(this WebApplication app)
    {
        var options = app.Services
           .GetRequiredService<IOptions<XperienceMCPServerConfiguration>>()
           .Value;

        app.MapMcp(options.BasePath);

        return app;
    }
}
