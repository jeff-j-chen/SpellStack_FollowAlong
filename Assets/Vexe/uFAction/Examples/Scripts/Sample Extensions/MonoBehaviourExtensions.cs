using UnityEngine;

namespace uFAction.Extensions
{
	public static class MonoBehaviourExtensions
	{
		public static void SmoothRotateBy(this MonoBehaviour mb, Vector3 angle, float timeToTake)
		{
			mb.StartCoroutine(mb.transform.SmoothRotateBy(angle, timeToTake));
		}

		public static void SmoothRotate90DgsY(this MonoBehaviour mb)
		{
			mb.SmoothRotateBy(new Vector3(0, 90f, 0), 1f);
		}
	}
}