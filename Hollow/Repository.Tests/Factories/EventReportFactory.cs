namespace Repository.Tests
{
    internal class EventReportFactory
    {
        private int created = 0;

        public EventReport Create(User reporter, Event location)
        {
            created++;
            return new EventReport
            {
                SelfId = reporter.Id,
                OtherId = location.HostId,
                EventId = location.Id,
                Type = Shared.EventReportType.Misleading,
                FilingDate = DateTimeOffset.MinValue,
                Notes = "Test Event Report " + created
            };          
        }
    }
}
