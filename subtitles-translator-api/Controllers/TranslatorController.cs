using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using subtitles_translator_api.Model;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace subtitles_translator_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslatorController : ControllerBase
    {
        private readonly ILogger<TranslatorController> _logger;
        private readonly IChineseSubtitlesTranslator _chineseSubtitlesTranslator;
        private readonly IMinecraftLanguagePackTranslator _minecraftLanguagePackTranslator;

        public TranslatorController(ILogger<TranslatorController> logger, IChineseSubtitlesTranslator chineseSubtitlesTranslator, IMinecraftLanguagePackTranslator minecraftLanguagePackTranslator)
        {
            _logger = logger;
            _chineseSubtitlesTranslator = chineseSubtitlesTranslator;
            _minecraftLanguagePackTranslator = minecraftLanguagePackTranslator;
        }

        [HttpPost]
        public async Task<string> PostSubtitleAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string textToTranslate = await reader.ReadToEndAsync();
                var result = await _chineseSubtitlesTranslator.TranslateSubtitles(textToTranslate, addPinyin: true, Language.French);
                return result;
            }
        }

        [HttpPost]
        [Route("translate-minecraft-language-pack/{inputLanguage}/{outputLanguage}")]
        public async Task<MinecraftLanguagePack> PostMinecraftLanguagePackAsync(Language inputLanguage, Language outputLanguage, [FromBody] MinecraftLanguagePack minecraftLanguagePack)
        {
            var result = await _minecraftLanguagePackTranslator.Translate(minecraftLanguagePack, inputLanguage, outputLanguage);
            return result;
        }
    }
}
