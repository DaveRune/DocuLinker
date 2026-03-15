using UnityEditor;
using UnityEngine;

namespace KnightForge.ReadmeLinker
{
    [InitializeOnLoad]
    internal static class ReadmeLinkerInitializer
    {
        private static EditorWindow _lastProjectWindow;

        static ReadmeLinkerInitializer()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
            EditorApplication.projectChanged += OnProjectChanged;
            EditorApplication.update += EnsureMouseMoveEnabled;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            ProjectWindowItemDrawer.Draw(guid, selectionRect);
        }

        private static void OnProjectChanged()
        {
            DocLinkProvider.ClearCache();
            ProjectWindowItemDrawer.ClearHover();
        }

        private static void EnsureMouseMoveEnabled()
        {
            var mouseOverWindow = EditorWindow.mouseOverWindow;
            if (!mouseOverWindow)
                return;

            if (mouseOverWindow.GetType().Name != "ProjectBrowser")
                return;

            if (_lastProjectWindow != mouseOverWindow)
            {
                _lastProjectWindow = mouseOverWindow;
                mouseOverWindow.wantsMouseMove = true;
            }

            mouseOverWindow.Repaint();
        }
    }
}
