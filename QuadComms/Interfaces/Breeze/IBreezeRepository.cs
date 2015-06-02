using Breeze.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Breeze
{
    public interface IBreezeRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get();

        bool Add(T item);

        bool Delete(T item);

        bool Update(T item);
        
        Task<SaveResult> SaveChanges();

        void CancelChanges();
    }
}
