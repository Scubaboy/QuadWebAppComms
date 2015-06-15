using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Queues
{
    public interface IDataTransferQueue<T>
    {
        void Add(T item);
        bool Any();
        bool Remove(out T item);
    }
}
