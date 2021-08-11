using Server.Entities;
using System;
using Xunit;

namespace Tests
{
    public class EventTest
    {
        [Fact]
        public void InitialiseEvent()
        {
            var e = new Event("111");

            Assert.True(e.HostID.Equals("111"));
        }
    }
}
