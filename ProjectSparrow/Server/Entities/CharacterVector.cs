using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
	internal class CharacterVector
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

		public void MoveTowards(CharacterVector otherVector)
		{

		}

		public int Extraversion { get; }
		public int Athleticism { get; }
		public int Openness { get; }
		public int Chaoticness { get; }
		public int Competitiveness { get; }
		public int Industriousness { get; }
		public int NightOwl { get; }

		public float Magnitude => MathF.Sqrt(Extraversion * Extraversion + Athleticism * Athleticism +
			Openness * Openness + Chaoticness * Chaoticness + Competitiveness * Competitiveness +
			NightOwl * NightOwl);
	}
}
