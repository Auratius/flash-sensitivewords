using System;

namespace Flash.SensitiveWords.Domain.Entities;

/// <summary>
/// Represents operation statistics for tracking API usage
/// </summary>
public class OperationStat
{
    /// <summary>
    /// Gets or sets the unique identifier for this statistic
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of operation (CREATE, READ, UPDATE, DELETE, SANITIZE)
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of resource being operated on
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total count of this operation
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this statistic was last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Enumeration of operation types for tracking
/// </summary>
public static class OperationType
{
    public const string Create = "CREATE";
    public const string Read = "READ";
    public const string Update = "UPDATE";
    public const string Delete = "DELETE";
    public const string Sanitize = "SANITIZE";
}

/// <summary>
/// Enumeration of resource types for tracking
/// </summary>
public static class ResourceType
{
    public const string SensitiveWord = "SensitiveWord";
    public const string Message = "Message";
}
