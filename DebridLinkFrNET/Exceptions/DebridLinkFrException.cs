using System.Net;

namespace DebridLinkFrNET;

public class DebridLinkFrException : Exception
{
    public DebridLinkFrException(String error, String errorCode)
        : this(error, errorCode, null)
    {
    }

    public DebridLinkFrException(String error, String errorCode, HttpStatusCode? statusCode)
        : base(GetMessage(error, errorCode))
    {
        ServerError = error;
        ErrorCode = errorCode;
        Error = GetMessage(error, errorCode);
        StatusCode = statusCode;
    }

    public String ServerError { get; }
    public String ErrorCode { get; }
    public String Error { get; }

    /// <summary>
    ///     The HTTP status code returned by the API, when the error comes from an HTTP response.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    private static String GetMessage(String error, String errorCode)
    {
        return $"{error} ({errorCode})";
    }
}