namespace DemoShop.Domain.Common.Exceptions;

public static class ErrorMessages
{
    public static string NotFound<T>(string operation, string? identifier = null) =>
        identifier != null
            ? $"{typeof(T).Name} not found while {operation} with identifier {identifier}"
            : $"No {typeof(T).Name} found while {operation}";

    private static string Error<T>(string operation, string type, string? identifier = null) =>
        identifier != null
            ? $"{type} error occurred while {operation} {typeof(T).Name} [{identifier}]"
            : $"{type} error occurred while {operation} collection of {typeof(T).Name}";

    public static string DatabaseError<T>(string operation, string? identifier = null) =>
        Error<T>(operation, "Database", identifier);

    public static string OperationError<T>(string operation, string? identifier = null) =>
        Error<T>(operation, "Operation", identifier);

    public static string DeleteFailed<T>(string operation, string? identifier = null)
        => $"Failed to delete {typeof(T).Name} while {operation} [{identifier ?? "collection"}]";
}
