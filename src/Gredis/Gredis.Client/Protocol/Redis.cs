using ServiceStack.Redis;

namespace Gredis.Client.Protocol
{
    internal class Redis
    {
        private RedisClient _client;

        internal Redis(string host, int port, string password, int database)
        {
            _client = new RedisClient(host, port, password);
            _client.Db = database;
        }

        internal GVertex CreateVertex(GVertex vertex)
        {
            // TODO: lua script which creates vertex keys in redis

            return vertex;
        }

        internal GEdge CreateEdge(GEdge edge)
        {
            // TODO: lua script which creates edge keys in redis

            return edge;
        }
    }
}
