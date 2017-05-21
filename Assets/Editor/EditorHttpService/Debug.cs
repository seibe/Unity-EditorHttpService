
//#define EDITORSERVICE_ENABLE_PRINT
#define EDITORSERVICE_ENABLE_EXCEPTION

using System;

namespace EditorHttpService
{
    static class Debug
    {
        //[System.Diagnostics.Conditional("EDITORSERVICE_ENABLE_PRINT")]
        public static void Print(string message)
        {
#if EDITORSERVICE_ENABLE_PRINT
            UnityEngine.Debug.Log(message);
#endif
        }

        //[System.Diagnostics.Conditional("EDITORSERVICE_ENABLE_PRINT")]
        public static void PrintFormat(string format, params object[] args)
        {
#if EDITORSERVICE_ENABLE_PRINT
            UnityEngine.Debug.LogFormat(format, args);
#endif
        }

        //[System.Diagnostics.Conditional("EDITORSERVICE_ENABLE_EXCEPTION")]
        public static void Exception(Exception exception)
        {
#if EDITORSERVICE_ENABLE_EXCEPTION
            UnityEngine.Debug.LogException(exception);
#endif
        }
    }
}
