
namespace Gredis.Client
{
    public class GCreateVertex
    {
        private GVertex _vertex = new GVertex();

        public GCreateVertex Vertex(string className)
        {
            _vertex.Class = className;

            return this;
        }

        #region Properties

        public GCreateVertex Properties<T>(T obj)
        {
            _vertex.MapFrom(obj);

            return this;
        }

        public GCreateVertex Property<T>(string name, T value)
        {
            _vertex.SetField(name, value);

            return this;
        }

        #endregion

        public GVertex Run()
        {
            return G.Redis.CreateVertex(_vertex);
        }
    }
}
