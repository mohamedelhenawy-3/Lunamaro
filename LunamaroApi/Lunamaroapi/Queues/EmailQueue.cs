using System.Collections.Concurrent;

namespace Lunamaroapi.Queues
{
    public class EmailQueue
    {

        public static ConcurrentQueue<(string Email, string Subject, string Body)> Queue
       = new();
    }
}

