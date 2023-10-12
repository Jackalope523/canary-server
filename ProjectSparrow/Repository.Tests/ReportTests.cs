using Server.Boundaries;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Tests
{
    public class ReportTests
    {
        [Fact]
        public void GetReports_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void GetReportsByUser_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void ReportUser_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void GetEventReports_SUCCESS()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void ReportEvent_SUCCESS()
        {
            throw new NotImplementedException();
        }


        public EventShard FindAttendingEvent(Guid id) { throw new NotImplementedException(); }
        public List<EventShard> FindUpcomingEvents(Guid id) { throw new NotImplementedException(); }
        public List<EventShard> FindPastEvents(Guid id) { throw new NotImplementedException(); }
       
    }
}
