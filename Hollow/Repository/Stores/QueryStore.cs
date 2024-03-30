namespace Repository
{
    public abstract class QueryStore
    {
        protected IDatabaseSentry storeSentry;

        public QueryStore(IDatabaseSentry sentry)
        {
            storeSentry = sentry;
        }      
    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
