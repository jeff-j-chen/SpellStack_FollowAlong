using uFAction;
using UnityEngine;

/// <summary>
/// An example class that gives the ability to remove components from a specified target
/// Very useful when used in combine with delegates to remotely remove the component
/// </summary>
public class ComponentRemover : ComponentModifier
{
	/// <summary>
	/// The delegate to invoke when the component's been removed from the target
	/// You could take this one step further and use a ComponentAction (UnityAction<Component>)
	/// to pass in the removed component to all the delegate's subscribers to let them know which component has been removed
	/// </summary>
	[HideInInspector] // drawn from a custom inspector instead
	public UnityAction onRemove = new UnityAction();

	/// <summary>
	/// Removes the component with the specified name (if found) from the target
	/// </summary>
	public void RemoveComponent(string componentName)
	{
		Component comp;
		if (IsComponentValid(out comp)) {
			if (Application.isPlaying)
				Destroy(comp);
			else DestroyImmediate(comp);
			onRemove.Invoke();
		}
	}

	/// <summary>
	/// Removes the component selected in the inspector from the target
	/// </summary>
	public void RemoveSelectedComponent()
	{
		RemoveComponent(component);
	}

	/// <summary>
	/// Removes the component selected in the inspector from the target
	/// </summary>
	public override void Modify()
	{
		RemoveSelectedComponent();
	}
}