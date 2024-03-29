using InoxThanhNamServer.Datas;
using System.Collections.Concurrent;

namespace InoxThanhNamServer.Database
{
    public class SharedDB
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connection;
        public ConcurrentDictionary<string, UserConnection> connection => _connection;
    }
}
