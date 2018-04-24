using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JinShanSDK
{
    public class JinShanUtil
    {
        private static readonly string url = "http://dict-co.iciba.com/api/dictionary.php?";
        private static readonly string key = "096311D10366D29EE0C7ED7A60C6CE9E";
        private static readonly string type = "json";
        private static readonly string sentenceUrl = "http://open.iciba.com/dsapi/";

        public static JinShanDomain Translate(string word)
        {
            var fullUrl = url + "w=" + HttpUtility.UrlEncode(word, Encoding.UTF8) + "&type=" + type + "&key=" + key;
            WebClient client = new WebClient();
            var jsonCallback = client.DownloadString(fullUrl);
            var result = ConvertJsonToDomain(jsonCallback);
            return result;
        }

        private static JinShanDomain ConvertJsonToDomain(string json)
        {
            JinShanDomain result = null;
            if (json.Contains("part_name"))
            {
                result = new JinShanDomain();
                var domain = JsonConvert.DeserializeObject<JinShanSentenceDomain>(json);
                result.word_name = domain.word_name;
                result.wordFlag = false;
                result.symbols = new List<symbols>();
                foreach (var symbol in domain.symbols)
                {
                    var partsTemp = new List<parts>();
                    foreach (var part in symbol.parts)
                    {
                        var meansTemp = new List<string>(); 
                        foreach (var mean in part.means)
                        {
                            meansTemp.Add(mean.word_mean);
                        }
                        var partTemp = new parts()
                        {
                            means = meansTemp
                        };
                        partsTemp.Add(partTemp);
                    }
                    result.symbols.Add(new symbols
                    {
                        ph_en = symbol.word_symbol,
                        parts = partsTemp
                    });
                }
            }
            else
            {
                result = JsonConvert.DeserializeObject<JinShanDomain>(json);
                result.wordFlag = true;
            }
            return result;
        }

        public static SentenceDomain GetSentence()
        {
            WebClient client = new WebClient();
            var jsonCallback = client.DownloadString(sentenceUrl);
            var result = JsonConvert.DeserializeObject<SentenceDomain>(jsonCallback);
            return result;
        }

        public static VoiceAndPH GetVoiceUrl(string word)
        {
            var info = new VoiceAndPH();
            try
            {
                var fullUrl = url + "w=" + HttpUtility.UrlEncode(word, Encoding.UTF8) + "&type=" + type + "&key=" + key;
                WebClient client = new WebClient();
                var jsonCallback = client.DownloadString(fullUrl);
                var jinShanDomain = JsonConvert.DeserializeObject<JinShanDomain>(jsonCallback);
                if (jinShanDomain != null && jinShanDomain.symbols.Any())
                {
                    foreach (var symbol in jinShanDomain.symbols)
                    {
                        if (!string.IsNullOrEmpty(symbol.ph_en_mp3))
                        {
                            info.VoiceUrl = symbol.ph_en_mp3;
                        }
                        else if (!string.IsNullOrEmpty(symbol.ph_am_mp3))
                        {
                            info.VoiceUrl = symbol.ph_am_mp3;
                        }
                        else if (!string.IsNullOrEmpty(symbol.ph_tts_mp3))
                        {
                            info.VoiceUrl = symbol.ph_tts_mp3;
                        }
                        else if (!string.IsNullOrEmpty(symbol.symbol_mp3))
                        {
                            info.VoiceUrl = symbol.symbol_mp3;
                        }

                        if (!string.IsNullOrEmpty(symbol.ph_en))
                        {
                            info.PH = symbol.ph_en;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return info;
        }
    }
}
