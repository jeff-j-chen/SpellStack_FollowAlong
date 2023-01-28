using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnTriggerEnter message to your collider
/// passing in the collider object
/// </summary>
public class OnTriggerEnterEvent : MonoBehaviour
{
	[ShowDelegate("OnTriggerEnter", @canSetArgsFromEditor: false)]
	public ColliderAction onTriggerEnter = new ColliderAction();

	void OnTriggerEnter(Collider other)
	{
		onTriggerEnter.Invoke(other);
	}
}