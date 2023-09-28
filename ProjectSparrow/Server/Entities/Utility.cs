using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Entities
{
    public readonly struct GeoLocation
    {
        public static Distance DistanceBetween(GeoLocation locationAlpha, GeoLocation locationBeta)
        {

            double sdlat = Math.Sin((locationBeta.LatitudeInRadians - locationAlpha.LatitudeInRadians) / 2);
            double sdlon = Math.Sin((locationBeta.LongitudeInRadians - locationAlpha.LongitudeInRadians) / 2);
            double q = sdlat * sdlat + Math.Cos(locationAlpha.LatitudeInRadians) * Math.Cos(locationBeta.LatitudeInRadians) * sdlon * sdlon;
            double d = 2 * EarthRadius.Kilometres * Math.Asin(Math.Sqrt(q));

            Distance distance = new() { Kilometres = d };

            return distance;
        }

        public static bool AreInRange(GeoLocation locationAlpha, GeoLocation locationBeta, Distance range)
        {
            Distance distance = DistanceBetween(locationAlpha, locationBeta);
            
            return distance.Kilometres <= range.Kilometres;
        }

        public static Distance EarthRadius => new() { Kilometres = 6371 };


        public double Latitude { get; init; }
        public double Longitude { get; init; }

        public double LatitudeInRadians => (Math.PI / 180) * Latitude;
        public double LongitudeInRadians => (Math.PI / 180) * Longitude;
    }


    public struct Distance
    {
        public double Kilometres
        { 
            get => Metres / 1000d;
            set => Metres = value * 1000d;
        }
        public double Metres { get; set; }

        public static bool operator ==(Distance a, Distance b)
            => a.Metres == b.Metres;

        public static bool operator !=(Distance a, Distance b)
            => a.Metres != b.Metres;

        public static bool operator <(Distance a, Distance b)
            => a.Metres < b.Metres;

        public static bool operator >(Distance a, Distance b)
            => a.Metres > b.Metres;
	}
}
