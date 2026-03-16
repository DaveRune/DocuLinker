using KnightForge.DocuLinker;
using UnityEditor;
using UnityEngine;

namespace KnightForge.DocuLinker
{
    [InitializeOnLoad]
    internal static class DocuLinkerInitializer
    {
        private static EditorWindow _lastProjectWindow;

        static DocuLinkerInitializer()
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
            DocuLinkProvider.ClearCache();
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
