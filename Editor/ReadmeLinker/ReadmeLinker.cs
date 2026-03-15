#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KnightForge
{
    [InitializeOnLoad]
    public static class ReadmeLinker
    {
        private static readonly Color GradientColor = new(1f, 1f, 1f, 0.15f);
        private static readonly Color NormalColor = new(1f, 1f, 1f, 0.2f);
        private static readonly Color HoverColor = new(1f, 1f, 1f, 1f);
        private static readonly Dictionary<string, bool> ReadmeCache = new();
        private static readonly GUIStyle IconStyle;

        private static EditorWindow _lastProjectWindow;
        private static Texture2D _gradientTexture;
        private static string _hoveredGuid;

        static ReadmeLinker()
        {
            IconStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                normal = { textColor = Color.white }
            };

            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
            EditorApplication.projectChanged += ClearCache;
            EditorApplication.update += EnsureMouseMoveEnabled;
        }

        private static void EnsureMouseMoveEnabled()
        {
            var mouseOverWindow = EditorWindow.mouseOverWindow;
            if (!mouseOverWindow)
                return;

            // Check if it's a Project window (ProjectBrowser)
            var windowType = mouseOverWindow.GetType().Name;
            if (windowType != "ProjectBrowser")
                return;

            if (_lastProjectWindow != mouseOverWindow)
            {
                _lastProjectWindow = mouseOverWindow;
                mouseOverWindow.wantsMouseMove = true;
            }

            // Force repaint while mouse is over the project window for responsive hover
            if (_hoveredGuid != null || mouseOverWindow.wantsMouseMove)
            {
                mouseOverWindow.Repaint();
            }
        }

        private static void ClearCache()
        {
            ReadmeCache.Clear();
            _hoveredGuid = null;
        }

        private static bool HasReadme(string assetPath)
        {
            if (ReadmeCache.TryGetValue(assetPath, out var hasReadme))
                return hasReadme;

            var fullPath = Path.GetFullPath(assetPath);

            if (!Directory.Exists(fullPath))
            {
                ReadmeCache[assetPath] = false;
                return false;
            }

            var readmePath = Path.Combine(fullPath, "Readme~", "README.md");
            hasReadme = File.Exists(readmePath);
            ReadmeCache[assetPath] = hasReadme;

            return hasReadme;
        }

        private static string GetReadmePath(string assetPath)
        {
            var fullPath = Path.GetFullPath(assetPath);
            return Path.Combine(fullPath, "Readme~", "README.md");
        }

        private static Texture2D GetGradientTexture()
        {
            if (_gradientTexture)
                return _gradientTexture;

            _gradientTexture = new Texture2D(64, 1, TextureFormat.RGBA32, false);

            for (var x = 0; x < 64; x++)
            {
                var t = x / 63f;
                var alpha = t * t * GradientColor.a;
                _gradientTexture.SetPixel(x, 0, new Color(1f, 1f, 1f, alpha));
            }

            _gradientTexture.Apply();
            _gradientTexture.wrapMode = TextureWrapMode.Clamp;

            return _gradientTexture;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath) || !HasReadme(assetPath))
                return;

            var iconSize = 16f;
            var iconRect = new Rect(
                selectionRect.xMax - iconSize - 2,
                selectionRect.y + (selectionRect.height - iconSize) / 2,
                iconSize,
                iconSize
            );

            var mousePosition = Event.current.mousePosition;
            var isHovered = iconRect.Contains(mousePosition);

            // Update hover state
            if (isHovered)
            {
                _hoveredGuid = guid;
            }
            else if (_hoveredGuid == guid)
            {
                _hoveredGuid = null;
            }

            var isCurrentlyHovered = _hoveredGuid == guid;

            // Draw gradient background on hover
            if (isCurrentlyHovered)
            {
                var gradientRect = new Rect(
                    selectionRect.xMax - 60,
                    selectionRect.y,
                    60,
                    selectionRect.height
                );
                GUI.DrawTexture(gradientRect, GetGradientTexture());
            }

            // Draw the "?" icon
            var color = isCurrentlyHovered ? HoverColor : NormalColor;
            IconStyle.normal.textColor = color;
            IconStyle.fontStyle = isCurrentlyHovered ? FontStyle.Bold : FontStyle.Normal;

            GUI.Label(iconRect, "?", IconStyle);

            // Handle click
            if (isCurrentlyHovered && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Event.current.Use();
                var readmePath = GetReadmePath(assetPath);
                OpenInExternalEditor(readmePath);
            }
        }

        private static void OpenInExternalEditor(string filePath)
        {
            var editorPath = EditorPrefs.GetString("kScriptsDefaultApp");

            if (!string.IsNullOrEmpty(editorPath) && File.Exists(editorPath))
            {
                var args = $"\"{filePath}\"";

                // Rider prefers --line format
                if (editorPath.Contains("rider", System.StringComparison.OrdinalIgnoreCase))
                {
                    args = $"--line 1 \"{filePath}\"";
                }
                // VS Code uses -g file:line format
                else if (editorPath.Contains("code", System.StringComparison.OrdinalIgnoreCase))
                {
                    args = $"-g \"{filePath}:1\"";
                }

                System.Diagnostics.Process.Start(editorPath, args);
                return;
            }

            // Fallback to system default
            EditorUtility.OpenWithDefaultApp(filePath);
        }
    }
}
#endif
