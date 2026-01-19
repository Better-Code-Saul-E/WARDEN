namespace Warden.CLI.Domain.Rules
{
    public static class FileCategoryRules
    {
        private static readonly Dictionary<string, List<string>> _categoryDefinitions = new()
        {
            { "Images",      new List<string> { "jpg", "jpeg", "png", "gif", "bmp", "svg", "webp", "ico" } },
            { "Documents",   new List<string> { "pdf", "docx", "doc", "txt", "md", "xlsx", "csv", "pptx" } },
            { "Audio",       new List<string> { "mp3", "wav", "flac", "ogg", "aac" } },
            { "Video",       new List<string> { "mp4", "mkv", "avi", "mov", "wmv" } },
            { "Archives",    new List<string> { "zip", "rar", "7z", "tar", "gz" } },
            { "Executables", new List<string> { "exe", "msi", "bat", "sh", "iso" } },
            { "Code",        new List<string> { "cs", "py", "js", "json", "xml", "html", "css", "sql" } }
        };

        private static readonly Dictionary<string, string> _extensions;

        static FileCategoryRules()
        {
            _extensions = new Dictionary<string, string>();

            foreach (var category in _categoryDefinitions)
            {
                foreach (var extension in category.Value)
                {
                    if (!_extensions.ContainsKey(extension))
                    {
                        _extensions.Add(extension, category.Key);
                    }
                }
            }
        }

        public static string GetCategory(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return "_Other";
            }

            var cleanedExtension = extension.TrimStart('.').ToLowerInvariant();

            if(_extensions.TryGetValue(cleanedExtension, out var category))
            {
                return category;
            }

            return "_Other";
        }
    }
}