using subtitles_translator_api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace subtitles_translator_api.Model
{
    public interface IMinecraftLanguagePackTranslator
    {
        Task<MinecraftLanguagePack> Translate(MinecraftLanguagePack languagePack, Language inputLanguage,
            Language outputLanguage);
    }

    public class MinecraftLanguagePackTranslator : IMinecraftLanguagePackTranslator
    {
        private readonly ITranslatorCaller _translatorCaller;

        public MinecraftLanguagePackTranslator(ITranslatorCaller translatorCaller)
        {
            _translatorCaller = translatorCaller;
        }

        public async Task<MinecraftLanguagePack> Translate(MinecraftLanguagePack languagePack, Language inputLanguage,
            Language outputLanguage)
        {
            var originalTranslations = languagePack.translations.Values.Distinct().ToList();
            var resultOutputLanguage = await _translatorCaller.Translate(originalTranslations, inputLanguage, outputLanguage);

            var translations = originalTranslations.ToDictionary(x => x, x => resultOutputLanguage[originalTranslations.IndexOf(x)]);

            var newLanguagePack = new MinecraftLanguagePack();
            foreach(var translation in languagePack.translations)
            {
                var key = translation.Key;
                var value = translation.Value;
                newLanguagePack.translations.Add(key, $"{value} ({translations[value]})");
            }

            return newLanguagePack;
        }
        
    }
}
