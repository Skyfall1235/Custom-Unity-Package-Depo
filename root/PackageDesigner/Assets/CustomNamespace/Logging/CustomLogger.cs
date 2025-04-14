using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomNamespace.Logging
{
    [CreateAssetMenu(menuName = "Custom Logger")]
    [Serializable]
    /// <summary>
    /// A custom logging system for Unity, allowing for categorized and level-based logging with file output for errors and warnings.
    /// It provides customizable log settings via ScriptableObject, enabling developers to control which log categories are displayed in the console.
    /// Additionally, it includes utility functions for colorizing text and converting colors to hexadecimal strings.
    /// </summary>
    public class CustomLogger : ScriptableObject
    {
        /// <summary>
        /// An array of LogSettings, determining which log categories are displayed.
        /// </summary>
        [SerializeField]
        LogSettings[] m_logSettings = new LogSettings[6];

        /// <summary>
        /// A unique hash value used in log file names to differentiate instances.
        /// </summary>
        static private byte _Hash = 0;

        /// <summary>
        /// Called when the ScriptableObject is loaded or reloaded. Initializes the hash value.
        /// </summary>
        private void OnEnable()
        {
            _Hash = (byte)Mathf.FloorToInt(UnityEngine.Random.value * 256);
        }

        /// <summary>
        /// Logs a message with default severity (Info).
        /// </summary>
        /// <param name="message">The message to log.</param>
        static public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Logs a message with a specified severity.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="severity">The severity of the log message.</param>
        static public void Log(string message, LogLevel severity)
        {
            Log(message, severity, LogCategory.Default);
        }

        /// <summary>
        /// Logs a message with a specified severity and category.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="severity">The severity of the log message.</param>
        /// <param name="category">The category of the log message.</param>
        static public void Log(string message, LogLevel severity, LogCategory category)
        {
            //Redirects to the instance Log method, which handles the actual logging.
            //This static method is a convenience wrapper.
            //Since we cant get a reference to the instance, this static method is not useful.
            //The instance method should be used.
            //Log(message, severity);
        }

        /// <summary>
        /// Logs a message with specified level, category, and sender.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The severity level of the log.</param>
        /// <param name="category">The category of the log message. Defaults to Nulled.</param>
        /// <param name="sender">The object that sent the log message. Defaults to null.</param>
        public void Log(string message, LogLevel level = LogLevel.Info, LogCategory category = LogCategory.Nulled, UnityEngine.Object sender = null)
        {
            int indexOfLogSettings = (int)category;
            if (m_logSettings[indexOfLogSettings].ShowLogs == false) { return; }

            switch (level)
            {
                case LogLevel.Error:
                    Debug.LogError(message, sender);
                    LogErrorToFile(message, sender, category);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message, sender);
                    LogErrorToFile(message, sender, category);
                    break;
                case LogLevel.Info:
                    Debug.Log(message, sender);
                    break;
            }
        }

        #region Static Extensions

        /// <summary>
        /// Colors a text string with the specified color.
        /// </summary>
        /// <param name="text">The text to color.</param>
        /// <param name="color">The color to apply.</param>
        /// <returns>The colored text string.</returns>
        public static string ColorText(string text, Color color)
        {
            string output;
            output = $"<color={ToHex(color)}>{text}</color>";
            return output;
        }

        /// <summary>
        /// Converts a Color to a hexadecimal string.
        /// </summary>
        /// <param name="c">The Color to convert.</param>
        /// <returns>The hexadecimal string representation of the color.</returns>
        public static string ToHex(Color c)
        {
            return string.Format($"#{ToByte(c.r)}{ToByte(c.g)}{ToByte(c.b)}");
        }

        /// <summary>
        /// Converts a float to a byte, clamping the value between 0 and 1.
        /// </summary>
        /// <param name="f">The float to convert.</param>
        /// <returns>The byte representation of the float.</returns>
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        #endregion

        /// <summary>
        /// Logs an error message to a file.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="sender">The object that sent the error message.</param>
        /// <param name="category">The category of the log message. Defaults to Default.</param>
        static public void LogErrorToFile(string message, UnityEngine.Object sender, LogCategory category = LogCategory.Default)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = Application.dataPath + $"/CustomLogs/error_log_{timestamp}_{_Hash}.txt";
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (!File.Exists(filePath))
            {
                // Create a new file if it doesn't exist
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    writer.WriteLine($"[{sender}]"); // Optional: Add a header for the new file
                }
            }

            // Now use File.AppendText to append to the existing or newly created file
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"[{timestamp}] Source: {sender}, Logger: {category}, Scene: {currentSceneName}\n {message}");
                writer.Close();
            }
        }

        /// <summary>
        /// Enumeration of log categories.
        /// </summary>
        [Serializable]
        public enum LogCategory
        {
            System,
            Audio,
            Enemy,
            Player,
            Environment,
            Default,
            Other,
            Nulled
        }

        /// <summary>
        /// Enumeration of log levels.
        /// </summary>
        [Serializable]
        public enum LogLevel
        {
            Error,      //only errors
            Warning,    //only warnings
            Info        //only standard log info
        }

        /// <summary>
        /// Structure representing log settings for a category.
        /// </summary>
        [Serializable]
        private struct LogSettings
        {
            /// <summary>
            /// The category of the log settings.
            /// </summary>
            [SerializeField]
            public LogCategory Category;

            /// <summary>
            /// Whether logs of this category should be displayed.
            /// </summary>
            [SerializeField]
            public bool ShowLogs;

            /// <summary>
            /// Constructor for LogSettings.
            /// </summary>
            /// <param name="category">The log category.</param>
            /// <param name="showLogs">Whether to show logs for this category.</param>
            public LogSettings(LogCategory category, bool showLogs)
            {
                Category = category;
                ShowLogs = showLogs;
            }
        }
    }
}