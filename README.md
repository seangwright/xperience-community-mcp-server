# Xperience Community MCP Server

[![CI: Build and Test](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/ci.yml/badge.svg)](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/ci.yml)

[![Release: Publish to NuGet](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/publish.yml/badge.svg)](https://github.com/seangwright/xperience-community-mcp-server/actions/workflows/publish.yml)

[![NuGet Package](https://img.shields.io/nuget/v/XperienceCommunity.MCPServer.svg)](https://www.nuget.org/packages/XperienceCommunity.MCPServer)

## Description

An [MCP Server](https://code.visualstudio.com/docs/copilot/chat/mcp-servers) built with the [.NET MCP Server SDK](https://github.com/modelcontextprotocol/csharp-sdk) tailored for Xperience by Kentico projects and installed as a NuGet package.

## Screenshots

 <a href="https://raw.githubusercontent.com/seangwright/xperience-community-mcp-server/main/images/mcp-server-vs-code-xperience-dancing-goat.webp">
    <img src="https://raw.githubusercontent.com/seangwright/xperience-community-mcp-server/main/images/mcp-server-vs-code-xperience-dancing-goat.webp"
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

- [VS Code Insiders](https://code.visualstudio.com/insiders/) until HTTP Streaming MCP support is released in VS Code in May-June 2025.

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

1. Set up your [MCP Server configuration VS Code](https://code.visualstudio.com/docs/copilot/chat/mcp-servers#_enable-mcp-support-in-vs-code)

1. You can use the `.vscode/mcp.json` configuration as an example. The scheme, domain, and port come from the `examples/DancingGoat/Properties/launchSettings.json` file since the DancingGoat Xperience by Kentico application also runs the MCP Server. The default MCP Server path prefix is `/xperience-mcp`

1. Use the tools exposed by the Xperience Community MCP Server [in agent mode in VS Code](https://code.visualstudio.com/docs/copilot/chat/mcp-servers#_use-mcp-tools-in-agent-mode).

## Full Instructions

View the [Usage Guide](./docs/Usage-Guide.md) for more detailed instructions.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Security

For any security issues see [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
