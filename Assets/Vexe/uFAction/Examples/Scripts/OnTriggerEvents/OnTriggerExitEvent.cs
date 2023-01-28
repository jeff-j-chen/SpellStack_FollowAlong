using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnTriggerExit message to your collider
/// passing in the collider object
/// </summary>
public class OnTriggerExitEvent : MonoBehaviour
{
	[ShowDelegate("OnTriggerExit", @canSetArgsFromEditor: false)]
	public ColliderAction onTriggerExit = new ColliderAction();

	void OnTriggerExit(Collider other)
	{
		onTriggerExit.Invoke(other);
	}
}