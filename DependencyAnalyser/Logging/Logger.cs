using Microsoft.Extensions.Logging;

namespace AssemblyDependencyAnalyser.Logging
{
    /// <summary>
    /// Static logger functionality.
    /// </summary>
    public static class Logger
    {
        private static ILoggerFactory _loggerFactory;
        private static ILogger _logger;

        /// <summary>
        /// Returns static logger instance.
        /// </summary>
        public static ILogger GetLogger => _logger;

        /// <summary>
        /// Default constructor - creates default instance of logger.
        /// </summary>
        static Logger()
        {
            // Create default logger factory and logger
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug).AddConsole();
            });

            _logger = _loggerFactory.CreateLogger("DependencyAnalyser");
        }

        /// <summary>
        /// Logs info message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Log(string message) => GetLogger.Log(LogLevel.Information, message);

        /// <summary>
        /// Logs message with given level of severity.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="logLevel">Log level (severity).</param>
        public static void Log(string message, LogLevel logLevel) => GetLogger.Log(logLevel, message);

        /// <summary>
        /// Logs exception message as error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="e">Exception to log.</param>
        public static void LogException(string message, Exception e) => GetLogger.Log(LogLevel.Error, message, e);

        /// <summary>
        /// Overrides the default logger factory and logger.
        /// </summary>
        /// <param name="loggerFactory">User specified logger factory.</param>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (loggerFactory != null)
            {
                _loggerFactory = loggerFactory;
                _logger = _loggerFactory.CreateLogger("DependencyAnalyser");
            }
        }
    }
}
