namespace KnightForge.DocuLinker.DocuLinkTypes
{
    internal sealed class ReadmeDocuLink : DocuLink
    {
        private readonly string _filePath;

        public ReadmeDocuLink(string filePath) => _filePath = filePath;

        public override string Icon => "?";
        public override void Open() => EditorLauncher.OpenFile(_filePath);
    }
}