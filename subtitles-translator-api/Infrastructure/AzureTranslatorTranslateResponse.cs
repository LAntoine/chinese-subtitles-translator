using System.Collections.Generic;

namespace subtitles_translator_api.Model
{
    public class AzureTranslatorTranslateResponse
    {
        public List<AzureTranslatorTranslation> Translations { get; set; }
    }

    public class AzureTranslatorTranslation
    {
        public string Text { get; set; }
        public string To { get; set; }
        public string Script { get; set; }
    }
}
