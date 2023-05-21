using Repository.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Sentries
{
    public class TestSentry : Sentry
    {
        private static TestSentry singleton;

        public TestSentry() 
        {
            context = new TestContext();
        }

        public static TestSentry GetTestSentry()
        {
            if (singleton == null) singleton = new TestSentry();
            return singleton;
        }
        protected override void QueryMade()
        {
            context.SaveChanges();
            context.Dispose();

            context = new TestContext();
        }
    }
}
