namespace subtitles_translator_api.Model
{
    public class AzureTranslatorRequestBodyElement
    {
        public string Text;

        public AzureTranslatorRequestBodyElement(string text)
        {
            Text = text;
        }
    }
}
