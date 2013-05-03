
namespace Gredis.Client
{
    public class GCreate
    {
        #region Vertex

        public GCreateVertex Vertex(string className)
        {
            GCreateVertex createVertex = new GCreateVertex();

            return createVertex.Vertex(className);
        }

        public GCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        public GCreateVertex Vertex(GVertex vertex)
        {
            GCreateVertex createVertex = new GCreateVertex()
                .Vertex(vertex.Class)
                .Properties(vertex);

            return createVertex;
        }

        #endregion

        #region Edge

        public GCreateEdge Edge(string label, GEdgeDirection direction = GEdgeDirection.None)
        {
            GCreateEdge createEdge = new GCreateEdge();

            return createEdge.Edge(label, direction);
        }

        public GCreateEdge Edge<T>(GEdgeDirection direction = GEdgeDirection.None)
        {
            return Edge(typeof(T).Name, direction);
        }

        public GCreateEdge Edge(GEdge edge)
        {
            GCreateEdge createEdge = new GCreateEdge()
                .Edge(edge.Label, edge.Direction)
                .Properties(edge);

            return createEdge;
        }

        #endregion
    }
}
