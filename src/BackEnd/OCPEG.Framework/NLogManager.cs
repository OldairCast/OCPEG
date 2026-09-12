using NLog;

namespace OCPEG.Framework
{
    public class NLogManager 
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public NLogManager() {
        }
        public static void LogDebug(string controller, string action, string message)
        {
            message = $"Controller: {controller} | Action: {action} | " + message;
            logger.Debug(message);
        }
        public static void LogError(string message)
        {
            message = $"---------- Erro=> " + message;
            logger.Error(message);
        }
        public static void LogInfoX(string controller, string action, string message)
        {
            message = $"Controller: {controller} | Action: {action} | " + message;
            logger.Info(message);
        }
        public static void LogInfo(string message)
        {
            message = $"Controller:" + message;
            logger.Info(message);
        }

        public static void LogWarn(string controller, string action, string message)
        {
            message = $"Controller: {controller} | Action: {action} | " + message;
            logger.Warn(message);
        }
    }
}
