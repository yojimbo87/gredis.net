
namespace Gredis.Client
{
    public class GCreateEdge
    {
        private GEdge _edge = new GEdge();

        public GCreateEdge Edge(string label, GEdgeDirection direction)
        {
            _edge.Label = label;
            _edge.Direction = direction;

            return this;
        }

        #region Properties

        public GCreateEdge Properties<T>(T obj)
        {
            _edge.MapFrom(obj);

            return this;
        }

        public GCreateEdge Property<T>(string name, T value)
        {
            _edge.SetField(name, value);

            return this;
        }

        #endregion

        public GEdge Run()
        {
            return G.Redis.CreateEdge(_edge);
        }
    }
}
