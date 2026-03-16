namespace KnightForge.DocuLinker.DocuLinkTypes
{
    internal sealed class ExternalDocuLink : DocuLink
    {
        private readonly string _url;

        public ExternalDocuLink(string url) => _url = url;

        public override string Icon => "↗";
        public override void Open() => EditorLauncher.OpenUrl(_url);
    }
}