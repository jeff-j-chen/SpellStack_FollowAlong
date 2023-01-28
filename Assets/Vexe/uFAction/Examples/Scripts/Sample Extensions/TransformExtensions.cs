using System.Collections;
using UnityEngine;

namespace uFAction.Extensions
{
	public static class TransformExtensions
	{
		public static IEnumerator SmoothRotateBy(this Transform transform, Vector3 angle, float timeToTake)
		{
			Quaternion from = transform.rotation;
			Vector3 targetAngle = from.eulerAngles + angle;
			Quaternion to = Quaternion.Euler(targetAngle);
			float t = 0;

			while (t < 1)
			{
				t += Time.deltaTime / timeToTake;
				transform.rotation = Quaternion.Slerp(from, to, t);
				yield return null;
			}
			transform.rotation = to;
		}
	}
}