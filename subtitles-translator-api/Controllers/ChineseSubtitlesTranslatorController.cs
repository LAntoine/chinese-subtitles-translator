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
    public class ChineseSubtitlesTranslatorController : ControllerBase
    {
        private readonly ILogger<ChineseSubtitlesTranslatorController> _logger;
        private readonly IChineseSubtitlesTranslator _chineseSubtitlesTranslator;

        public ChineseSubtitlesTranslatorController(ILogger<ChineseSubtitlesTranslatorController> logger, IChineseSubtitlesTranslator chineseSubtitlesTranslator)
        {
            _logger = logger;
            _chineseSubtitlesTranslator = chineseSubtitlesTranslator;
        }

        [HttpPost]
        public async Task<string> PostAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string textToTranslate = await reader.ReadToEndAsync();
                var result = await _chineseSubtitlesTranslator.TranslateSubtitles(textToTranslate, addPinyin: true, OutputLanguage.French);
                return result;
            }

        }
    }
}
