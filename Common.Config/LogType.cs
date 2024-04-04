namespace Common.Config;

public enum LogType : short
{
    Verbose = 1,
    Debug,
    Information,
    Warning,
    Error,
    Fatal
}

public static class Logger
{
    public static void Log(LogType logType, string message)
    {
        switch (logType)
        {
            case LogType.Verbose:
                Console.WriteLine($"Verbose: {message}");
                break;
            case LogType.Debug:
                Console.WriteLine($"Debug: {message}");
                break;
            case LogType.Information:
                Console.WriteLine($"Information: {message}");
                break;
            case LogType.Warning:
                Console.WriteLine($"Warning: {message}");
                break;
            case LogType.Error:
                Console.WriteLine($"Error: {message}");
                break;
            case LogType.Fatal:
                Console.WriteLine($"Fatal: {message}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
        }
    }
}