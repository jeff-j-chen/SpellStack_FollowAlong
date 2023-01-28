using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnMouseEnter message to your collider
/// </summary>
public class OnMouseEnterEvent: MonoBehaviour
{
	[ShowDelegate("OnMouseEnter")]
	public UnityAction onMouseEnter = new UnityAction();

	void OnMouseEnter()
	{
		onMouseEnter.Invoke();
	}
}