using subtitles_translator_api.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace subtitles_translator_api.Model
{
    public interface IChineseSubtitlesTranslator
    {
        Task<string> TranslateSubtitles(string textToTranslate, bool addPinyin, Language outputLanguage);
        Task<Subtitle> Translate(Subtitle subtitle, bool addPinyin, Language outputLanguage);
    }

    public class ChineseSubtitlesTranslator : IChineseSubtitlesTranslator
    {
        private readonly ITranslatorCaller _translatorCaller;

        public ChineseSubtitlesTranslator(ITranslatorCaller translatorCaller)
        {
            _translatorCaller = translatorCaller;
        }

        public async Task<string> TranslateSubtitles(string textToTranslate, bool addPinyin,
            Language outputLanguage)
        {
            var splitedTextToTranslate = textToTranslate.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
            var listSubtitles = new List<Subtitle>();
            for (int i = 0; i < splitedTextToTranslate.Length; i++)
            {
                int subtitleNumber;
                if (int.TryParse(splitedTextToTranslate[i], out subtitleNumber))
                {
                    var subtitle = new Subtitle() { Number = subtitleNumber };
                    subtitle.Time = splitedTextToTranslate[++i];
                    var subtitleText = splitedTextToTranslate[++i];
                    while (!String.IsNullOrWhiteSpace(subtitleText))
                    {
                        subtitle.Text.Add(subtitleText);
                        subtitleText = splitedTextToTranslate[++i];
                    }
                    listSubtitles.Add(subtitle);
                }
            }

            //var tasksGetPinyin = listSubtitles.Select(subtitle =>
            //    _translatorCaller.Translate(subtitle.Text, OutputLanguage.Pinyin)
            //    );

            //var tasksGetTranslation = listSubtitles.Select(subtitle =>
            //    (subtitle.Number,
            //        _translatorCaller.Translate(subtitle.Text, outputLanguage)
            //    ));

            var tasks = listSubtitles.Select(subtitle =>
                Translate(subtitle, addPinyin:true, Language.English)
            );


            var listSubtitlesTranslated = await Task.WhenAll(tasks);

            var result = "";
            foreach (var subtitle in listSubtitlesTranslated)
            {
                result += subtitle.Number + "\n" +
                          subtitle.Time + "\n" +
                          string.Join('\n', subtitle.Text) + "\n" +
                          string.Join('\n', subtitle.PinyinText) + "\n" +
                          string.Join('\n', subtitle.TranslatedText) + "\n" +
                          "\n";
            }


            return result;
        }

        public async Task<Subtitle> Translate(Subtitle subtitle, bool addPinyin, Language outputLanguage)
        {
            if (addPinyin)
            {
                var pinyin = await _translatorCaller.Translate(subtitle.Text, Language.Pinyin);
                subtitle.PinyinText = pinyin.ToList();
            }
            var translations = await _translatorCaller.Translate(subtitle.Text, outputLanguage);
            subtitle.TranslatedText = translations.ToList();
            return subtitle;
        }
        
    }
}
