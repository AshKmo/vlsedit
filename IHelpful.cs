namespace VLSEdit
{
    public interface IHelpful
    {
        public string HelpTitle { get; }

        public string HelpContent { get; }

        public void ShowHelp()
        {
            GUIPrompt.ShowHelp(HelpTitle, HelpContent);
        }
    }
}