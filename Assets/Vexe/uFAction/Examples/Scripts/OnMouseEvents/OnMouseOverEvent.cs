using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnMouseOver message to your collider
/// </summary>
public class OnMouseOverEvent : MonoBehaviour
{
	[ShowDelegate("OnMouseOver")]
	public UnityAction onMouseOver = new UnityAction();

	void OnMouseOver()
	{
		onMouseOver.Invoke();
	}
}