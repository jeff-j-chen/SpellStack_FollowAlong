using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnMouseDown message to your collider
/// </summary>
public class OnMouseDownEvent : MonoBehaviour
{
	[ShowDelegate("OnMouseDown")]
	public UnityAction onMouseDown = new UnityAction();

	void OnMouseDown()
	{
		onMouseDown.Invoke();
	}
}