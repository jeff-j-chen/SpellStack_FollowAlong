using UnityEngine;

/// <summary>
/// An example class that gives the ability to modify components (add/remove/change state) on specified target
/// Very useful when used in combine with delegates to remotely modify the component
/// </summary>
[ExecuteInEditMode]
public abstract class ComponentModifier : MonoBehaviour
{
	/// <summary>
	/// The target gameObject to modify its components
	/// </summary>
	[HideInInspector, SerializeField]
	protected GameObject target;

	/// <summary>
	/// The target component to modify
	/// </summary>
	[HideInInspector, SerializeField]
	protected string component;

	/// <summary>
	/// If the target was null, then we target the gameObject that this script is attached to
	/// </summary>
	void OnEnable()
	{
		if (target == null)
			target = gameObject;
	}

	/// <summary>
	/// Returns true if the specified component is attached to the target gameObject
	/// </summary>
	protected bool IsComponentValid(out Component comp)
	{
		comp = target.GetComponent(component);
		return comp != null;
	}

	/// <summary>
	/// Modifies the target component - override to specify the modification behaviour
	/// </summary>
	public abstract void Modify();
}