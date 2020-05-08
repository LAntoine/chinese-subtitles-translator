using RestSharp;
using RestSharp.Serialization.Json;
using subtitles_translator_api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace subtitles_translator_api.Infrastructure
{
    public interface ITranslatorCaller
    {
        Task<IEnumerable<string>> Translate(IEnumerable<string> textToTranslate, Language outputLanguage);
        Task<List<string>> Translate(List<string> textToTranslate, Language inputLanguage, Language outputLanguage);
    }

    public class AzureTranslatorCaller : ITranslatorCaller
    {
        private readonly string _microsoftAzureKey;
        private readonly RestClient _restClient;
        private readonly string _apiVersion = "3.0";

        public AzureTranslatorCaller(string microsoftAzureKey)
        {
            _microsoftAzureKey = microsoftAzureKey;
            _restClient = new RestClient("https://api.cognitive.microsofttranslator.com/");
        }

        public async Task<IEnumerable<string>> Translate(IEnumerable<string> textToTranslate,
            Language outputLanguage)
        {
            string languageCode;
            string resource;

            switch (outputLanguage)
            {
                case Language.French:
                    resource = "translate";
                    languageCode = "fr";
                    break;
                case Language.Pinyin:
                    resource = "transliterate";
                    languageCode = "Latn";
                    break;
                case Language.English:
                    resource = "translate";
                    languageCode = "en";
                    break;
                default:
                    throw new InvalidOperationException("Language not handled");
            }

            var requestBody = textToTranslate.Select(text => new AzureTranslatorRequestBodyElement(text)).ToArray();

            if (resource.Equals("translate"))
            {
                var request = new RestRequest($"translate?api-version={_apiVersion}&to={languageCode}&from=zh-Hans");
                request.AddHeader("Ocp-Apim-Subscription-Key", _microsoftAzureKey);
                request.AddJsonBody(requestBody);
                var response = await _restClient.PostAsync<List<AzureTranslatorTranslateResponse>>(request);
                return response.First().Translations.Select(translation => translation.Text);
            }
            else
            {
                var request = new RestRequest($"transliterate?api-version={_apiVersion}&fromscript=Hans&language=zh-Hans&toscript={languageCode}");
                request.AddHeader("Ocp-Apim-Subscription-Key", _microsoftAzureKey);
                request.AddJsonBody(requestBody);
                var response = await _restClient.PostAsync<List<AzureTranslatorTranslation>>(request);
                return response.Select(translation => translation.Text);
            }
        }

        public async Task<List<string>> Translate(List<string> textToTranslate, Language inputLanguage, Language outputLanguage)
        {
            var (inputLanguageResource, inputLanguageCode) = GetResourceAndLanguageCode(inputLanguage);
            var (outputLanguageResource, outputLanguageCode) = GetResourceAndLanguageCode(outputLanguage);

            var requestBody = textToTranslate.Select(text => new AzureTranslatorRequestBodyElement(text));

            var chunkedRequestBody = requestBody.ChunkBy(10);
            var chunksizes = chunkedRequestBody.Select(chunk => chunk.Sum(translation => translation.Text.Length));
            var listToReturn = new List<string>();

            foreach(var chunk in chunkedRequestBody)
            {
                if (outputLanguageResource.Equals("translate"))
                {
                    var request = new RestRequest($"translate?api-version={_apiVersion}&to={outputLanguageCode}&from={inputLanguageCode}");
                    request.AddHeader("Ocp-Apim-Subscription-Key", _microsoftAzureKey);
                    request.AddJsonBody(chunk);
                    var response = await _restClient.PostAsync<List<AzureTranslatorTranslateResponse>>(request);
                    listToReturn.AddRange(response.First().Translations.Select(translation => translation.Text));
                }
                else
                {
                    var request = new RestRequest($"transliterate?api-version={_apiVersion}&fromscript=Hans&language=zh-Hans&toscript={outputLanguageCode}");
                    request.AddHeader("Ocp-Apim-Subscription-Key", _microsoftAzureKey);
                    request.AddJsonBody(chunk);
                    var rawResponse = _restClient.Post(request);
                    var deserial = new JsonDeserializer();
                    var response = deserial.Deserialize<List<AzureTranslatorTranslation>>(rawResponse);
                    listToReturn.AddRange(response.Select(translation => translation.Text));
                }
            }
            return listToReturn;
        }

        private (string, string) GetResourceAndLanguageCode(Language language)
        {
            string resource;
            string languageCode;
            switch (language)
            {
                case Language.French:
                    resource = "translate";
                    languageCode = "fr";
                    break;
                case Language.Pinyin:
                    resource = "transliterate";
                    languageCode = "Latn";
                    break;
                case Language.English:
                    resource = "translate";
                    languageCode = "en";
                    break;
                case Language.Mandarin:
                    resource = "translate";
                    languageCode = "zh-Hans";
                    break;
                default:
                    throw new InvalidOperationException("Language not handled");
            }
            return (resource, languageCode);
    }
}

    public static class IEnumerable
    {
        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}