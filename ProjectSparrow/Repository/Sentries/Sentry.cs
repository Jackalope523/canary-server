using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Contexts;

namespace Repository.Sentries
{
    public abstract class Sentry
    {
        protected QueryContext context;

        protected abstract void QueryMade();

        public QueryContext GetContext()
        {
            QueryMade();
            return context;
        }     
    }
}
