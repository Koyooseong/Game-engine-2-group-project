using UnityEngine;

public static class Log
{
    public static bool IsDebugEnabled = true;

    public static void Info(string message, Object context = null)
    {
#if UNITY_EDITOR
        if (IsDebugEnabled)
            Debug.Log($"<color=cyan>[INFO]</color> {message}", context);
#endif
    }

    public static void Warn(string message, Object context = null)
    {
#if UNITY_EDITOR
        Debug.LogWarning($"<color=yellow>[WARN]</color> {message}", context);
#endif
    }

    public static void Error(string message, Object context = null)
    {
#if UNITY_EDITOR
        Debug.LogError($"<color=red>[ERROR]</color> {message}", context);
#endif
    }

    public static void System(string message, Object context = null)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=magenta>[SYSTEM]</color> {message}", context);
#endif
    }
}
