namespace Repository
{
    internal class GatheringReportFactory
    {
        private int created = 0;

        internal GatheringReport Create(User reporter, Gathering location)
        {
            created++;
            return new GatheringReport
            {
                UserId = reporter.Id,
                GatheringId = location.Id,
                Type = GatheringReportType.Misleading,
                FilingDate = DateTimeOffset.MinValue,
                Notes = "Test Gathering Report " + created
            };          
        }
    }
}
