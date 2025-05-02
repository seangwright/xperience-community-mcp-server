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
