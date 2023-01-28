using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnMouseExit message to your collider
/// </summary>
public class OnMouseExitEvent : MonoBehaviour
{
	[ShowDelegate("OnMouseExit")]
	public UnityAction onMouseExit = new UnityAction();

	void OnMouseExit()
	{
		onMouseExit.Invoke();
	}
}