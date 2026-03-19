using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KnightForge.DocuLinker
{
    internal static class CreateDocuLinkMenuItems
    {
        private const string ReadmeDirName = "Readme~";
        private const string ReadmeFileName = "README.md";
        private const string LinkFileName = "link.txt";
        private const string LinkDefaultContent = "www.your-url-goes-here.com/example";
        private const string ReadmeDefaultContent = "Create your documentation here.\n\nThe Documentation is associated with the containing folder \"{0}\"";

        [MenuItem("Assets/Create/DocuLinker/Local Readme", priority = 200)]
        private static void CreateLocalReadme()
        {
            CreateDocuLinkFile(ReadmeFileName, "Documentation");
        }

        [MenuItem("Assets/Create/DocuLinker/Local Readme", validate = true)]
        private static bool ValidateCreateLocalReadme() => Selection.activeObject != null;

        [MenuItem("Assets/Create/DocuLinker/External Link", priority = 201)]
        private static void CreateExternalLink()
        {
            CreateDocuLinkFile(LinkFileName, "Link");
        }

        [MenuItem("Assets/Create/DocuLinker/External Link", validate = true)]
        private static bool ValidateCreateExternalLink() => Selection.activeObject != null;

        private static void CreateDocuLinkFile(string fileName, string label)
        {
            var result = GetSelectedFolderPaths();
            if (result == null)
            {
                Debug.LogWarning("DocuLinker: Could not resolve a target folder from the current selection.");
                return;
            }

            var (fullPath, assetFolderPath) = result.Value;
            var readmeDir = Path.Combine(fullPath, ReadmeDirName);
            var filePath = Path.Combine(readmeDir, fileName);

            if (File.Exists(filePath))
            {
                Debug.Log($"DocuLinker: {label} already exists at '{assetFolderPath}'.");
                EditorLauncher.OpenFile(filePath);
                return;
            }

            var defaultContent = BuildDefaultContent(fileName, assetFolderPath);

            Directory.CreateDirectory(readmeDir);
            File.WriteAllText(filePath, defaultContent);

            DocuLinkProvider.ClearCache();
            EditorApplication.RepaintProjectWindow();

            EditorLauncher.OpenFile(filePath);
        }

        private static string BuildDefaultContent(string fileName, string assetFolderPath)
        {
            if (fileName != ReadmeFileName)
                return LinkDefaultContent;
            
            var folderName = Path.GetFileName(assetFolderPath);
            return string.Format(ReadmeDefaultContent, folderName);
        }

        private static (string fullPath, string assetFolderPath)? GetSelectedFolderPaths()
        {
            var activeObject = Selection.activeObject;
            if (!activeObject) return null;

            var assetPath = AssetDatabase.GetAssetPath(activeObject);
            if (string.IsNullOrEmpty(assetPath)) return null;

            var fullPath = Path.GetFullPath(assetPath);

            if (Directory.Exists(fullPath))
                return (fullPath, assetPath);

            var lastSlash = assetPath.LastIndexOf('/');
            var folderAssetPath = lastSlash >= 0 ? assetPath[..lastSlash] : assetPath;

            return (Path.GetDirectoryName(fullPath), folderAssetPath);
        }
    }
}
