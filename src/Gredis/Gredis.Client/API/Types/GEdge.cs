using System.Collections.Generic;

namespace Gredis.Client
{
    public class GEdge : GDocument
    {
        public long ID { get; set; }
        public string Label { get; set; }
        public GVertex In { get; set; }
        public GVertex Out { get; set; }
        public GEdgeDirection Direction { get; set; }
    }
}
