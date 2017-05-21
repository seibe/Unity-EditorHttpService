
using System.Reflection;
using UnityEditor;

namespace EditorService
{
    static class CompileErrorChecker
    {
        public static void ForceCompile()
        {
            _editorUtilityRequestScriptReload.Invoke(null, null);
            var views = SceneView.sceneViews;
            if (views.Count > 0) ((SceneView)views[0]).Focus();
        }

        public static bool HasError()
        {
            _logEntriesClear.Invoke(null, null);
            var hasError = ((int)_logEntriesGetCount.Invoke(null, null) > 0);
            return hasError;
        }

        static CompileErrorChecker()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));

            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
            _logEntriesClear = logEntries.GetMethod("Clear");
            _logEntriesGetCount = logEntries.GetMethod("GetCount");

            var editorUtility = assembly.GetType("UnityEditorInternal.InternalEditorUtility");
            _editorUtilityRequestScriptReload = editorUtility.GetMethod("RequestScriptReload");
        }

        private static readonly MethodInfo _logEntriesClear;
        private static readonly MethodInfo _logEntriesGetCount;
        private static readonly MethodInfo _editorUtilityRequestScriptReload;
    }
}
