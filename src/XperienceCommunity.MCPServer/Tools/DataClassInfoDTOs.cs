using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;

namespace XperienceCommunity.MCPServer.Tools;

/// <summary>
/// 
/// </summary>
public class DataClassResponse
{
    /// <summary>
    /// 
    /// </summary>
    public Guid ClassGUID { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public string ClassName { get; init; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string ClassDisplayName { get; init; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string ClassContentTypeType { get; init; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DataClassResponse FromDataClassInfo(DataClassInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        return new DataClassResponse
        {
            ClassGUID = info.ClassGUID,
            ClassDisplayName = info.ClassDisplayName,
            ClassName = info.ClassName,
            ClassContentTypeType = info.ClassContentTypeType,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DataClassResponse FromReusableFieldSchema(ReusableFieldSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        return new DataClassResponse
        {
            ClassGUID = schema.Guid,
            ClassDisplayName = schema.DisplayName,
            ClassName = schema.Name,
            ClassContentTypeType = "ReusableFieldSchema",
        };
    }
}

/// <summary>
/// A serializable DTO representation of CMS.DataEngine.DataClassInfo
/// </summary>
public class DataClassDetailResponse
{
    /// <summary>
    /// The unique identifier of the class.
    /// </summary>
    public int ClassID { get; init; }

    /// <summary>
    /// The display name of the class.
    /// </summary>
    public string ClassDisplayName { get; init; } = string.Empty;

    /// <summary>
    /// The code name of the class.
    /// </summary>
    public string ClassName { get; init; } = string.Empty;

    /// <summary>
    /// The name of the table where the class data is stored.
    /// </summary>
    public string ClassTableName { get; init; } = string.Empty;

    /// <summary>
    /// The class type.
    /// </summary>
    public string ClassType { get; init; } = string.Empty;

    /// <summary>
    /// Email, Reusable, or Website
    /// </summary>
    public string ClassContentTypeType { get; init; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ClassShortName { get; init; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public bool ClassWebPageHasURL { get; init; } = false;

    /// <summary>
    /// 
    /// </summary>
    public string ClassIconClass { get; init; } = string.Empty;

    /// <summary>
    /// Does the class represent a content type.
    /// </summary>
    public bool ClassIsContentType { get; init; }

    /// <summary>
    /// The form definition of the class.
    /// </summary>
    public string ClassFormDefinition { get; init; } = string.Empty;

    /// <summary>
    /// The class XML schema.
    /// </summary>
    public string ClassXmlSchema { get; init; } = string.Empty;

    /// <summary>
    /// The last modified timestamp.
    /// </summary>
    public DateTime ClassLastModified { get; init; }

    /// <summary>
    /// The GUID of the class.
    /// </summary>
    public Guid ClassGUID { get; init; }

    /// <summary>
    /// A list of all reusable field schemas for the content type.
    /// </summary>
    public IEnumerable<ReusableFieldSchema> ReusableFieldSchemas { get; init; } = Enumerable.Empty<ReusableFieldSchema>();

    /// <summary>
    /// Creates a new instance of DataClassInfoDto from a DataClassInfo instance.
    /// </summary>
    /// <param name="info">The DataClassInfo to convert</param>
    /// <param name="manager"></param>
    /// <returns>A new DataClassInfoDto instance</returns>
    public static DataClassDetailResponse FromDataClassInfo(DataClassInfo info, IReusableFieldSchemaManager manager)
    {
        ArgumentNullException.ThrowIfNull(info);

        return new DataClassDetailResponse
        {
            ClassID = info.ClassID,
            ClassDisplayName = info.ClassDisplayName,
            ClassName = info.ClassName,
            ClassTableName = info.ClassTableName,
            ClassType = info.ClassType,
            ClassContentTypeType = info.ClassContentTypeType,
            ClassShortName = info.ClassShortName,
            ClassWebPageHasURL = info.ClassWebPageHasUrl,
            ClassIconClass = info.ClassIconClass,
            ClassIsContentType = info.ClassType.Equals("content", StringComparison.OrdinalIgnoreCase),
            ClassFormDefinition = info.ClassFormDefinition,
            ClassXmlSchema = info.ClassXmlSchema,
            ClassLastModified = info.ClassLastModified,
            ClassGUID = info.ClassGUID,
            ReusableFieldSchemas = manager.GetSchemasForContentType(info.ClassName)
        };
    }

    /// <summary>
    /// Updates an existing instance of DataClassInfo from a DataClassInfoDto instance.
    /// </summary>
    /// <param name="dto">The DataClassInfoDto to convert</param>
    /// <returns>An updated DataClassInfo instance</returns>
    public static DataClassInfo ToExistingDataClassInfo(DataClassDetailResponse dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var info = DataClassInfo.New(c =>
        {
            c.ClassID = dto.ClassID;
            c.ClassGUID = dto.ClassGUID;
            c.ClassDisplayName = dto.ClassDisplayName;
            c.ClassName = dto.ClassName;
            c.ClassTableName = dto.ClassTableName;
            c.ClassType = dto.ClassType;
            c.ClassIconClass = dto.ClassIconClass;
            c.ClassContentTypeType = dto.ClassContentTypeType;
            c.ClassFormDefinition = dto.ClassFormDefinition;
            c.ClassXmlSchema = dto.ClassXmlSchema;
            c.ClassWebPageHasUrl = dto.ClassWebPageHasURL;
            c.ClassShortName = dto.ClassShortName;
        });

        return info;
    }
}

/// <summary>
/// A DTO for creating a new DataClassInfo instance.
/// </summary>
public class DataClassInfoNewRequest
{
    /// <summary>
    /// The display name of the class.
    /// </summary>
    public string ClassDisplayName { get; init; } = string.Empty;

    /// <summary>
    /// The code name of the class.
    /// </summary>
    public string ClassName { get; init; } = string.Empty;

    /// <summary>
    /// The name of the table where the class data is stored.
    /// </summary>
    public string ClassTableName { get; init; } = string.Empty;

    /// <summary>
    /// The class type.
    /// </summary>
    public string ClassType { get; init; } = string.Empty;

    /// <summary>
    /// Email, Reusable, or Website.
    /// </summary>
    public string ClassContentTypeType { get; init; } = string.Empty;

    /// <summary>
    /// The short name of the class.
    /// </summary>
    public string ClassShortName { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if the webpage has a URL.
    /// </summary>
    public bool ClassWebPageHasURL { get; init; } = false;

    /// <summary>
    /// The icon class of the class.
    /// </summary>
    public string ClassIconClass { get; init; } = string.Empty;

    /// <summary>
    /// The form definition of the class.
    /// </summary>
    public string ClassFormDefinition { get; init; } = string.Empty;

    /// <summary>
    /// The XML schema of the class.
    /// </summary>
    public string ClassXmlSchema { get; init; } = string.Empty;

    /// <summary>
    /// Creates a new instance of DataClassInfo from a DataClassInfoDto instance.
    /// </summary>
    /// <param name="dto">The DataClassInfoDto to convert</param>
    /// <returns>A new DataClassInfo instance</returns>
    public static async Task<DataClassInfo> ToNewDataClassInfo(DataClassInfoNewRequest dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        string shortName = await GetValidShortName(dto.ClassShortName);

        var info = DataClassInfo.New(c =>
        {
            c.ClassDisplayName = dto.ClassDisplayName;
            c.ClassName = dto.ClassName;
            c.ClassTableName = dto.ClassTableName;
            c.ClassType = dto.ClassType;
            c.ClassIconClass = dto.ClassIconClass;
            c.ClassContentTypeType = dto.ClassContentTypeType;
            c.ClassFormDefinition = dto.ClassFormDefinition;
            c.ClassXmlSchema = dto.ClassXmlSchema;
            c.ClassWebPageHasUrl = dto.ClassWebPageHasURL;
            c.ClassShortName = shortName;
        });

        return info;
    }

    /// <summary>
    /// From _internal class ClassShortNameProvider_
    /// </summary>
    /// <param name="codeName"></param>
    /// <returns></returns>
    private static Task<string> GetValidShortName(string codeName)
    {
        string originalShortName = ContentTypeShortNameHelper.CleanCodeName(codeName);
        string shortName = originalShortName;

        int index = 1;
        while (ShortNameExists(shortName))
        {
            shortName = $"{originalShortName}{index}";
            index++;
        }

        return Task.FromResult(shortName);
    }

    private static bool ShortNameExists(string shortName)
    {
        int count = DataClassInfoProvider.GetClasses()
            .WhereEquals(nameof(DataClassInfo.ClassShortName), shortName)
            .Count;

        return count > 0;
    }
}
