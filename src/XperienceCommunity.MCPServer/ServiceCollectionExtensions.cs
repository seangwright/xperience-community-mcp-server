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
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddXperienceMCPServer(this IServiceCollection services) =>
        services
            .AddXperienceMCPServer(options => { });

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseXperienceMCPServer(this WebApplication app)
    {
        var options = app.Services
           .GetRequiredService<IOptions<XperienceMCPServerConfiguration>>()
           .Value;

        app.MapMcp(options.BasePath);

        return app;
    }
}
