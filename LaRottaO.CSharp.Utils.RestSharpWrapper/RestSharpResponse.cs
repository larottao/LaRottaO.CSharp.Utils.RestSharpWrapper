using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class RestSharpResponse
    {
        public Boolean success { get; set; }
        public String details { get; set; }
        public String content { get; set; }
    }
}