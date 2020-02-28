using System.Collections.Generic;

namespace subtitles_translator_api.Model
{
    public class Subtitle
    {
        public int Number { get; set; }
        public string Time { get; set; }
        public List<string> Text { get; set; } = new List<string>();
        public List<string> PinyinText { get; set; } = new List<string>();
        public List<string> TranslatedText { get; set; } = new List<string>();
    }
}
