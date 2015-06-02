using Breeze.Sharp;
using QuadEFModels.Entities;
using QuadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadModelEntityExtensions.Extensions;

namespace QuadComms.Interfaces.Breeze.Repositories.ActiveQuadRepository
{
    public class ActiveQuadRepos : IBreezeRepository<ActiveQuad>
    {
        private EntityManager reposEntityManager;

        private EntityQuery<ActiveQuad> theQuery;

        public ActiveQuadRepos(IBreezeEntityManagerFactory entityManagerFact)
        {
            this.reposEntityManager = entityManagerFact.CreateEntityManager("test");
            this.theQuery = new EntityQuery<ActiveQuadEntity>().Select(quad => quad.ToModel());
        }

        public async Task<IEnumerable<ActiveQuad>> Get()
        {
            return await this.reposEntityManager.ExecuteQuery<ActiveQuad>(this.theQuery).ConfigureAwait(false);  
        }

        public bool Add(ActiveQuad item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ActiveQuad item)
        {
            throw new NotImplementedException();
        }

        public bool Update(ActiveQuad item)
        {
            throw new NotImplementedException();
        }

        public Task<SaveResult> SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void CancelChanges()
        {
            throw new NotImplementedException();
        }
    }
}
