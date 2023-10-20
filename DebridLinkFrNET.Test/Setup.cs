using System;

namespace DebridLinkFrNET.Test;

public static class Setup
{
    public static String ApiKey => System.IO.File.ReadAllText(@"\\192.168.0.18\share\projects\DebridLinkFr.NET\DebridLinkFrNET.Test\secret.txt");
}