using System.ComponentModel;

using Microsoft.Extensions.AI;

using ModelContextProtocol.Server;

namespace XperienceCommunity.MCPServer.Prompts;

/// <summary>
/// 
/// </summary>
[McpServerPromptType]
public static class PageBuilderPrompts
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [
        McpServerPrompt(Name = nameof(CreateNewPageBuilderWidget)),
        Description("Creates a prompt with instructions to create a new Page Builder widget.")
    ]
    public static ChatMessage CreateNewPageBuilderWidget() =>
        new(ChatRole.User, """
        A Page Builder widget always follows the conventions below

        - Is a C# class that inherits from `ViewComponent` and has a class name suffixed with "Widget"
        - Has a properties class implementing `IWidgetProperties`, matching the name of the widget suffixed with "Properties"
        - Has a view model class matching the name of the widget, suffixed with "ViewModel"
        - Has a single method with the following signature where "WidgetPropertiesClassName" is the name of the generated properties class

        ```csharp
        public async Task<ViewViewComponentResult> InvokeAsync(WidgetPropertiesClassName properties)
        {
            var model = new WidgetViewModel(properties);

            return View("~/Components/PageBuilder/Widgets/WidgetName/WidgetName.cshtml", model);
        }
        ```

        - Uses C# primary constructors
        - The primary constructor should have an IContentRetriever parameter assigned to a private readonly field named contentRetriever
        - The widget, properties, and view model classes should all be in the same file
        - Has a Razor view named the same as the widget without the "Widget" suffix
        - Both the Widget C# class and Razor view are located in a folder for the widget, 
          and the folder should be in the ASP.NET Core application's ~/Components/Widgets folder and not include the "Widget" suffix
        """);
}
