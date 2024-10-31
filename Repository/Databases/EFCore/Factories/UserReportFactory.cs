namespace Repository
{
    internal class UserReportFactory : Factory
    {
        private int created = 0;

        internal UserReport Create(User reporter, User reportee, Gathering location)
        {
            created++;
            return new UserReport
            {
                SelfId = reporter.Id,
                OtherId = reportee.Id,
                GatheringId = location.Id,
                Type = UserReportType.Rude,
                FilingDate = DateTimeOffset.MinValue,
                Notes = "Test User Report " + created
            };
        }
    }
}
