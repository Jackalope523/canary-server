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

            float sdlat = MathF.Sin((locationBeta.LatitudeInRadians - locationAlpha.LatitudeInRadians) / 2);
            float sdlon = MathF.Sin((locationBeta.LongitudeInRadians - locationAlpha.LongitudeInRadians) / 2);
            float q = sdlat * sdlat + MathF.Cos(locationAlpha.LatitudeInRadians) * MathF.Cos(locationBeta.LatitudeInRadians) * sdlon * sdlon;
            float d = 2 * EarthRadius.Kilometres * MathF.Asin(MathF.Sqrt(q));

            Distance distance = new() { Kilometres = d };

            return distance;
        }

        public static bool AreInRange(GeoLocation locationAlpha, GeoLocation locationBeta, Distance range)
        {
            Distance distance = DistanceBetween(locationAlpha, locationBeta);
            
            return distance.Kilometres <= range.Kilometres;
        }

        public static Distance EarthRadius => new() { Kilometres = 6371 };


        public float Latitude { get; init; }
        public float Longitude { get; init; }

        public float LatitudeInRadians => (MathF.PI / 180) * Latitude;
        public float LongitudeInRadians => (MathF.PI / 180) * Longitude;

        // Figure out location initialisation
    }


    public struct Distance
    {
        public float Kilometres
        { 
            get => Metres / 1000f;
            set => Metres = value * 1000f;
        }
        public float Metres { get; set; }
    }
}
