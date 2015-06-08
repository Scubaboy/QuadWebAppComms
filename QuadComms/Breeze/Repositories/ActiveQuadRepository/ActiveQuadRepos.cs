using Breeze.Sharp;
using QuadEFModels.Entities;
using QuadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadModelEntityExtensions.Extensions;
using QuadComms.Interfaces.Breeze;


namespace QuadComms.Breeze.Repositories.ActiveQuadRepository
{
    public class ActiveQuadRepos : IBreezeRepository<ActiveQuad>
    {
        private EntityManager reposEntityManager;
        private bool primed;
        private EntityQuery<ActiveQuadEntity> theQuery;

        public ActiveQuadRepos(IBreezeEntityManagerFactory entityManagerFact, string serviceName)
        {
            this.reposEntityManager = entityManagerFact.CreateEntityManager(serviceName);
            this.theQuery = new EntityQuery<ActiveQuadEntity>();
            this.primed = false;
        }

        public async Task<IEnumerable<ActiveQuad>> Get()
        {
            return await this.ExecuteQuery()
                .ContinueWith(completedQuery =>
                {
                    var result = completedQuery.Result;
                    var returnData = new List<ActiveQuad>();

                    returnData.AddRange(result.Select(quad => quad.ToModel()));
                    this.primed = true;
                    return returnData;
                })
            .ConfigureAwait(false);   
        }

        public bool Add(ActiveQuad item)
        {
            var theEntity = this.reposEntityManager.AddEntity(item.ToEntity());

            return theEntity.EntityAspect.EntityState == EntityState.Added;
        }

        public async Task<bool> Delete(ActiveQuad item)
        {
            //Find the entity.
            try
            {
                var data = await this.ExecuteQuery().ContinueWith(coninueData =>
                {
                    var result = coninueData.Result;

                    return result.First(quad => quad.ActiveQuadEntityId == item.Id);
                }).ConfigureAwait(false);

                data.EntityAspect.Delete();

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public async Task<bool> Update(ActiveQuad item)
        {
            try
            {
                var data = await this.ExecuteQuery().ContinueWith(coninueData =>
                {
                    var result = coninueData.Result;

                    return result.First(quad => quad.ActiveQuadEntityId == item.Id);
                }).ConfigureAwait(false);

                //Update the entity
                data.InUse = item.InUse;

                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public async Task<SaveResult> SaveChanges()
        {
            return await this.reposEntityManager.SaveChanges();
        }

        public void CancelChanges()
        {
            if (this.reposEntityManager.HasChanges())
            {
                this.reposEntityManager.RejectChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<ActiveQuadEntity>> ExecuteQuery()
        {
            if (this.primed)
            {
                return this.reposEntityManager.ExecuteQueryLocally<ActiveQuadEntity>(this.theQuery);
            }

            return await this.reposEntityManager.ExecuteQuery<ActiveQuadEntity>(this.theQuery);
        }
    }
}
