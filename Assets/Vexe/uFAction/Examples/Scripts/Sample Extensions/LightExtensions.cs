using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace uFAction.Extensions
{
	public static class LightExtensions
	{
		private static Func<float, float, float> rand = Random.Range;

		public static void RandomizeIntensity(this Light light, float min, float max)
		{
			light.intensity = rand(min, max);
		}

		public static void RandomizeIntensity(this Light light)
		{
			light.RandomizeIntensity(0, 1);
		}

		public static void RandomizeColor(this Light light)
		{
			light.color = new Color(rand(0, 1), rand(0, 1), rand(0, 1));
		}
	}
}