using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BaiduPackage
{
    public class TranslationMethod
    {
 

        public static TranslationResult TranslatedWithBaidu(string source, LanguageEnum from = LanguageEnum.Auto, LanguageEnum to = LanguageEnum.Auto)
        {
            var jsonResult = string.Empty;
            try
            {
                var fromParam = from.ToString().ToLowerInvariant();
                var toParam = to.ToString().ToLowerInvariant();
                var randomNum = DateTime.Now.Millisecond.ToString();
                var signFormatStr = string.Format("{0}{1}{2}{3}", ConstFile.App_Id, source, randomNum, ConstFile.Security_Code);
                var md5Sign = EncryptWithMD5(signFormatStr);
                var url = $"{ConstFile.baiduHost}?q={HttpUtility.UrlEncode(source, Encoding.UTF8)}&from={fromParam}&to={toParam}&appid={ConstFile.App_Id}&salt={randomNum}&sign={md5Sign}";
                WebClient webClient = new WebClient();
                jsonResult = webClient.DownloadString(url);
                var result = JsonConvert.DeserializeObject<TranslationResult>(jsonResult);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new TranslationResult();
        }

        private static string EncryptWithMD5(string str)
        {
            var md5Info = new MD5CryptoServiceProvider();
            var result = Encoding.UTF8.GetBytes(str);
            var md5Str = md5Info.ComputeHash(result);
            return BitConverter.ToString(md5Str).Replace("-", "").ToLowerInvariant();
        }
    }
}
