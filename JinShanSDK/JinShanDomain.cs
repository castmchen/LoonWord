using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinShanSDK
{
    public class JinShanDomain
    {
        public string word_name { get; set; }
        public List<symbols> symbols { get; set; }
        public bool wordFlag { get; set; }
    }

    public class symbols
    {
        public string symbol_mp3 { get; set; }

        public string ph_en_mp3 { get; set; }

        public string ph_am_mp3 { get; set; }

        public string ph_tts_mp3 { get; set; }

        public string ph_en { get; set; }

        public List<parts> parts { get; set; }
    }

    public class parts
    {
        public string part { get; set; }
        public List<string> means { get; set; }
    }

    public class JinShanSentenceDomain
    {
        public string word_name { get; set; }
        public List<symbolsSentence> symbols { get; set; }
    }

    public class symbolsSentence
    {
        public string word_symbol { get; set; }

        public List<partsSentence> parts { get; set; }
    }

    public class partsSentence
    {
        public List<Sentence> means { get; set; }
    }

    public class Sentence
    {
        public string word_mean { get; set; }
    }

    public class VoiceAndPH
    {
        public string VoiceUrl { get; set; }
        public string PH { get; set; }
    }
}
