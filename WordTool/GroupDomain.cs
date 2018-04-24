using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTool
{
    public class GroupDomain
    {
        public string Name { get; set; }
    }

    public class ModelDomain
    {
        public ModelEnum Model { get; set; }
        public string Value { get; set; }
    }
}
