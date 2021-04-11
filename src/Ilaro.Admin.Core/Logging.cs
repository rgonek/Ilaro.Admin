using System;
using System.IO;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public interface IInternalLogger
    {
        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string format, params object[] args);

        void Fatal(object message);

        void Fatal(object message, Exception exception);

        void Debug(object message);

        void Debug(object message, Exception exception);

        void DebugFormat(string format, params object[] args);

        void Info(object message);

        void Info(object message, Exception exception);

        void InfoFormat(string format, params object[] args);

        void Warn(object message);

        void Warn(object message, Exception exception);

        void WarnFormat(string format, params object[] args);
    }

    public interface ILoggerFactory
    {
        IInternalLogger LoggerFor(string keyName);

        IInternalLogger LoggerFor(Type type);
    }

    //public class LoggerProvider
    //{
    //    private const string IlaroAdminLoggerConfKey = "ilaro.admin-logger";
    //    private readonly ILoggerFactory loggerFactory;
    //    private static LoggerProvider instance;

    //    static LoggerProvider()
    //    {
    //        string ilaroAdminLoggerClass = GetIlaroAdminLoggerClass();
    //        ILoggerFactory loggerFactory = string.IsNullOrEmpty(ilaroAdminLoggerClass) ? new NoLoggingLoggerFactory() : GetLoggerFactory(ilaroAdminLoggerClass);
    //        SetLoggersFactory(loggerFactory);
    //    }

    //    private static ILoggerFactory GetLoggerFactory(string ilaroAdminLoggerClass)
    //    {
    //        ILoggerFactory loggerFactory;
    //        var loggerFactoryType = Type.GetType(ilaroAdminLoggerClass);
    //        try
    //        {
    //            loggerFactory = (ILoggerFactory)Activator.CreateInstance(loggerFactoryType);
    //        }
    //        catch (MissingMethodException ex)
    //        {
    //            throw new ApplicationException("Public constructor was not found for " + loggerFactoryType, ex);
    //        }
    //        catch (InvalidCastException ex)
    //        {
    //            throw new ApplicationException(loggerFactoryType + "Type does not implement " + typeof(ILoggerFactory), ex);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ApplicationException("Unable to instantiate: " + loggerFactoryType, ex);
    //        }
    //        return loggerFactory;
    //    }

    //    private static string GetIlaroAdminLoggerClass()
    //    {
    //        var ilaroAdminLogger = ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => IlaroAdminLoggerConfKey.Equals(k.ToLowerInvariant()));
    //        string ilaroAdminLoggerClass = null;
    //        if (string.IsNullOrEmpty(ilaroAdminLogger))
    //        {
    //            // look for log4net.dll
    //            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
    //            string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
    //            string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
    //            string log4NetDllPath = binPath == null ? "log4net.dll" : Path.Combine(binPath, "log4net.dll");

    //            if (System.IO.File.Exists(log4NetDllPath) || AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "log4net"))
    //            {
    //                ilaroAdminLoggerClass = typeof(Log4NetLoggerFactory).AssemblyQualifiedName;
    //            }
    //        }
    //        else
    //        {
    //            ilaroAdminLoggerClass = ConfigurationManager.AppSettings[ilaroAdminLogger];
    //        }
    //        return ilaroAdminLoggerClass;
    //    }

    //    public static void SetLoggersFactory(ILoggerFactory loggerFactory)
    //    {
    //        instance = new LoggerProvider(loggerFactory);
    //    }

    //    private LoggerProvider(ILoggerFactory loggerFactory)
    //    {
    //        this.loggerFactory = loggerFactory;
    //    }

    //    public static IInternalLogger LoggerFor(string keyName)
    //    {
    //        return instance.loggerFactory.LoggerFor(keyName);
    //    }

    //    public static IInternalLogger LoggerFor(Type type)
    //    {
    //        return instance.loggerFactory.LoggerFor(type);
    //    }
    //}
}