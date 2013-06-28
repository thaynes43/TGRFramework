using System.IO;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace SlotMachine.Prototype.Tools
{
    /// <inheritdoc/>
    public class LogTool : ILogTool
    {
        public LogTool(string fileName)
            : this ("Appender", fileName)
        {
        }

        public LogTool(string template, string fileName)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
            this.Log = this.GetNewLogger(template, fileName);
        }

        private ILog Log { get; set; }

        /// <inheritdoc/>
        public void Info(string format, params object[] args)
        {
            this.Log.Info(string.Format(format, args));
        }

        /// <inheritdoc/>
        public void Debug(string format, params object[] args)
        {
            this.Log.Debug(string.Format(format, args));
        }

        /// <inheritdoc/>
        public void Warn(string format, params object[] args)
        {
            this.Log.Warn(string.Format(format, args));
        }

        /// <inheritdoc/>
        public void Error(string format, params object[] args)
        {
            this.Log.Error(string.Format(format, args));
        }

        /// <inheritdoc/>
        public void Fatal(string format, params object[] args)
        {
            this.Log.Fatal(string.Format(format, args));
        }

        /// <summary>
        /// Return copy of appender associated with template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private ILog GetNewLogger(string template, string name)
        {
            // TODO Validate parameters & throw Argument exception
            Logger logger = (Logger)LogManager.GetLogger(name).Logger;
            logger.Additivity = false;

            RollingFileAppender templateAppender = this.GetAppender(template) as RollingFileAppender;
            RollingFileAppender newAppender = new RollingFileAppender();

            newAppender.Name = name;
            newAppender.RollingStyle = templateAppender.RollingStyle;
            newAppender.MaxSizeRollBackups = templateAppender.MaxSizeRollBackups;
            newAppender.DatePattern = templateAppender.DatePattern;
            newAppender.StaticLogFileName = templateAppender.StaticLogFileName;
            newAppender.CountDirection = templateAppender.CountDirection;
            newAppender.MaxFileSize = templateAppender.MaxFileSize;

            string path = Path.GetDirectoryName(templateAppender.File);
            string extension = Path.GetExtension(templateAppender.File);
            newAppender.File = Path.Combine(path, name + extension);
      
            if (templateAppender.LockingModel is log4net.Appender.FileAppender.ExclusiveLock)
            {
                newAppender.LockingModel = new log4net.Appender.FileAppender.ExclusiveLock();
            }
            else if (templateAppender.LockingModel is log4net.Appender.FileAppender.InterProcessLock)
            {
                newAppender.LockingModel = new log4net.Appender.FileAppender.InterProcessLock();
            }
            else
            {
                newAppender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
            }
            newAppender.AppendToFile = newAppender.AppendToFile;

            log4net.Layout.PatternLayout pattern = new log4net.Layout.PatternLayout();
            pattern.ConversionPattern = ((log4net.Layout.PatternLayout)templateAppender.Layout).ConversionPattern;
            pattern.ActivateOptions();

            newAppender.Layout = pattern;

            // Active the appender, creates file
            // TODO_MEDIUM Figure out how to avoid exceptions here when switching screens
            logger.AddAppender(newAppender);
            newAppender.ActivateOptions();
            return LogManager.GetLogger(name);
        }

        private IAppender GetAppender(string template)
        {
            // TODO_LOW Validate parameters & throw Argument exception
            Hierarchy hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            IAppender[] appenders = hierarchy.GetAppenders();

            foreach (IAppender appender in appenders)
            {
                if (appender.Name == template)
                {
                    return appender;
                }
            }        
            return null;
        }
    }
}