using System.IO;
using UnityEditor;
using UnityEngine;

namespace KnightForge.DocuLinker
{
    internal static class EditorLauncher
    {
        public static void OpenFile(string filePath)
        {
            var editorPath = EditorPrefs.GetString("kScriptsDefaultApp");

            if (!string.IsNullOrEmpty(editorPath) && File.Exists(editorPath))
            {
                var args = $"\"{filePath}\"";

                if (editorPath.Contains("rider", System.StringComparison.OrdinalIgnoreCase))
                    args = $"--line 1 \"{filePath}\"";
                else if (editorPath.Contains("code", System.StringComparison.OrdinalIgnoreCase))
                    args = $"-g \"{filePath}:1\"";

                System.Diagnostics.Process.Start(editorPath, args);
                return;
            }

            EditorUtility.OpenWithDefaultApp(filePath);
        }

        public static void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}
