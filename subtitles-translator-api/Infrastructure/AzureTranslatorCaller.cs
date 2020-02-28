using RestSharp;
using subtitles_translator_api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace subtitles_translator_api.Infrastructure
{
    public interface ITranslatorCaller
    {
        Task<IEnumerable<string>> Translate(IEnumerable<string> textToTranslate, OutputLanguage outputLanguage);
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
            OutputLanguage outputLanguage)
        {
            string languageCode;
            string resource;

            switch (outputLanguage)
            {
                case OutputLanguage.French:
                    resource = "translate";
                    languageCode = "fr";
                    break;
                case OutputLanguage.Pinyin:
                    resource = "transliterate";
                    languageCode = "Latn";
                    break;
                case OutputLanguage.English:
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
    }
}