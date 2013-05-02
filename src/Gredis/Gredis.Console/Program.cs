using System;
using System.IO;
using Gredis.Client;

namespace Gredis.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] connectionString = File.ReadAllText(@"..\..\..\..\..\ConnectionString.txt").Split(';');

            G.Initialize(
                connectionString[0],
                int.Parse(connectionString[1]),
                connectionString[2],
                int.Parse(connectionString[3])
            );
        }
    }
}
