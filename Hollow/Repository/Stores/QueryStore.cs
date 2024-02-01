namespace Repository
{
    public abstract class QueryStore
    {
        protected Sentry storeSentry;

        public QueryStore(Sentry sentry)
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
