using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Websites;
using CMS.Websites.Internal;

using Microsoft.Extensions.Options;

using ModelContextProtocol.Server;

using System.ComponentModel;
using System.Text.Json;

namespace XperienceCommunity.MCPServer.Tools;

/// <summary>
/// Provides tools for content retrieval and summarization in the MCP server.
/// </summary>
[McpServerToolType]
public static class ContentRetrievalTool
{
    /// <summary>
    /// Retrieves all webpage URLs from the Xperience by Kentico database for a given website channel name.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="pathProvider"></param>
    /// <param name="channelProvider"></param>
    /// <param name="channelName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [McpServerTool(
        Name = nameof(GetAllWebpageUrlsByChannel),
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        ReadOnly = true,
        Title = "Get all web page URLs by channel"),
    Description("Retrieves all absolute webpage URLs for a specified website channel name")]
    public static async Task<IEnumerable<string>> GetAllWebpageUrlsByChannel(
        IOptions<XperienceMCPServerConfiguration> options,
        IInfoProvider<WebPageUrlPathInfo> pathProvider,
        IInfoProvider<WebsiteChannelInfo> channelProvider,
        [Description("The name of the website channel")] string channelName,
        CancellationToken cancellationToken)
    {
        var channels = await channelProvider.Get()
            .Source(s => s.Join<ChannelInfo>(nameof(WebsiteChannelInfo.WebsiteChannelChannelID), nameof(ChannelInfo.ChannelID)))
            .WhereEquals(nameof(ChannelInfo.ChannelName), channelName)
            .GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

        if (channels.FirstOrDefault() is not WebsiteChannelInfo webChannel)
        {
            throw new ArgumentException($"No channel found with the name '{channelName}'.", nameof(channelName));
        }

        var urls = await pathProvider.Get()
            .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebsiteChannelID), webChannel.WebsiteChannelID)
            .WhereTrue(nameof(WebPageUrlPathInfo.WebPageUrlPathIsCanonical))
            .WhereTrue(nameof(WebPageUrlPathInfo.WebPageUrlPathIsLatest))
            .WhereFalse(nameof(WebPageUrlPathInfo.WebPageUrlPathIsDraft))
            .WhereNull(nameof(WebPageUrlPathInfo.WebPageUrlPathRedirectWebPageFormerUrlPathID))
            .Column(nameof(WebPageUrlPathInfo.WebPageUrlPath))
            .Distinct()
            .GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

        string host = $"{(options.Value.UseHttps ? "https" : "http")}://{webChannel.WebsiteChannelDomain}";
        return urls.Select(u => $"{host}{u.WebPageUrlPath}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [McpServerTool(
        Name = nameof(GetContentTypes),
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        ReadOnly = true,
        Title = "Get all content types"),
    Description("Gets a list of all content types in the application")]
    public static async Task<string> GetContentTypes(
        IOptions<XperienceMCPServerConfiguration> options)
    {
        var dataClasses = await DataClassInfoProvider.GetClasses()
            .WhereEquals(nameof(DataClassInfo.ClassType), "Content")
            .Columns(nameof(DataClassInfo.ClassDisplayName), nameof(DataClassInfo.ClassName), nameof(DataClassInfo.ClassContentTypeType))
            .GetEnumerableTypedResultAsync();

        return JsonSerializer.Serialize(
            dataClasses.Select(DataClassResponse.FromDataClassInfo),
            options.Value.SerializerOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [McpServerTool(
        Name = nameof(GetContentTypeDetails),
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        ReadOnly = true,
        Title = "Get content type details"),
    Description("Gets the content type information for the given content type")]
    public static string GetContentTypeDetails(
        IOptions<XperienceMCPServerConfiguration> options,
        [Description("The content type name")] string contentType)
    {
        var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(contentType) ?? throw new ArgumentException($"No content type found with the name '{contentType}'.", nameof(contentType));

        var dto = DataClassDetailResponse.FromDataClassInfo(dataClassInfo);
        return JsonSerializer.Serialize(dto, options.Value.SerializerOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dataClassInfo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [McpServerTool(
        Name = "CreateContentType",
        Destructive = false,
        Idempotent = false,
        OpenWorld = false,
        ReadOnly = false,
        Title = "Create new content type"),
    Description("Creates a new content type in the Xperience by Kentico database from the given settings")]
    public static async Task<string> CreateContentType(
        IOptions<XperienceMCPServerConfiguration> options,
        [Description("The content type definition")] DataClassInfoNewRequest dataClassInfo)
    {
        var dc = await DataClassInfoNewRequest.ToNewDataClassInfo(dataClassInfo);

        DataClassInfoProvider.SetDataClassInfo(dc);

        return JsonSerializer.Serialize(DataClassDetailResponse.FromDataClassInfo(dc), options.Value.SerializerOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dataClassInfo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [McpServerTool(
        Name = "UpdateContentType",
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        ReadOnly = false,
        Title = "Update content type"),
    Description("Updates an existing content type in the Xperience by Kentico database from the given settings")]
    public static string UpdateContentType(
        IOptions<XperienceMCPServerConfiguration> options,
        [Description("The content type definition")] DataClassDetailResponse dataClassInfo)
    {
        var dc = DataClassDetailResponse.ToExistingDataClassInfo(dataClassInfo);

        DataClassInfoProvider.SetDataClassInfo(dc);

        return JsonSerializer.Serialize(DataClassDetailResponse.FromDataClassInfo(dc), options.Value.SerializerOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    [McpServerTool(
        Name = "GetAllContentTypeIcons",
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        ReadOnly = true,
        Title = "Get all web page URLs by channel"),
    Description("Gets all valid icons for content types")]
    public static string GetAllContentTypeIcons(
        IOptions<XperienceMCPServerConfiguration> options
    )
    {
        var allIconNames = typeof(Kentico.Xperience.Admin.Base.Icons)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => f.GetValue(null)?.ToString())
            .Where(value => value != null)
            .ToList();

        return JsonSerializer.Serialize(allIconNames, options.Value.SerializerOptions);
    }
}

