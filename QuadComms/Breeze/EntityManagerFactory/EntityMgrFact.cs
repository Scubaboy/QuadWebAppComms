using Breeze.Sharp;
using QuadComms.Interfaces.Breeze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Breeze.EntityManagerFactory
{
    internal class EntityMgrFact : IBreezeEntityManagerFactory
    {
        public EntityMgrFact()
        {

        }
        public EntityManager CreateEntityManager(string serviceName)
        {
            return new EntityManager(serviceName);
        }
    }
}
