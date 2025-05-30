# Xperience Community MCP Server

[![CI: Build and Test](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/ci.yml/badge.svg)](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/ci.yml)

[![Release: Publish to NuGet](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/publish.yml/badge.svg)](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/publish.yml)

[![NuGet Package](https://img.shields.io/nuget/v/XperienceCommunity.MCPServer.svg)](https://www.nuget.org/packages/XperienceCommunity.MCPServer)

## Description

An [MCP Server](https://code.visualstudio.com/docs/copilot/chat/mcp-servers) built with the [.NET MCP Server SDK](https://github.com/modelcontextprotocol/csharp-sdk) tailored for Xperience by Kentico projects and installed as a NuGet package.

Why use this library? By exposing a discrete set of documented tools to [an AI agent](https://code.visualstudio.com/blogs/2025/02/24/introducing-copilot-agent-mode) in an Xperience by Kentico project, that agent has more well structured context and capabilities. This means it can be a better copilot to developers building features for marketers using Xperience by Kentico.

For a more detailed explanation of this project, read the blog post _[Turn your Xperience by Kentico application into an MCP Server](https://community.kentico.com/blog/turn-your-xperience-by-kentico-application-into-an-mcp-server)_.

## Screenshots

 <a href="https://raw.githubusercontent.com/seangwright/xperience-community-mcp-server/main/images/mcp-server-vs-code-xperience-dancing-goat.png">
    <img src="https://raw.githubusercontent.com/seangwright/xperience-community-mcp-server/main/images/mcp-server-vs-code-xperience-dancing-goat.png"
    width="400" alt="Using the MCP Server in VS Code">
</a>

## Requirements

### Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 30.4.1         | 1.0.0           |

### Dependencies

- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.kentico.com)

### Other requirements

- [The latest April 2025 release](https://code.visualstudio.com/updates/v1_100#_mcp-support-for-streamable-http) of VS Code (v1.100) or newer, which supports MCP Servers using HTTP streaming.

## Package Installation

Add the package to your application using the .NET CLI

```powershell
dotnet add package XperienceCommunity.MCPServer
```

## Quick Start

1. Once the package is installed, update your Xperience by Kentico application `Program.cs`

   ```csharp
   // Program.cs

   // ...

   // Adds the MCP dependencies
   if (builder.Environment.IsDevelopment())
   {
       builder.Services.AddXperienceMCPServer();
   }

   // ...

   // Adds the MCP endpoint
   if (builder.Environment.IsDevelopment())
   {
       app.UseXperienceMCPServer();
   }

   app.Kentico().MapRoutes();

   // ...
   ```

1. Start your Xperience by Kentico application.

1. Set up your [MCP Server configuration VS Code](https://code.visualstudio.com/docs/copilot/chat/mcp-servers#_enable-mcp-support-in-vs-code)

   ```json
   {
     "servers": {
       "xperience-mcp-server": {
         "type": "http",
         "url": "http://localhost:<your-port-here>/xperience-mcp"
       }
     }
   }
   ```

   You can use the `.vscode/mcp.json` configuration as an example. The scheme, domain, and port come from the `examples/DancingGoat/Properties/launchSettings.json` file since the DancingGoat Xperience by Kentico application also runs the MCP Server. The default MCP Server path prefix is `/xperience-mcp`.

   You can use the VS Code command palette to [manage and start your MCP Server](https://code.visualstudio.com/docs/copilot/chat/mcp-servers#_managing-tools) or you can click the "Start" action above your server configuration in `mcp.json`.

   **Note**: VS Code's MCP client [does not yet support self-signed certificates](https://github.com/microsoft/vscode/issues/248170), so you will need to add an http:// binding in your ASP.NET Core `launchSettings.json` file if you have been using https:// and [dev-certs](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs) for your Xperience application. This binding can be any unused localhost port. Ex: `http://localhost:34543`. After you add the binding, update your `mcp.json` to use the `http://` URL.

1. Use the tools exposed by the Xperience Community MCP Server [in agent mode in VS Code](https://code.visualstudio.com/docs/copilot/chat/mcp-servers#_use-mcp-tools-in-agent-mode) with a prompt in GitHub Copilot chat.

   ```text
   Tell me about some of the web page content types in this project and a summary of their structure and relationships. For example, let me know if some of these web page content types have references to content items of other types as part of their field definition.
   ```

## Full Instructions

View the [Usage Guide](./docs/Usage-Guide.md) for more detailed instructions and examples.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Security

For any security issues see [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
