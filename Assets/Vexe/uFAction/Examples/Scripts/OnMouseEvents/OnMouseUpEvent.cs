using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnMouseUp message to your collider
/// </summary>
public class OnMouseUpEvent : MonoBehaviour
{
	[ShowDelegate("OnMouseUp")]
	public UnityAction onMouseUp = new UnityAction();

	void OnMouseUp()
	{
		onMouseUp.Invoke();
	}
}