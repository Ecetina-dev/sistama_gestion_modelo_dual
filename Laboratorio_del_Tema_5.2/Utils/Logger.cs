using System;
using System.Collections.Generic;
using System.IO;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Logger simple que escribe a archivo.
    /// </summary>
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string _logDirectory;
        private static readonly string _logFileName;

        static Logger()
        {
            // Crear directorio de logs si no existe
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            // Nombre del archivo con fecha
            _logFileName = $"app_{DateTime.Now:yyyyMMdd}.log";
        }

        /// <summary>
        /// Obtiene la ruta completa del archivo de log actual.
        /// </summary>
        public static string GetLogFilePath()
        {
            return Path.Combine(_logDirectory, _logFileName);
        }

        /// <summary>
        /// Escribe un mensaje de log de informacion.
        /// </summary>
        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// Escribe un mensaje de log de advertencia.
        /// </summary>
        public static void Warning(string message)
        {
            WriteLog("WARN", message);
        }

        /// <summary>
        /// Escribe un mensaje de log de error.
        /// </summary>
        public static void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        /// <summary>
        /// Escribe un mensaje de log de error con excepcion.
        /// </summary>
        public static void Error(string message, Exception ex)
        {
            WriteLog("ERROR", $"{message}\nException: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }

        /// <summary>
        /// Escribe un mensaje de log de debug (solo en modo Debug).
        /// </summary>
        public static void Debug(string message)
        {
#if DEBUG
            WriteLog("DEBUG", message);
#endif
        }

        /// <summary>
        /// Escribe un mensaje de log personalizado.
        /// </summary>
        public static void Log(string level, string message)
        {
            WriteLog(level, message);
        }

        /// <summary>
        /// Escribe una linea en el archivo de log de forma segura (thread-safe).
        /// </summary>
        private static void WriteLog(string level, string message)
        {
            lock (_lock)
            {
                try
                {
                    string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
                    File.AppendAllText(GetLogFilePath(), logLine + Environment.NewLine);
                    
                    // Also write to console in debug mode
#if DEBUG
                    Console.WriteLine(logLine);
#endif
                }
                catch (Exception ex)
                {
                    // Si falla el logging, no throw para no afectar la app
                    Console.WriteLine($"Error al escribir log: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Limpia los logs antiguos (mas de X dias).
        /// </summary>
        public static void CleanOldLogs(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var logFiles = Directory.GetFiles(_logDirectory, "*.log");

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoffDate)
                    {
                        File.Delete(file);
                        Info($"Log antiguo eliminado: {fileInfo.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Error("Error al limpiar logs antiguos", ex);
            }
        }
    }
}