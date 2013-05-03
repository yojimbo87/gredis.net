using System.Collections.Generic;

namespace Gredis.Client
{
    public class GVertex : GDocument
    {
        public long ID { get; set; }
        public string Class { get; set; }
        public List<GEdge> In { get; set; }
        public List<GEdge> Out { get; set; }
    }
}
