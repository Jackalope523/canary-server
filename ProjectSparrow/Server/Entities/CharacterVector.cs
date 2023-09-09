using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
	internal struct CharacterVector
	{
		public static float AngleBetween(CharacterVector firstVector, CharacterVector secondVector)
		{
			int dotProduct =
				firstVector.Extraversion * secondVector.Extraversion +
				firstVector.Athleticism * secondVector.Athleticism +
				firstVector.Openness * secondVector.Openness +
				firstVector.Chaoticness * secondVector.Chaoticness +
				firstVector.Competitiveness * secondVector.Competitiveness +
				firstVector.Industriousness * secondVector.Industriousness +
				firstVector.NightOwl * secondVector.NightOwl;

			float angle = MathF.Acos(dotProduct / (firstVector.Magnitude * secondVector.Magnitude));

			return angle;
		}

		public int Extraversion { get; init; }
		public int Athleticism { get; init; }
		public int Openness { get; init; }
		public int Chaoticness { get; init; }
		public int Competitiveness { get; init; }
		public int Industriousness { get; init; }
		public int NightOwl { get; init; }

		public float Magnitude => MathF.Sqrt(Extraversion * Extraversion + Athleticism * Athleticism +
			Openness * Openness + Chaoticness * Chaoticness + Competitiveness * Competitiveness +
			NightOwl * NightOwl);

		public CharacterVector MoveTowards(CharacterVector otherVector, float modifier = 1f)
		{
			return this + ((otherVector - this) * modifier);
		}

		public static CharacterVector operator +(CharacterVector a, CharacterVector b)
		{
			return new()
			{
				Extraversion = a.Extraversion + b.Extraversion,
				Athleticism = a.Athleticism + b.Athleticism,
				Openness = a.Openness + b.Openness,
				Chaoticness = a.Chaoticness + b.Chaoticness,
				Competitiveness = a.Competitiveness + b.Competitiveness,
				Industriousness = a.Industriousness + b.Industriousness,
				NightOwl = a.NightOwl + b.NightOwl
			};
		}

		public static CharacterVector operator -(CharacterVector a, CharacterVector b)
		{
			return new()
			{
				Extraversion = a.Extraversion - b.Extraversion,
				Athleticism = a.Athleticism - b.Athleticism,
				Openness = a.Openness - b.Openness,
				Chaoticness = a.Chaoticness - b.Chaoticness,
				Competitiveness = a.Competitiveness - b.Competitiveness,
				Industriousness = a.Industriousness - b.Industriousness,
				NightOwl = a.NightOwl - b.NightOwl
			};
		}

		public static CharacterVector operator *(CharacterVector a, float f)
		{
			return new()
			{
				Extraversion = (int) (a.Extraversion * f),
				Athleticism = (int) (a.Athleticism * f),
				Openness = (int) (a.Openness * f),
				Chaoticness = (int) (a.Chaoticness * f),
				Competitiveness = (int) (a.Competitiveness * f),
				Industriousness = (int) (a.Industriousness * f),
				NightOwl = (int) (a.NightOwl * f)
			};
		}
	}
}
