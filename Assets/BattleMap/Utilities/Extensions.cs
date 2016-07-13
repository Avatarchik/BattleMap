namespace Utilities
{
	using UnityEngine;
	using System;

	public static class Extensions
	{
		public static Vector3 Round(this Vector3 v, float MultipleOf = 1f)
		{
			v.x = NearestRound(v.x, MultipleOf);
			v.y = NearestRound(v.y, MultipleOf);
			v.z = NearestRound(v.z, MultipleOf);
			return v;
		}

		public static float Average(this Vector3 v)
		{
			return (v.x + v.y + v.z) / 3;
		}

		public static float XZAverage(this Vector3 v)
		{
			return (v.x + v.z) / 2;
		}

		public static float NearestRound(float x, float delX)
		{
			if (delX < 1)
			{
				float i = Mathf.Floor(x);
				float x2 = i;
				while ((x2 += delX) < x) ;
				float x1 = x2 - delX;
				return (Mathf.Abs(x - x1) < Mathf.Abs(x - x2)) ? x1 : x2;
			}
			else
			{
				return (float)Math.Round(x / delX, MidpointRounding.AwayFromZero) * delX;
			}
		}
	}
}
