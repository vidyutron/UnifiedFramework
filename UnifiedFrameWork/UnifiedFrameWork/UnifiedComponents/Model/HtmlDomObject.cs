using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.UnifiedComponents
{
    public class HtmlDomObject
    {
        public List<KeyValue> Id { get; set; }
        public List<KeyValue> Class { get; set; }
        public List<KeyValue> AnchorText { get; set; }
        public List<KeyValue> AnchorXpath { get; set; }

        public List<KeyValue> ButtonXpath { get; set; }
    }
}
