using System;
using System.Collections.Generic;

namespace subtitles_translator_api.Model
{
    public class MinecraftLanguagePack
    {
        public Dictionary<string, string> translations { get; set; } = new Dictionary<string, string>();
    }
}
