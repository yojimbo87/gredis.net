using System.Collections.Generic;

namespace Gredis.Client
{
    public class GEdge : GDocument
    {
        public string Label { get; set; }
        public GVertex In { get; set; }
        public GVertex Out { get; set; }
        public GEdgeDirection Direction { get; set; }
    }
}
