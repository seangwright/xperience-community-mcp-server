using System.ComponentModel;

using Microsoft.Extensions.AI;

using ModelContextProtocol.Server;

namespace XperienceCommunity.MCPServer.Prompts;

/// <summary>
/// 
/// </summary>
[McpServerPromptType]
public static class SqlServerPrompts
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [
        McpServerPrompt(Name = "sql_server_expert"),
        Description("Creates a prompt to help the agent in SQL Server querying with valid syntax and introspection")
    ]
    public static ChatMessage Summarize() =>
        new(ChatRole.User, """
        You are a SQL Server database expert. You can help users understand their database structure,
        write queries, and explain data relationships. When writing queries, ensure they are safe
        and follow best practices. Avoid suggesting queries that could modify or delete data unless
        explicitly requested.

        Available tools:
        - ExecuteQuery: Run a SQL query and get results
        - GetDatabases: List all databases on the server
        - GetTables: List all tables in a database
        - GetColumns: Get column information for a table
        """);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [
        McpServerPrompt(Name = "sql_server_analysis"),
        Description("Creates a prompt to help the agent in SQL Server query result analysis and introspection")
    ]
    public static ChatMessage SqlServerAnalysis() =>
        new(ChatRole.User, """
        Analyze the database structure and provide insights about:
        1. Table relationships and foreign keys
        2. Potential indexing opportunities
        3. Data type choices and their implications
        4. Normalization level and suggestions
        
        Use the available tools to gather information before making recommendations.
        """);
}
