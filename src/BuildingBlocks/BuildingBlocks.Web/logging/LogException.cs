// we don't want to register to Dependency Injection Container
// we make it public static so that it can be accessed globally
using Serilog;

namespace BuildingBlocks.Web.Logging
{
    public static class LogException
    {
        // Anytime an exception occurs anywhere in the application it would be passed to this method and assigned via its constructor
        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex.Message);
            LogToDebugger(ex.Message);
            LogToConsole(ex.Message);
        }

        public static void LogToFile(string message) => Log.Information(message);

        public static void LogToDebugger(string message) => Log.Debug(message);

        public static void LogToConsole(string message) => Log.Warning(message);
    }
}
