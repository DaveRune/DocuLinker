namespace KnightForge.ReadmeLinker.DocuLinkTypes
{
    internal sealed class ReadmeDocLink : DocLink
    {
        private readonly string _filePath;

        public ReadmeDocLink(string filePath) => _filePath = filePath;

        public override string Icon => "?";
        public override void Open() => EditorLauncher.OpenFile(_filePath);
    }
}