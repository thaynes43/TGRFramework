using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlotMachine.Prototype.Tools
{
    /// <summary>
    /// log4net log wrapper
    /// </summary>
    public interface ILogTool
    {
        /// <summary>
        /// Log Info Level
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">objects specified in format</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// Log Debug Level
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">objects specified in format</param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// Log Warn Level
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">objects specified in format</param>
        void Warn(string format, params object[] args);

        /// <summary>
        /// Log Error Level
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">objects specified in format</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// Log Fatal Level
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">objects specified in format</param>
        void Fatal(string format, params object[] args);
    }
}
