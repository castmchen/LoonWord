using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastmRepository
{
    public class WordDomain
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
        public string Trascation { get; set; }
        public string Phonetic { get; set; }
        public string Voice { get; set; }
        public int Flag { get; set; }
        public long CreateTime { get; set; }

        public bool TrascationFlag { get; set; }
    }
}
