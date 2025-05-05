# Usage Guide

## Sample prompt

```text
Follow these instructions
1. Analyse existing content types (GetContentTypes tool)
2. Find several reusable content types and analyse their details (GetContentTypeDetails tool)
3. Use the patterns and data you discover to construct a new Link content type
4. The Link content type should have the following fields

LinkLabel (text)
LinkDescription (long text - rich text field)
LinkURL (text)
LinkTag (taxonomy - use the taxonomy field definition from another reusable content type)

5. Find a suitable icon for the content type (GetAllContentTypeIcons tool)
6. Create the new Link content type (CreateContentType tool)

Use the schema and properties for requests and responses defined by the tools, not other naming conventions
```

This prompt will guide your agent to create a new, valid Link content type using the tools exposed by the MCP Server.

![New Link content type in the CI repository](/images/new-link-content-type-ci-repository.jpg)

With [CI synchronization](https://docs.kentico.com/x/FAKQC) enabled, the new content type can be shared with other developers on your team through source control.

![New Link content type in the Content Types application in Xperience's administration](/images/new-link-content-type-administration.jpg)

Additionally, the new content type can be further customized with the agent or through Xperience's administration UI.
