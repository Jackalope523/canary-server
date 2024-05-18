namespace Repository
{
    internal class UserReportFactory
    {
        private int created = 0;

        internal UserReport Create(User reporter, User reportee, Event location)
        {
            created++;
            return new UserReport
            {
                SelfId = reporter.Id,
                OtherId = reportee.Id,
                EventId = location.Id,
                Type = UserReportType.Rude,
                FilingDate = DateTimeOffset.MinValue,
                Notes = "Test User Report " + created
            };
        }
    }
}
