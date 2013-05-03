using ServiceStack.Redis;

namespace Gredis.Client
{
    public static class G
    {
        internal static RedisClient Redis;

        public static void Initialize(string host, int port, string password, int database)
        {
            Redis = new RedisClient(host, port, password);
            Redis.Db = database;
        }
    }
}
