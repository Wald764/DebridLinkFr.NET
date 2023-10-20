﻿namespace DebridLinkFrNET;

public class DebridLinkFrException : Exception
{
    public DebridLinkFrException(String error, String errorCode)
        : base(GetMessage(error, errorCode))
    {
        ServerError = error;
        ErrorCode = errorCode;
        Error = GetMessage(error, errorCode);
    }

    public String ServerError { get; }
    public String ErrorCode { get; }
    public String Error { get; }

    private static String GetMessage(String error, String errorCode)
    {
        return $"{error} ({errorCode})";
    }
}