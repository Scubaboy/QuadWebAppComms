using Breeze.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Breeze
{
    public interface IBreezeDataContext
    {
        public IBreezeRepository<T> GetRepository<T>();

        public bool PendingChanges();

        public Task<SaveResult> SavePendingChanges();
    }
}
