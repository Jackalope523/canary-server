using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shared;

namespace Core.Entities
{
    public readonly struct GeoLocation
    {
        #region Olive Branches

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

        #endregion

        #region Variables

        public double Latitude { get; init; }
        public double Longitude { get; init; }

        public double LatitudeInRadians => (Math.PI / 180) * Latitude;
        public double LongitudeInRadians => (Math.PI / 180) * Longitude;

        #endregion
    }


    public struct Distance
    {
        #region Variables

        public double Kilometres
        {
            get => Metres / 1000d;
            set => Metres = value * 1000d;
        }
        public double Metres { get; set; }

        #endregion

        #region Dissimilation

        public static bool operator ==(Distance a, Distance b)
            => a.Metres == b.Metres;

        public static bool operator !=(Distance a, Distance b)
            => a.Metres != b.Metres;

        public static bool operator <(Distance a, Distance b)
            => a.Metres < b.Metres;

        public static bool operator >(Distance a, Distance b)
            => a.Metres > b.Metres;

        public override bool Equals(object obj)
        {
            return obj is Distance other && this == other;
        }

        public override int GetHashCode()
        {
            return Metres.GetHashCode();
        }

        #endregion
    }


    public struct Synced<T>
    {
		private T cachedValue;
        private bool synced;

        private Func<Task<T>> syncFunction;
        private Task<T> syncTask;

        public Synced(Func<Task<T>> synchronisationFunction)
        {
            cachedValue = default;
            synced = false;
            syncFunction = synchronisationFunction;
            syncTask = null;
        }

        public Synced(T value)
        {
            cachedValue = value;
            synced = true;
            syncFunction = null;
            syncTask = null;
        }

        public async Task<T> Value()
        {
            if (!synced)
            { await Sync(); }

            return cachedValue;
        }

        public async Task Sync()
        {
            if (syncFunction == null)
            { throw new UndefinedBehaviourException($"Cannot Sync {nameof(T)} without synchronising function."); }

            if (syncTask == null || syncTask.IsCompleted)
            { syncTask = syncFunction.Invoke(); }
            
            cachedValue = await syncTask;
            synced = true;
        }

        public void Set(T value)
        {
            cachedValue = value;
            synced = true;
		}

        #region Dissimilation

        public static T operator +(Synced<T> a, T b)
            => ((dynamic)a.cachedValue) + ((dynamic)b);

		public static T operator -(Synced<T> a, T b)
            => ((dynamic)a.cachedValue) - ((dynamic)b);

		#endregion
	}
}
