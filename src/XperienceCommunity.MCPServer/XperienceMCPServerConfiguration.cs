using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace XperienceCommunity.MCPServer;

/// <summary>
/// Configuration options for the Model Context Protocol (MCP) server integration with Xperience by Kentico.
/// </summary>
public class XperienceMCPServerConfiguration
{
    /// <summary>
    /// Gets or sets the base URL path for the MCP server endpoint.
    /// </summary>
    /// <remarks>
    /// If not specified, defaults to "/mcp".
    /// </remarks>
    public string BasePath { get; set; } = "/xperience-mcp";

    /// <summary>
    /// 
    /// </summary>
    public bool UseHttps { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public List<Assembly> ScannedAssemblies { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions { WriteIndented = true };

    /// <summary>
    /// Gets or sets the content types to be exposed through the MCP server.
    /// </summary>
    /// <remarks>
    /// If not specified, no content will be exposed. You must explicitly specify which content types to include.
    /// </remarks>
    [Required]
    public List<ContentTypeConfiguration> ContentTypes { get; set; } = [];

    /// <summary>
    /// Gets or sets the maximum number of items to return in a single request.
    /// </summary>
    /// <remarks>
    /// This helps prevent performance issues with overly large result sets.
    /// </remarks>
    public int MaxItemsPerRequest { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to include site context in content queries.
    /// </summary>
    /// <remarks>
    /// When true, content will be filtered by the current site. When false, content from all sites will be included.
    /// </remarks>
    public bool UseSiteContext { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache duration in seconds for MCP content responses.
    /// </summary>
    /// <remarks>
    /// Set to 0 to disable caching.
    /// </remarks>
    public int CacheDurationSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets whether to include system fields in the content returned by the MCP server.
    /// </summary>
    /// <remarks>
    /// System fields include technical properties like NodeID, NodeGUID, etc.
    /// </remarks>
    public bool IncludeSystemFields { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable debug information in API responses.
    /// </summary>
    /// <remarks>
    /// This should be disabled in production environments.
    /// </remarks>
    public bool EnableDebugInfo { get; set; } = false;

    /// <summary>
    /// Gets or sets custom field formatters for specific field types.
    /// </summary>
    /// <remarks>
    /// Key is the field type identifier, value is the fully qualified type name of the formatter.
    /// </remarks>
    public Dictionary<string, string> CustomFieldFormatters { get; set; } = [];

    /// <summary>
    /// Validates the MCP server configuration.
    /// </summary>
    /// <param name="configuration">The configuration to validate.</param>
    /// <exception cref="ValidationException">Thrown if the configuration is invalid.</exception>
    internal static void ValidateConfiguration(XperienceMCPServerConfiguration configuration)
    {
        configuration.ContentTypes ??= [];
        var invalidContentTypes = configuration.ContentTypes
            .Where(contentType => string.IsNullOrWhiteSpace(contentType.CodeName));

        if (invalidContentTypes.Any())
        {
            throw new ValidationException("Content type code name cannot be empty.");
        }

        // Validate path
        if (string.IsNullOrWhiteSpace(configuration.BasePath))
        {
            throw new ValidationException("Base path cannot be empty.");
        }

        if (!configuration.BasePath.StartsWith('/'))
        {
            throw new ValidationException("Base path must start with a forward slash (/).");
        }

        // Validate MaxItemsPerRequest
        if (configuration.MaxItemsPerRequest <= 0)
        {
            throw new ValidationException("MaxItemsPerRequest must be greater than 0.");
        }
    }
}

/// <summary>
/// Configuration for a specific content type to be exposed through the MCP server.
/// </summary>
public class ContentTypeConfiguration
{
    /// <summary>
    /// Gets or sets the code name of the content type.
    /// </summary>
    /// <remarks>
    /// This should match the exact code name as defined in Xperience.
    /// </remarks>
    [Required]
    public string CodeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a friendly name for the content type to use in the MCP context.
    /// </summary>
    /// <remarks>
    /// If not specified, the code name will be used.
    /// </remarks>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the fields from this content type to be included in the MCP context.
    /// </summary>
    /// <remarks>
    /// If not specified, all fields will be included except system fields (unless <see cref="XperienceMCPServerConfiguration.IncludeSystemFields"/> is true).
    /// </remarks>
    public ICollection<string> IncludedFields { get; set; } = [];

    /// <summary>
    /// Gets or sets the fields from this content type to be excluded from the MCP context.
    /// </summary>
    /// <remarks>
    /// This takes precedence over <see cref="IncludedFields"/> if there is a conflict.
    /// </remarks>
    public ICollection<string> ExcludedFields { get; set; } = [];

    /// <summary>
    /// Gets or sets field mappings for this content type.
    /// </summary>
    /// <remarks>
    /// Allows renaming fields in the MCP context. Key is the original field name, value is the name to use in the MCP context.
    /// </remarks>
    public Dictionary<string, string> FieldMappings { get; set; } = [];

    /// <summary>
    /// Gets or sets whether to include related content items in the MCP context.
    /// </summary>
    public bool IncludeRelatedContent { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum depth for including related content.
    /// </summary>
    /// <remarks>
    /// Only applies when <see cref="IncludeRelatedContent"/> is true.
    /// </remarks>
    public int MaxRelatedContentDepth { get; set; } = 1;
}

