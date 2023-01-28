using UnityEngine;
using uFAction;

/// <summary>
/// A useful example class that allows you to invoke a delegate when Unity sends a OnTriggerStay message to your collider
/// passing in the collider object
/// </summary>
public class OnTriggerStayEvent : MonoBehaviour
{
	[ShowDelegate("OnTriggerStay", @canSetArgsFromEditor: false)]
	public ColliderAction onTriggerStay = new ColliderAction();

	void OnTriggerStay(Collider other)
	{
		onTriggerStay.Invoke(other);
	}
}