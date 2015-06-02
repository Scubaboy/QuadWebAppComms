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
        IBreezeRepository<T> GetRepository<T>() where T : class;

        bool PendingChanges();

        Task<SaveResult> SavePendingChanges();
    }
}
