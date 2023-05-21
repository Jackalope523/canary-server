using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.QueryStore;

namespace Repository.Sentries
{  
    public class QueueSentry : Sentry
    {
        private int changeQueueSize;
        private int changesQueued;
        
        protected override void QueryMade()
        {
            changesQueued++;

            if (changesQueued == changeQueueSize)
            {
                changesQueued = 0;
                context.SaveChanges();
                context.Dispose();

                context = new TestContext();
            }
        }
    }
}
