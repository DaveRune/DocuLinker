using System.Collections.Generic;
using System.IO;
using KnightForge.DocuLinker.DocuLinkTypes;
using UnityEngine;

namespace KnightForge.DocuLinker
{
    internal static class DocuLinkProvider
    {
        private static readonly Dictionary<string, List<DocuLink>> Cache = new();

        public static List<DocuLink> GetDocLinks(string assetPath)
        {
            if (Cache.TryGetValue(assetPath, out var cached))
                return cached;

            var links = Resolve(assetPath);
            Cache[assetPath] = links;
            return links;
        }

        public static void ClearCache() => Cache.Clear();

        private static List<DocuLink> Resolve(string assetPath)
        {
            var fullPath = Path.GetFullPath(assetPath);
            var links = new List<DocuLink>();

            if (!Directory.Exists(fullPath))
                return links;

            var readmeDir = FindDirectory(fullPath, "Readme~");
            if (readmeDir == null)
                return links;

            var linkFile = FindFile(readmeDir, "link.txt");
            if (linkFile != null)
            {
                var url = File.ReadAllText(linkFile).Trim();
                if (!string.IsNullOrEmpty(url))
                    links.Add(new ExternalDocuLink(url));
            }

            var readmeFile = FindFile(readmeDir, "README.md");
            if (readmeFile != null)
                links.Add(new ReadmeDocuLink(readmeFile));

            return links;
        }

        // Case-insensitive directory search within a parent directory.
        private static string FindDirectory(string parentPath, string name)
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(parentPath))
                {
                    if (string.Equals(Path.GetFileName(dir), name, System.StringComparison.OrdinalIgnoreCase))
                        return dir;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{DocuLinkerConstants.ProductName}: Could not search directory '{parentPath}'. {e.Message}");
            }

            return null;
        }

        // Case-insensitive file search within a directory.
        private static string FindFile(string directoryPath, string fileName)
        {
            try
            {
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    if (string.Equals(Path.GetFileName(file), fileName, System.StringComparison.OrdinalIgnoreCase))
                        return file;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{DocuLinkerConstants.ProductName}: Could not search directory '{directoryPath}'. {e.Message}");
            }

            return null;
        }
    }
}
