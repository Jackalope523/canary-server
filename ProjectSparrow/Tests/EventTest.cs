using Server.Entities;
using System;
using Xunit;

namespace Tests
{
    public class EventTest
    {
        [Fact]
        internal void Constructor_NormalInput_ProperlyInitialised()
        {
            /// Testing constructors is not necessary unless they have complex internal logic.
            /// In the case here, Event does not, and I am testing it to provide an example test.
            /// Once EventTest has more tests, this will be removed.

            string hostID = "111-1111-111";

            DateTime beforeConstruction = DateTime.UtcNow;

            Event e = new(hostID);

            DateTime afterConstruction = DateTime.UtcNow;

            Assert.True(e.HostID.Equals(hostID));

            Assert.Equal(1, e.Participants.Count);
            Assert.True(e.Participants[0].ID.Equals(hostID));

            Assert.True(e.StartTime.Ticks >= beforeConstruction.Ticks || e.StartTime.Ticks <= afterConstruction.Ticks);
        }
    }
}
