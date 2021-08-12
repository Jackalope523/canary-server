using Server.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class GeoLocationTest
    {
        private static GeoLocation Paris = new() { Latitude = 48.8589507f, Longitude = 2.2770197f };
        private static GeoLocation Montreal = new() { Latitude = 45.5581804f, Longitude = -74.0104929f };
        private static GeoLocation Riga = new() { Latitude = 56.9715357f, Longitude = 23.8489805f };
        private static GeoLocation Melbourne = new() { Latitude = -37.970154f, Longitude = 144.4926742f };

        public static IEnumerable<object[]> CityDistances =>
            new List<object[]>
            {
                new object[] { Paris, Montreal, new Distance() { Kilometres = 5526 } },
                new object[] { Paris, Riga, new Distance() { Kilometres = 1694 } },
                new object[] { Melbourne, Montreal, new Distance() { Kilometres = 16746 } },
                new object[] { Melbourne, Melbourne, new Distance() { Kilometres = 0 } }
            };

        public static IEnumerable<object[]> CitiesInRange =>
            new List<object[]>
            {
                new object[] { Paris, Montreal, GeoLocation.EarthRadius },
                new object[] { Paris, Riga, new Distance() { Kilometres = 1700 } },
                new object[] { Melbourne, Montreal, new Distance() { Kilometres = 20000 } },
                new object[] { Melbourne, Melbourne, new Distance() { Metres = 1 } }
            };

        public static IEnumerable<object[]> CitiesNotInRange =>
            new List<object[]>
            {
                new object[] { Paris, Montreal,  new Distance() { Kilometres = 10 } },
                new object[] { Paris, Riga, new Distance() { Kilometres = 1690 } },
                new object[] { Melbourne, Montreal, new Distance() { Kilometres = 10000 } }
            };


        [Theory]
        [MemberData(nameof(CityDistances))]
        internal void Distance_TwoPoints_ReturnsDistance(GeoLocation pointA, GeoLocation pointB, Distance expectedDistance)
        {
            Assert.Equal(expectedDistance.Kilometres, MathF.Round(GeoLocation.DistanceBetween(pointA, pointB).Kilometres));
        }

        [Theory]
        [MemberData(nameof(CitiesInRange))]
        internal void AreInRange_TwoPoints_ReturnsTrue(GeoLocation pointA, GeoLocation pointB, Distance range)
        {
            Assert.True(GeoLocation.AreInRange(pointA, pointB, range));
        }

        [Theory]
        [MemberData(nameof(CitiesNotInRange))]
        internal void AreInRange_TwoPoints_ReturnsFalse(GeoLocation pointA, GeoLocation pointB, Distance range)
        {
            Assert.False(GeoLocation.AreInRange(pointA, pointB, range));
        }
    }
}
