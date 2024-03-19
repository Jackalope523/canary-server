namespace Repository
{
    public class UserReportFactory
    {
        private int created = 0;

        public UserReport Create(User reporter, User reportee, Event location)
        {
            created++;
            return new UserReport
            {
                SelfId = reporter.Id,
                OtherId = reportee.Id,
                EventId = location.Id,
                Type = Shared.UserReportType.Rude,
                FilingDate = DateTimeOffset.MinValue,
                Notes = "Test User Report " + created
            };
        }
    }
}
