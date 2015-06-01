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
        public Task<IEnumerable<T>> Get();

        public bool Add(T item);

        public bool Delete(T item);

        public bool Update(T item);

        public Task<SaveResult> SaveChanges();

        public void CancelChanges();
    }
}
