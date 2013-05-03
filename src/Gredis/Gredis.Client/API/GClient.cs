using Gredis.Client.Protocol;

namespace Gredis.Client
{
    public static class G
    {
        internal static Redis Redis;

        public GCreate Create { get { return new GCreate(); } }

        public static void Initialize(string host, int port, string password, int database)
        {
            Redis = new Redis(host, port, password, database);
        }
    }
}
