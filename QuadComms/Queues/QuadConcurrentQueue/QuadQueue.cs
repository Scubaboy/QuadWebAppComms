using QuadComms.Interfaces.Queues;
using System.Collections.Concurrent;
using System.Linq;

namespace QuadComms.Queues.QuadConcurrentQueue
{
    public class QuadQueue<T> : IDataTransferQueue<T> where T : class
    {
        private ConcurrentQueue<T> queue;

        public QuadQueue()
        {
            this.queue = new ConcurrentQueue<T>();
        }

        public void Add(T item)
        {
            this.queue.Enqueue(item);
        }

        public bool Remove(out T item)
        {
            return this.queue.TryDequeue(out item);
        }

        public bool Any()
        {
            return this.queue.Any();
        }
    }
}
