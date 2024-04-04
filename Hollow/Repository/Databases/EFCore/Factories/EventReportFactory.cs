namespace Repository
{
    internal class EventReportFactory
    {
        private int created = 0;

        internal EventReport Create(User reporter, Event location)
        {
            created++;
            return new EventReport
            {
                UserId = reporter.Id,
                EventId = location.Id,
                Type = Shared.EventReportType.Misleading,
                FilingDate = DateTimeOffset.MinValue,
                Notes = "Test Event Report " + created
            };          
        }
    }
}
