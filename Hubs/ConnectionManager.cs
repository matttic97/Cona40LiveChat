using System.Collections.Concurrent;

namespace Cona40LiveChat.Hubs;

public class ConnectionManager
{
    private const int MaxConnections = 20;

    private int _count;
    private readonly ConcurrentDictionary<string, byte> _activeConnections = new();
    
    public bool TryOpenConnection(string connectionId)
    {
        if (_count >= MaxConnections)
            return false;

        if (!_activeConnections.TryAdd(connectionId, 0))
            return false;
        
        Interlocked.Increment(ref _count);
        return true;

    }

    public void CloseConnection(string connectionId)
    {
        if (_activeConnections.TryRemove(connectionId, out _))
            Interlocked.Decrement(ref _count);
    }
}