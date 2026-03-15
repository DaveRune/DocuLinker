namespace KnightForge.ReadmeLinker.DocuLinkTypes
{
    internal sealed class ExternalDocLink : DocLink
    {
        private readonly string _url;

        public ExternalDocLink(string url) => _url = url;

        public override string Icon => "↗";
        public override void Open() => EditorLauncher.OpenUrl(_url);
    }
}