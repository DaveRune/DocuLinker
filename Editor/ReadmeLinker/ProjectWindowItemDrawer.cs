using System.Collections.Generic;
using KnightForge.ReadmeLinker.DocuLinkTypes;
using UnityEditor;
using UnityEngine;

namespace KnightForge.ReadmeLinker
{
    internal static class ProjectWindowItemDrawer
    {
        private const float IconSize = 16f;
        private const float IconSpacing = 4f;
        private const float IconPadding = 2f;
        private const float GradientWidth = 80f;

        private static readonly Color NormalColor = new(1f, 1f, 1f, 0.2f);
        private static readonly Color HoverColor = new(1f, 1f, 1f, 1f);
        private static readonly Color GradientColor = new(1f, 1f, 1f, 0.15f);

        private static readonly GUIStyle IconStyle = new()
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            normal = { textColor = Color.white }
        };

        private static Texture2D _gradientTexture;
        private static string _hoveredGuid;
        private static int _hoveredIconIndex = -1;

        public static void Draw(string guid, Rect selectionRect)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath))
                return;

            var links = DocLinkProvider.GetDocLinks(assetPath);
            if (links.Count == 0)
                return;

            var iconRects = BuildIconRects(selectionRect, links.Count);
            var mousePosition = Event.current.mousePosition;

            UpdateHoverState(guid, iconRects, mousePosition);

            var isRowHovered = _hoveredGuid == guid;

            if (isRowHovered)
                DrawGradient(selectionRect);

            DrawIcons(links, iconRects, isRowHovered);
            HandleClick(guid, links, iconRects);
        }

        public static void ClearHover() => _hoveredGuid = null;

        private static Rect[] BuildIconRects(Rect selectionRect, int count)
        {
            var rects = new Rect[count];
            var y = selectionRect.y + (selectionRect.height - IconSize) / 2f;

            for (var i = 0; i < count; i++)
            {
                var x = selectionRect.xMax - IconPadding - (IconSize + IconSpacing) * (count - i) + IconSpacing;
                rects[i] = new Rect(x, y, IconSize, IconSize);
            }

            return rects;
        }

        private static void UpdateHoverState(string guid, Rect[] iconRects, Vector2 mousePosition)
        {
            var anyHit = false;

            for (var i = 0; i < iconRects.Length; i++)
            {
                if (!iconRects[i].Contains(mousePosition))
                    continue;

                _hoveredGuid = guid;
                _hoveredIconIndex = i;
                anyHit = true;
                break;
            }

            if (anyHit || _hoveredGuid != guid)
                return;
            
            _hoveredGuid = null;
            _hoveredIconIndex = -1;
        }

        private static void DrawGradient(Rect selectionRect)
        {
            var gradientRect = new Rect(selectionRect.xMax - GradientWidth, selectionRect.y, GradientWidth, selectionRect.height);
            GUI.DrawTexture(gradientRect, GetGradientTexture());
        }

        private static void DrawIcons(List<DocLink> links, Rect[] iconRects, bool isRowHovered)
        {
            for (var i = 0; i < links.Count; i++)
            {
                var isIconHovered = isRowHovered && _hoveredIconIndex == i;
                IconStyle.normal.textColor = isIconHovered ? HoverColor : NormalColor;
                IconStyle.fontStyle = isIconHovered ? FontStyle.Bold : FontStyle.Normal;
                GUI.Label(iconRects[i], links[i].Icon, IconStyle);
            }
        }

        private static void HandleClick(string guid, List<DocLink> links, Rect[] iconRects)
        {
            if (Event.current.type != EventType.MouseDown || Event.current.button != 0)
                return;

            if (_hoveredGuid != guid)
                return;

            for (var i = 0; i < iconRects.Length; i++)
            {
                if (!iconRects[i].Contains(Event.current.mousePosition))
                    continue;

                Event.current.Use();
                links[i].Open();
                return;
            }
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
    }
}
