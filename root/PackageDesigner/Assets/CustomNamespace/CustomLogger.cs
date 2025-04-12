using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Custom Logger")]
[Serializable]
public class CustomLogger : ScriptableObject
{
    [SerializeField]
    LogSettings[] m_logSettings = new LogSettings[6];
    static private byte _Hash = 0;

    private void OnEnable()
    {
        _Hash = (byte)Mathf.FloorToInt(UnityEngine.Random.value * 256);
    }


    static public void Log(string message)
    {
        Log(message);
    }

    static public void Log(string message, LogLevel severity)
    {
        Log(message, severity);
    }

    static public void Log(string message, LogLevel severity, LogCategory category)
    {
        Log(message, severity);
    }


    public void Log(string message, LogLevel level = LogLevel.Info, LogCategory category = LogCategory.Nulled, UnityEngine.Object sender = null)
    {
        int indexOfLogSettings = (int)category;
        if (!m_logSettings[indexOfLogSettings].ShowLogs) { return; }

        switch(level)
        {
            case LogLevel.Error:
                Debug.LogError(message, sender);
                LogErrorToFile(message, sender);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(message, sender);
                LogErrorToFile(message, sender);
                break;
            case LogLevel.Info:
                Debug.Log(message, sender);
                break;
        }
    }

    #region Static Extensions

    public static string ColorText(string text, Color color)
    {
        string output;
        output = $"<color={ToHex(color)}>{text}</color>";
        return output;
    }

    public static string ToHex(Color c)
    {
        return string.Format($"#{ToByte(c.r)}{ToByte(c.g)}{ToByte(c.b)}");
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    #endregion

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

    [Serializable]
    public enum LogLevel
    {
        Error,   //only errors
        Warning,  //only warnings
        Info   //only standard log info
    }

    [Serializable]
    private struct LogSettings
    {
        [SerializeField]
        public LogCategory Category;
        [SerializeField]
        public bool ShowLogs;

        public LogSettings(LogCategory category, bool showLogs)
        {
            this.Category = category;
            this.ShowLogs = showLogs;
        }
    }



}




