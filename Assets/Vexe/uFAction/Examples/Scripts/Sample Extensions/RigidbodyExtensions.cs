using UnityEngine;

namespace uFAction.Extensions
{
	public static class RigidbodyExtensions
	{
		public static void AddRandomForce(this Rigidbody rb, Vector2 xMinMax, Vector2 yMinMax, Vector2 zMinMax, ForceMode forceMode)
		{
			rb.AddForce(
				Random.Range(xMinMax.x, xMinMax.y),
				Random.Range(yMinMax.x, yMinMax.y),
				Random.Range(zMinMax.x, zMinMax.y),
				forceMode
			);
		}
	}
}