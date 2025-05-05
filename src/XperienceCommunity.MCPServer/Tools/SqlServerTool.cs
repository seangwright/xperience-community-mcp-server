using System.ComponentModel;
using System.Text.Json;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using ModelContextProtocol.Server;

namespace XperienceCommunity.MCPServer.Tools;

/// <summary>
/// Provides tools for interacting with a SQL Server database, including executing queries
/// and retrieving metadata such as databases, tables, and columns.
/// </summary>
[McpServerToolType]
public static class SqlServerTool
{
    /// <summary>
    /// Executes a SQL query against the application's database identified in CMSConnectionString and returns the results as a JSON string.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the query results as a JSON string.</returns>
    [
        McpServerTool(
            Name = nameof(ExecuteSQLQuery),
            Destructive = true,
            Idempotent = false,
            OpenWorld = false,
            ReadOnly = false,
            Title = "Execute SQL query"),
        Description("Executes an arbitrary SQL query against the current Xperience by Kentico database and returns the results")
    ]
    public static async Task<string> ExecuteSQLQuery(
        IOptions<XperienceMCPServerConfiguration> options,
        IConfiguration configuration,
        [Description("The SQL query to execute.")] string query,
        CancellationToken cancellationToken)
    {
        string? connectionString = configuration.GetConnectionString("CMSConnectionString");
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<Dictionary<string, object>>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            results.Add(row);
        }

        return JsonSerializer.Serialize(results, options.Value.SerializerOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [
        McpServerTool(
            Name = nameof(GetSQLTables),
            Destructive = false,
            Idempotent = true,
            OpenWorld = false,
            ReadOnly = true,
            Title = "Get SQL tables"
        ),
        Description("Lists all schemas and tables of the current Xperience by Kentico database.")
    ]
    public static async Task<string> GetSQLTables(
        IOptions<XperienceMCPServerConfiguration> options,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        string? connectionString = configuration.GetConnectionString("CMSConnectionString");
        var builder = new SqlConnectionStringBuilder(connectionString);
        string databaseName = builder.InitialCatalog;

        string query = $"""
            USE [{databaseName}];
            SELECT TABLE_SCHEMA, TABLE_NAME 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE = 'BASE TABLE'
            """;

        return await ExecuteSQLQuery(options, configuration, query, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    /// <param name="tableName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [
        McpServerTool(
            Name = nameof(GetSQLTableColumns),
            Destructive = false,
            Idempotent = true,
            OpenWorld = false,
            ReadOnly = true,
            Title = "Get SQL table columns"
        ),
        Description("Lists all columns for the specified table of the current Xperience by Kentico database.")
    ]
    public static async Task<string> GetSQLTableColumns(
        IOptions<XperienceMCPServerConfiguration> options,
        IConfiguration configuration,
        [Description("The SQL database table name.")] string tableName,
        CancellationToken cancellationToken)
    {
        string? connectionString = configuration.GetConnectionString("CMSConnectionString");
        var builder = new SqlConnectionStringBuilder(connectionString);
        string databaseName = builder.InitialCatalog;

        string query = $"""
            USE [{databaseName}];
            SELECT 
                TABLE_CATALOG,              -- Database name
                TABLE_SCHEMA,              -- Schema name
                TABLE_NAME,                -- Table name
                COLUMN_NAME,               -- Column name
                ORDINAL_POSITION,          -- Column position in table
                COLUMN_DEFAULT,            -- Default value
                IS_NULLABLE,               -- YES/NO
                DATA_TYPE,                 -- SQL data type
                CHARACTER_MAXIMUM_LENGTH,   -- Max length for char/varchar
                CHARACTER_OCTET_LENGTH,    -- Max bytes for char/varchar
                NUMERIC_PRECISION,         -- Precision for numeric types
                NUMERIC_PRECISION_RADIX,   -- Base for numeric precision
                NUMERIC_SCALE,            -- Scale for numeric types
                DATETIME_PRECISION,       -- Precision for datetime types
                CHARACTER_SET_NAME,       -- Character set
                COLLATION_NAME,          -- Collation
                DOMAIN_NAME              -- Domain name if column is based on domain
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = '{tableName}'
            """;

        return await ExecuteSQLQuery(options, configuration, query, cancellationToken);
    }
}
